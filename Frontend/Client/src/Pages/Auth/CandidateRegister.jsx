import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {registerCandidate} from "../../Services/authService"


export default function CandidateRegister() {

  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    password: "",
    linkedInProfile: "",
    gitHubProfile: "",
  });
  const [educations, setEducations] = useState([
    {
      degree: "",
      university: "",
      college: "",
      passingYear: "",
      percentage: "",
    },
  ]);

  const [skills, setSkills] = useState([{ name: "", experience: "" }]);
  const [resumeFile, setResumeFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const navigateTo = useNavigate();
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

  // Handle education change
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
    setEducations((prev) => prev.filter((_, i) => i !== index));
  };

  // Handle skill input changes
  const handleSkillChange = (index, field, value) => {
    const updatedSkills = [...skills];
    updatedSkills[index][field] = value;
    setSkills(updatedSkills);
  };

  // Add new skill input fields
  const addSkillField = () => {
    setSkills([...skills, { name: "", experience: "" }]);
  };

  // Handle input field changes
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  // Handle resume file selection
  const handleFileChange = (e) => {
    setResumeFile(e.target.files[0]);
  };

  // Function to upload resume to Cloudinary
  const uploadToCloudinary = async (file) => {
    const formData = new FormData();
    formData.append("file", file);
    let token = localStorage.getItem("token")

    const res = await fetch("http://localhost:5233/api/Auth/upload-resume", {
      method: "POST",
      body: formData,
      headers:{
        Authorization : `Bearer ${token}`
      }
    });

    if (!res.ok) throw new Error("Failed to upload resume");
    const data = await res.json();
    return data.resumeUrl;
  };

  // Handle form submission for registration
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      let resumeUrl = null;

      //Upload resume to Cloudinary if file exists
      if (resumeFile) {
        resumeUrl = await uploadToCloudinary(resumeFile);
      }

      // Prepare candidate data for registration
      const candidateData = {
        ...formData,
        roleName: "Candidate",
        resumePath: resumeUrl,
        Educations: educations.map((edu) => ({
          degree: edu.degree,
          university: edu.university,
          college: edu.college,
          passingYear: parseInt(edu.passingYear),
          percentage: parseFloat(edu.percentage),
        })),
        skills: skills.map((skill) => ({
          name: skill.name,
          experience: skill.experience,
        })),
      };
      //register with data and token
      let token = localStorage.getItem('token');
      if( token == null ){
       alert("you are not a recruiter ! login with recruiter id");
       navigateTo("/login");
      }
      const response = await registerCandidate(candidateData, token);
      alert("Candidate registered successfully!");

      // Reset form
      setFormData({
        fullName: "",
        email: "",
        phoneNumber: "",
        password: "",
        linkedInProfile: "",
        gitHubProfile: "",
      });
      setResumeFile(null);
      setSkills([{ name: "", experience: "" }]);
     navigateTo("/Recruiter/Profile")
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100 p-4">
      <div className="w-full max-w-2xl bg-white rounded-2xl shadow-lg p-8">
        <h2 className="text-3xl font-semibold text-center mb-6 text-gray-800">
          Candidate Registration
        </h2>

        <form
          onSubmit={handleSubmit}
          className="grid grid-cols-1 md:grid-cols-2 gap-5"
        >
          {/* Full Name */}
          <div>
            <label className="block text-gray-700 mb-1">Full Name</label>
            <input
              type="text"
              name="fullName"
              value={formData.fullName}
              onChange={handleChange}
              placeholder="Enter your full name"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Email */}
          <div>
            <label className="block text-gray-700 mb-1">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="Enter your email"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Phone */}
          <div>
            <label className="block text-gray-700 mb-1">Phone Number</label>
            <input
              type="Number"
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleChange}
              placeholder="Enter phone number"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Password */}
          <div>
            <label className="block text-gray-700 mb-1">Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Enter password"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {/* LinkedIn */}
          <div>
            <label className="block text-gray-700 mb-1">LinkedIn Profile</label>
            <input
              type="url"
              name="linkedInProfile"
              value={formData.linkedInProfile}
              onChange={handleChange}
              placeholder="LinkedIn URL"
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* GitHub */}
          <div>
            <label className="block text-gray-700 mb-1">GitHub Profile</label>
            <input
              type="url"
              name="gitHubProfile"
              value={formData.gitHubProfile}
              onChange={handleChange}
              placeholder="GitHub URL"
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Resume Upload */}
          <div>
            <label className="block text-gray-700 mb-1">Upload Resume</label>
            <input
              type="file"
              accept=".pdf,.doc,.docx , .jpg, .png"
              onChange={handleFileChange}
              className="w-full border border-gray-300 rounded-lg p-2 cursor-pointer focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {/* EDUCATION SECTION */}
          <div className="col-span-2">
            <label className="font-semibold text-gray-700">Education</label>

            {educations.map((edu, index) => (
              <div
                key={index}
                className="grid grid-cols-4 gap-3 my-3 items-center"
              >
                
                <select
                  value={edu.degree}
                  onChange={(e) =>
                    handleEducationChange(index, "degree", e.target.value)
                  }
                  className="border p-2 rounded-lg w-full"
                >
                  <option value="">Degree</option>
                  {DEGREE_OPTIONS.map((degree, idx) => (
                    <option key={idx} value={degree}>
                      {degree}
                    </option>
                  ))}
                </select>

                <input
                  type="text"
                  placeholder="University"
                  value={edu.university}
                  onChange={(e) =>
                    handleEducationChange(index, "university", e.target.value)
                  }
                  className="border p-2 rounded-lg w-full"
                />

                <input
                  type="text"
                  placeholder="College"
                  value={edu.college}
                  onChange={(e) =>
                    handleEducationChange(index, "college", e.target.value)
                  }
                  className="border p-2 rounded-lg w-full"
                />

                
                <div className="row-span-2 flex justify-center items-center">
                  <p
                    onClick={() => removeEducationField(index)}
                    className="text-red-600 text-xl cursor-pointer"
                  >
                    <i className="fa-solid fa-ban"></i>
                  </p>
                </div>

                
                <input
                  type="number"
                  placeholder="Passing Year"
                  value={edu.passingYear}
                  onChange={(e) =>
                    handleEducationChange(index, "passingYear", e.target.value)
                  }
                  className="border p-2 rounded-lg w-full"
                />

                <input
                  type="number"
                  placeholder="Percentage"
                  value={edu.percentage}
                  onChange={(e) =>
                    handleEducationChange(index, "percentage", e.target.value)
                  }
                  className="border p-2 rounded-lg w-full"
                />
                {/* Empty spacing cell */}
                <div></div>
              </div>
            ))}

            <button
              type="button"
              onClick={addEducationField}
              className="text-blue-600 text-sm"
            >
              {" "}
              + Add Another Education{" "}
            </button>
          </div>

          <div className="col-span-2">
            <label className="block text-gray-700 mb-2">Skills</label>

            {skills.map((skill, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <input
                  type="text"
                  placeholder="Skill Name (e.g. React)"
                  value={skill.name}
                  onChange={(e) =>
                    handleSkillChange(index, "name", e.target.value)
                  }
                  className="w-1/2 border border-gray-300 rounded-lg p-2"
                />
                <input
                  type="number"
                  placeholder="Experience (in years)"
                  value={skill.experience}
                  onChange={(e) =>
                    handleSkillChange(index, "experience", e.target.value)
                  }
                  className="w-1/2 border border-gray-300 rounded-lg p-2"
                />
              </div>
            ))}

            <button
              type="button"
              onClick={addSkillField}
              className="text-blue-600 text-sm mt-1"
            >
              + Add Another Skill
            </button>
          </div>

          {/* Submit */}
          <div className="col-span-2 mt-4">
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-all"
            >
              {loading ? "Registering..." : "Register"}
            </button>
          </div>
        </form>

        {error && <p className="text-red-600 text-center mt-3">{error}</p>}

        <p className="text-center text-sm text-gray-600 mt-4">
          Already have an account?{" "}
          <Link
            to="/login"
            className="text-blue-600 hover:underline font-medium"
          >
            Login
          </Link>
        </p>
      </div>
    </div>
  );
}
