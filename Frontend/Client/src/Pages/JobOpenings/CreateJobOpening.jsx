import React, { useState, useEffect } from "react";
import {
  getAllSkills,
  getAllReviewers,
  getAllInterviewers,
  getAllDocuments,
  createJobOpening,
} from "../../Services/JobOpeningService";

export default function JobOpeningForm({}) {
  const [currentStep, setCurrentStep] = useState(1);
  const [isLoading, setIsLoading] = useState(false);

  const [jobData, setJobData] = useState({
    title: "",
    description: "",
    salaryRange: "",
    location: "",
    department: "",
    jobType: "",
    education: "",
    status: "1", //  Open
    experience: "",
    responsibilities: [""],
    requirement: [""],
    benefits: [""],
    deadLine: "",
    reviewerIds: [],
    interviewerIds: [],
    documents: [],
    jobSkills: [],
  });

  const [skills, setSkills] = useState([]);
  const [reviewers, setReviewers] = useState([]);
  const [interviewers, setInterviewers] = useState([]);
  const [documents, setDocuments] = useState([]);

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    setSkills(await getAllSkills());
    setReviewers(await getAllReviewers());
    setInterviewers(await getAllInterviewers());
    setDocuments(await getAllDocuments());
  }

  // Handle input changes
  const handleInputChange = (e) => {
    setJobData({ ...jobData, [e.target.name]: e.target.value });
  };

  // For handling simple array inputs like responsibilities, requirements, benefits
  const handleSimpleArrayInput = (field, index, value) => {
    const updated = [...jobData[field]];
    updated[index] = value;
    setJobData({ ...jobData, [field]: updated });
  };

  // Add or remove items from simple array fields
  const addItem = (field) => {
    setJobData({ ...jobData, [field]: [...jobData[field], ""] });
  };

  // Remove item from simple array fields
  const removeItem = (field, index) => {
    const updated = jobData[field].filter((_, i) => i !== index);
    setJobData({ ...jobData, [field]: updated });
  };

  // Toggle selection for reviewers and interviewers
  const toggleCheckbox = (field, id) => {
    setJobData((prev) => {
      const arr = [...prev[field]];
      // Check if id already exists else add it
      if (arr.includes(id)) {
        return { ...prev, [field]: arr.filter((x) => x !== id) };
      } else {
        return { ...prev, [field]: [...arr, id] };
      }
    });
  };

  // Toggle skill selection
  const toggleSkill = (skill) => {
    setJobData((prev) => {
      // check if skill already exists
      const exists = prev.jobSkills.some((s) => s.skillName === skill.name);

      if (exists) {
        // remove
        return {
          ...prev,
          jobSkills: prev.jobSkills.filter((s) => s.skillName !== skill.name),
        };
      } else {
        // add
        return {
          ...prev,
          jobSkills: [
            ...prev.jobSkills,
            { skillName: skill.name, isRequired: true },
          ],
        };
      }
    });
  };

  const toggleDocument = (id, checked) => {
    setJobData((prev) => {
      // check if document already exists else add/remove
      if (checked) {
        return {
          ...prev,
          documents: [...prev.documents, { documentId: id, isMandatory: true }],
        };
      } else {
        return {
          ...prev,
          documents: prev.documents.filter((d) => d.documentId !== id),
        };
      }
    });
  };

  const nextStep = () => {
    if (currentStep < 3) {
      setCurrentStep(currentStep + 1);
    }
  };

  const prevStep = () => {
    if (currentStep > 1) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleSubmit = (e) => {
    if(setIsLoading) return;
    setIsLoading(true);
    e.preventDefault();
    try{
    const token = localStorage.getItem("token");
    createJobOpening(jobData, token);
    alert("Job Opening Created Successfully!");
    Navigate("/job-openings");
    }catch(error){
      console.error("Error creating job opening:", error);
      alert("Failed to create job opening. Please try again.");
    }finally{
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6 border rounded-lg shadow-md">
      <h2 className="text-2xl font-bold">Create Job Opening</h2>

      {/* Progress Indicator */}
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center flex-1">
          <div
            className={`flex items-center justify-center w-10 h-10 rounded-full ${
              currentStep >= 1 ? "bg-blue-600 text-white" : "bg-gray-300"
            }`}
          >
            1
          </div>
          <div
            className={`flex-1 h-1 mx-2 ${
              currentStep >= 2 ? "bg-blue-600" : "bg-gray-300"
            }`}
          ></div>
        </div>

        <div className="flex items-center flex-1">
          <div
            className={`flex items-center justify-center w-10 h-10 rounded-full ${
              currentStep >= 2 ? "bg-blue-600 text-white" : "bg-gray-300"
            }`}
          >
            2
          </div>
          <div
            className={`flex-1 h-1 mx-2 ${
              currentStep >= 3 ? "bg-blue-600" : "bg-gray-300"
            }`}
          ></div>
        </div>

        <div
          className={`flex items-center justify-center w-10 h-10 rounded-full ${
            currentStep >= 3 ? "bg-blue-600 text-white" : "bg-gray-300"
          }`}
        >
          3
        </div>
      </div>

      {/* Step Labels */}
      <div className="flex justify-between mb-6 text-sm">
        <div
          className={`flex-1 text-center ${
            currentStep === 1 ? "font-bold text-blue-600" : "text-gray-500"
          }`}
        >
          Basic Details
        </div>
        <div
          className={`flex-1 text-center ${
            currentStep === 2 ? "font-bold text-blue-600" : "text-gray-500"
          }`}
        >
          Selection
        </div>
        <div
          className={`flex-1 text-center ${
            currentStep === 3 ? "font-bold text-blue-600" : "text-gray-500"
          }`}
        >
          Additional Info
        </div>
      </div>

      {/* Step 1: Basic Details */}
      {currentStep === 1 && (
        <div className="space-y-6">
          {/* Title */}
          <div>
            <label className="block font-medium mb-1">Job Title *</label>
            <input
              type="text"
              name="title"
              value={jobData.title}
              onChange={handleInputChange}
              className="w-full border p-2 rounded"
              placeholder="Backend Developer (.NET Core)"
            />
          </div>

          {/* Description */}
          <div>
            <label className="block font-medium mb-1">Description *</label>
            <textarea
              name="description"
              value={jobData.description}
              onChange={handleInputChange}
              className="w-full border p-2 rounded"
              rows="4"
              placeholder="Enter job description..."
            />
          </div>

          {/* Salary */}
          <div>
            <label className="block font-medium mb-1">Salary Range</label>
            <input
              type="text"
              name="salaryRange"
              value={jobData.salaryRange}
              onChange={handleInputChange}
              className="w-full border p-2 rounded"
              placeholder="8-12 LPA"
            />
          </div>

          {/* Experience */}
          <div>
            <label className="block font-medium mb-1">Experience</label>
            <input
              type="text"
              name="experience"
              value={jobData.experience}
              onChange={handleInputChange}
              className="w-full border p-2 rounded"
              placeholder="3-5 years"
            />
          </div>

          {/* ENUM DROPDOWNS */}
          <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
            <div>
              <label className="block font-medium mb-1 text-sm">Location</label>
              <select
                name="location"
                value={jobData.location}
                onChange={handleInputChange}
                className="border p-2 rounded w-full"
              >
                <option value="">Select Location</option>
                <option value="1">Remote</option>
                <option value="2">Onsite</option>
                <option value="3">Hybrid</option>
              </select>
            </div>

            <div>
              <label className="block font-medium mb-1 text-sm">
                Department
              </label>
              <select
                name="department"
                value={jobData.department}
                onChange={handleInputChange}
                className="border p-2 rounded w-full"
              >
                <option value="">Select Department</option>
                <option value="1">Engineering</option>
                <option value="2">HumanResources</option>
                <option value="3">Marketing</option>
                <option value="4">Sales</option>
                <option value="5">Finance</option>
                <option value="6">Operations</option>
                <option value="7">Design</option>
                <option value="8">ITSupport</option>
                <option value="9">ProductManagement</option>
              </select>
            </div>

            <div>
              <label className="block font-medium mb-1 text-sm">Job Type</label>
              <select
                name="jobType"
                value={jobData.jobType}
                onChange={handleInputChange}
                className="border p-2 rounded w-full"
              >
                <option value="">Select Job Type</option>
                <option value="1">Full-Time</option>
                <option value="2">Part-Time</option>
                <option value="3">Contract</option>
                <option value="4">Internship</option>
                <option value="5">Temporary</option>
              </select>
            </div>

            <div>
              <label className="block font-medium mb-1 text-sm">
                Education
              </label>
              <select
                name="education"
                value={jobData.education}
                onChange={handleInputChange}
                className="border p-2 rounded w-full"
              >
                <option value="">Select Education</option>
                <option value="1">HighSchool</option>
                <option value="2">Diploma</option>
                <option value="3">Bachelors</option>
                <option value="4">Masters</option>
                <option value="5">Doctorate</option>
                <option value="6">Other</option>
              </select>
            </div>
          </div>

          {/* DEADLINE */}
          <div>
            <label className="block font-medium mb-1">Deadline</label>
            <input
              type="date"
              name="deadLine"
              value={jobData.deadLine}
              onChange={handleInputChange}
              className="border p-2 rounded w-full"
            />
          </div>
        </div>
      )}

      {/* Step 2: Selection (Reviewers, Interviewers, Documents, Skills) */}
      {currentStep === 2 && (
        <div className="space-y-6">
          {/* Reviewers */}
          <div className="border p-3 rounded">
            <label className="block font-semibold mb-2">Select Reviewers</label>
            <div className="space-y-2 max-h-60 overflow-auto border p-2 rounded">
              {reviewers?.map((r) => (
                <label
                  key={r.id}
                  className="block border p-2 rounded hover:bg-gray-50 cursor-pointer"
                >
                  <div className="flex items-start gap-3">
                    <input
                      type="checkbox"
                      checked={jobData.reviewerIds.includes(r.id)}
                      onChange={() => toggleCheckbox("reviewerIds", r.id)}
                      className="mt-1"
                    />
                    <div>
                      <div className="font-medium">{r.user?.email}</div>
                      <div className="text-sm text-gray-600">
                        Reviewer ID: {r.id}
                      </div>
                    </div>
                  </div>
                </label>
              ))}
            </div>
          </div>

          {/* Interviewers */}
          <div className="border p-3 rounded">
            <label className="block font-semibold mb-2">
              Select Interviewers
            </label>
            <div className="space-y-2 max-h-60 overflow-auto border p-2 rounded">
              {interviewers?.map((i) => (
                <label
                  key={i.id}
                  className="block border p-2 rounded hover:bg-gray-50 cursor-pointer"
                >
                  <div className="flex items-start gap-3">
                    <input
                      type="checkbox"
                      checked={jobData.interviewerIds.includes(i.id)}
                      onChange={() => toggleCheckbox("interviewerIds", i.id)}
                      className="mt-1"
                    />
                    <div>
                      <div className="font-medium">{i.user?.email}</div>
                      <div className="text-sm text-gray-600">
                        Interviewer ID: {i.id}
                      </div>
                    </div>
                  </div>
                </label>
              ))}
            </div>
          </div>

          {/* Documents */}
          <div className="border p-3 rounded">
            <label className="block font-semibold mb-2">
              Required Documents
            </label>
            <div className="space-y-2 max-h-40 overflow-auto border p-2 rounded">
              {documents.map((d) => (
                <label
                  key={d.id}
                  className="flex items-center gap-3 p-1 cursor-pointer hover:bg-gray-100 rounded"
                >
                  <input
                    type="checkbox"
                    checked={jobData.documents.some(
                      (doc) => doc.documentId === d.id
                    )}
                    onChange={(e) => toggleDocument(d.id, e.target.checked)}
                  />
                  <div>
                    <div className="font-medium">{d.name}</div>
                    <div className="text-sm text-gray-600">
                      Document ID: {d.id}
                    </div>
                  </div>
                </label>
              ))}
            </div>
          </div>

          {/* Skills */}
          <div className="border p-3 rounded">
            <label className="block font-semibold mb-2">Required Skills</label>
            <div className="space-y-2 max-h-40 overflow-auto border p-2 rounded">
              {skills.map((s) => {
                const isChecked = jobData.jobSkills.some(
                  (sk) => sk.skillName === s.name
                );
                return (
                  <label
                    key={s.skillId}
                    className="flex items-center gap-3 p-1 cursor-pointer hover:bg-gray-100 rounded"
                  >
                    <input
                      type="checkbox"
                      checked={isChecked}
                      onChange={() => toggleSkill(s)}
                    />
                    <div>
                      <div className="font-medium">{s.name}</div>
                      <div className="text-sm text-gray-600">
                        Skill ID: {s.skillId}
                      </div>
                    </div>
                  </label>
                );
              })}
            </div>
          </div>
        </div>
      )}

      {/* Step 3: Additional Info (Responsibilities, Requirements, Benefits) */}
      {currentStep === 3 && (
        <div className="space-y-6">
          {/* Responsibilities */}
          <div>
            <label className="block font-medium mb-1">Responsibilities</label>
            {jobData.responsibilities.map((resp, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <input
                  value={resp}
                  onChange={(e) =>
                    handleSimpleArrayInput(
                      "responsibilities",
                      index,
                      e.target.value
                    )
                  }
                  className="w-full border p-2 rounded"
                  placeholder="Enter responsibility"
                />
                {jobData.responsibilities.length > 1 && (
                  <button
                    type="button"
                    onClick={() => removeItem("responsibilities", index)}
                    className="border px-3 py-1 rounded text-red-600 hover:bg-red-50"
                  >
                    ×
                  </button>
                )}
              </div>
            ))}
            <button
              type="button"
              onClick={() => addItem("responsibilities")}
              className="border px-4 py-1 rounded hover:bg-gray-50"
            >
              + Add Responsibility
            </button>
          </div>

          {/* Requirements */}
          <div>
            <label className="block font-medium mb-1">Requirements</label>
            {jobData.requirement.map((req, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <input
                  value={req}
                  onChange={(e) =>
                    handleSimpleArrayInput("requirement", index, e.target.value)
                  }
                  className="w-full border p-2 rounded"
                  placeholder="Enter requirement"
                />
                {jobData.requirement.length > 1 && (
                  <button
                    type="button"
                    onClick={() => removeItem("requirement", index)}
                    className="border px-3 py-1 rounded text-red-600 hover:bg-red-50"
                  >
                    ×
                  </button>
                )}
              </div>
            ))}
            <button
              type="button"
              onClick={() => addItem("requirement")}
              className="border px-4 py-1 rounded hover:bg-gray-50"
            >
              + Add Requirement
            </button>
          </div>

          {/* Benefits */}
          <div>
            <label className="block font-medium mb-1">Benefits</label>
            {jobData.benefits.map((benefit, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <input
                  value={benefit}
                  onChange={(e) =>
                    handleSimpleArrayInput("benefits", index, e.target.value)
                  }
                  className="w-full border p-2 rounded"
                  placeholder="Enter benefit"
                />
                {jobData.benefits.length > 1 && (
                  <button
                    type="button"
                    onClick={() => removeItem("benefits", index)}
                    className="border px-3 py-1 rounded text-red-600 hover:bg-red-50"
                  >
                    ×
                  </button>
                )}
              </div>
            ))}
            <button
              type="button"
              onClick={() => addItem("benefits")}
              className="border px-4 py-1 rounded hover:bg-gray-50"
            >
              + Add Benefit
            </button>
          </div>
        </div>
      )}

      {/* Navigation Buttons */}
      <div className="flex justify-between mt-8 pt-6 border-t">
        <button
          type="button"
          onClick={prevStep}
          disabled={currentStep === 1}
          className={`px-6 py-2 rounded ${
            currentStep === 1
              ? "bg-gray-300 text-gray-500 cursor-not-allowed"
              : "bg-gray-600 text-white hover:bg-gray-700"
          }`}
        >
          Previous
        </button>

        {currentStep < 3 ? (
          <button
            type="button"
            onClick={nextStep}
            className="px-6 py-2 rounded bg-blue-600 text-white hover:bg-blue-700"
          >
            Next
          </button>
        ) : (
          <button
            type="button"
            onClick={handleSubmit}
            disabled={isLoading}
            className={`px-6 py-2 rounded text-white
        ${
          isLoading
            ? "bg-gray-400 cursor-not-allowed"
            : "bg-green-600 hover:bg-green-700"
        }`}
          >
            {isLoading ? "Creating..." : "Create Job Opening"}
          </button>
        )}
      </div>
    </div>
  );
}
