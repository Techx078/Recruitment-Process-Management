import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getCandidateDetails,
  updateCandidateService,
} from "../../Services/CandidateService";
import { getAllSkills } from "../../Services/JobOpeningService";

export default function CandidateUpdate() {
  const { UserId } = useParams(); 
  const navigate = useNavigate();

  const id = UserId;

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  const [allSkills, setAllSkills] = useState([]);

  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    linkedInProfile: "",
    gitHubProfile: "",
    Domain: "",
    DomainExperienceYears: 0,
  });

  const [resumeFile, setResumeFile] = useState(null);
  const [resumePath, setResumePath] = useState(null);

  const [educations, setEducations] = useState([]);
  const [skills, setSkills] = useState([]);

  const DOMAIN_OPTIONS = [
    "NotSpecified",
    "FullStackDevelopment",
    "FrontendDevelopment",
    "BackendDevelopment",
    "MobileAppDevelopment",
    "DataScience",
    "ArtificialIntelligence_ML",
    "CloudComputing",
    "DevOps",
    "IndustrialIoT",
    "EmbeddedSystems",
    "AutomationEngineering",
    "SupplyChainTech",
    "QualityAssurance",
    "CyberSecurity",
  ];

  const DEGREE_OPTIONS = [
    "B.Tech",
    "M.Tech",
    "BCA",
    "MCA",
    "B.Sc",
    "M.Sc",
    "B.Com",
    "M.Com",
    "MBA",
    "Diploma",
    "PhD",
  ];

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    try {
      setLoading(true);
      let token = localStorage.getItem("token");

      const [candidate, skillsList] = await Promise.all([
        getCandidateDetails(id, token),
        getAllSkills(token),
      ]);
      setAllSkills(skillsList);
      setFormData({
        fullName: candidate.user.fullName,
        email: candidate.user.email,
        phoneNumber: candidate.user.phoneNumber,
        linkedInProfile: candidate.linkedInProfile,
        gitHubProfile: candidate.gitHubProfile,
        Domain: candidate.user.domain,
        DomainExperienceYears: candidate.user.domainExperienceYears,
      });
      setEducations(candidate.educations);
      setSkills(candidate.user.skills);
      setResumePath(candidate.resumePath);
      setLoading(false);
    } catch (err) {
      setLoading(false);
      alert("Failed to load candidate: " + err.message);
    }
  }

  // update input fields
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleFileChange = (e) => {
    setResumeFile(e.target.files[0]);
  };

  // Education handlers
  const handleEducationChange = (index, field, value) => {
    const updated = [...educations];
    updated[index][field] = value;
    setEducations(updated);
  };

  const addEducationField = () => {
    setEducations([
      ...educations,
      {
        degree: "",
        university: "",
        college: "",
        passingYear: "",
        percentage: "",
      },
    ]);
  };

  const removeEducationField = (index) => {
    setEducations(educations.filter((_, i) => i !== index));
  };

  // Skill handlers
  const handleSkillChange = (index, field, value) => {
    const updated = [...skills];
    updated[index][field] = value;
    setSkills(updated);
  };

  const addSkillField = () => {
    setSkills([...skills, { name: "", experience: "" }]);
  };

  // Upload resume
  const uploadResume = async (file) => {
    const data = new FormData();
    data.append("file", file);
    let token = localStorage.getItem("token");

    const res = await fetch("http://localhost:5233/api/Auth/upload-resume", {
      method: "POST",
      headers: { Authorization: `Bearer ${token}` },
      body: data,
    });

    if (!res.ok) throw new Error("Resume upload failed");
    const json = await res.json();
    return json.resumeUrl;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);

    try {
      let finalResumePath = resumePath;

      if (resumeFile) {
        finalResumePath = await uploadResume(resumeFile);
      }

      const payload = {
        ...formData,
        resumePath: finalResumePath,
        educations,
        skills,
      };

      let token = localStorage.getItem("token");
      console.log(payload);
      await updateCandidateService(id, payload, token);

      alert("Candidate Profile Updated!");
      navigate(`/candidate/Profile/${id}`);
    } catch (err) {
      alert("Update failed: ");
    } finally {
      setSaving(false);
    }
  };

  if (loading)
    return (
      <div className="flex justify-center mt-10 text-xl text-gray-700">
        Loading Candidate...
      </div>
    );

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100 p-4">
      <div className="w-full max-w-3xl bg-white rounded-2xl shadow-lg p-8">
        <h2 className="text-3xl font-semibold text-center mb-6 text-gray-800">
          Update Candidate Profile
        </h2>

        <form
          onSubmit={handleSubmit}
          className="grid grid-cols-1 md:grid-cols-2 gap-5"
        >
          {/* Full Name */}
          <div>
            <label>Full Name</label>
            <input
              name="fullName"
              value={formData?.fullName}
              onChange={handleChange}
              className="w-full border p-2 rounded-lg"
              required
            />
          </div>

          {/* Email */}
          <div>
            <label>Email</label>
            <input
              type="email"
              disabled
              value={formData?.email}
              className="w-full border p-2 rounded-lg bg-gray-200"
            />
          </div>

          {/* Phone */}
          <div>
            <label>Phone Number</label>
            <input
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleChange}
              className="w-full border p-2 rounded-lg"
            />
          </div>

          {/* LinkedIn */}
          <div>
            <label>LinkedIn</label>
            <input
              name="linkedInProfile"
              value={formData.linkedInProfile}
              onChange={handleChange}
              className="w-full border p-2 rounded-lg"
            />
            <p>sample:https://www.linkedin.com/in/xyz</p>
          </div>
            
          {/* Github */}
          <div>
            <label>GitHub</label>
            <input
              name="gitHubProfile"
              value={formData.gitHubProfile}
              onChange={handleChange}
              className="w-full border p-2 rounded-lg"
            />
            <p>sample:https://github.com/xyz</p>
          </div>
            
          {/* Resume */}
          <div>
            <label>Upload New Resume</label>
            <input
              className="w-full border p-2 rounded-lg"
              type="file"
              accept=".pdf,.doc,.docx"
              onChange={handleFileChange}
            />
            {resumePath && (
              <p className="text-blue-600 text-sm mt-1">
                Current:{" "}
                <a href={resumePath} target="_blank">
                  View Resume
                </a>
              </p>
            )}
          </div>

          {/* DOMAIN */}
          <div>
            <label>Primary Domain</label>
            <select
              name="Domain"
              value={formData.Domain}
              onChange={handleChange}
              className="border p-2 rounded-lg w-full"
            >
              <option value="">Select Domain</option>
              {DOMAIN_OPTIONS.map((d, i) => (
                <option key={i} value={d}>
                  {d}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label>Domain Experience (Years)</label>
            <input
              type="number"
              name="DomainExperienceYears"
              value={formData.DomainExperienceYears}
              onChange={handleChange}
              className="border p-2 rounded-lg w-full"
            />
          </div>

          {/* EDUCATIONS */}
          <div className="col-span-2">
            <label className="font-semibold">Education</label>

            {educations.map((edu, idx) => (
              <div key={idx} className="grid grid-cols-4 gap-3 my-3">
                <select
                  value={edu.degree}
                  onChange={(e) =>
                    handleEducationChange(idx, "degree", e.target.value)
                  }
                  className="border p-2 rounded-lg"
                >
                  <option value="">Degree</option>
                  {DEGREE_OPTIONS.map((d, i) => (
                    <option key={i} value={d}>
                      {d}
                    </option>
                  ))}
                </select>

                <input
                  placeholder="University"
                  value={edu.university}
                  onChange={(e) =>
                    handleEducationChange(idx, "university", e.target.value)
                  }
                  className="border p-2 rounded-lg"
                />

                <input
                  placeholder="College"
                  value={edu.college}
                  onChange={(e) =>
                    handleEducationChange(idx, "college", e.target.value)
                  }
                  className="border p-2 rounded-lg"
                />

                <p
                  className="text-red-500 cursor-pointer text-center"
                  onClick={() => removeEducationField(idx)}
                >
                  Remove
                </p>

                <input
                  type="number"
                  placeholder="Passing Year"
                  value={edu.passingYear}
                  onChange={(e) =>
                    handleEducationChange(idx, "passingYear", e.target.value)
                  }
                  className="border p-2 rounded-lg"
                />

                <input
                  type="number"
                  placeholder="Percentage"
                  value={edu.percentage}
                  onChange={(e) =>
                    handleEducationChange(idx, "percentage", e.target.value)
                  }
                  className="border p-2 rounded-lg"
                />
              </div>
            ))}

            <button
              onClick={addEducationField}
              type="button"
              className="text-blue-600"
            >
              + Add Education
            </button>
          </div>

          {/* SKILLS */}
          <div className="col-span-2">
            <label className="font-semibold">Skills</label>

            {skills.map((skill, index) => (
              <div key={index} className="flex gap-2 my-2">
                <select
                  value={skill.name}
                  onChange={(e) =>
                    handleSkillChange(index, "name", e.target.value)
                  }
                  className="w-1/2 border p-2 rounded-lg"
                >
                  <option value="">Select Skill</option>
                  {allSkills.map((s) => (
                    <option key={s.id} value={s.name}>
                      {s.name}
                    </option>
                  ))}
                </select>

                <input
                  type="number"
                  value={skill.experience}
                  onChange={(e) =>
                    handleSkillChange(index, "experience", e.target.value)
                  }
                  placeholder="Exp"
                  className="w-1/2 border p-2 rounded-lg"
                />
              </div>
            ))}

            <button
              onClick={addSkillField}
              type="button"
              className="text-blue-600"
            >
              + Add Skill
            </button>
          </div>

          {/* Submit */}
          <div className="col-span-2 mt-4">
            <button
              disabled={saving}
              className="w-full bg-blue-600 text-white py-2 rounded-lg"
            >
              {saving ? "Saving..." : "Update Profile"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
