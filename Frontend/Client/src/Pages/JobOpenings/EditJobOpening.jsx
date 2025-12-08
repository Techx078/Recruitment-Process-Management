import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  getJobOpeningById,
  updateJobFields,
} from "../../Services/JobOpeningService";
import {
  DOMAIN_OPTIONS,
  EDUCATION_OPTIONS,
  JOB_STATUS_OPTIONS,
  JOB_TYPE_OPTIONS,
  JOB_LOCATION_OPTIONS,
  DEPARTMENT_OPTIONS,
} from "../../Assets/Arrays_for_options/Array";

const EditJobOpening = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [formData, setFormData] = useState(null);
  const [loading, setLoading] = useState(true);

  const parseJsonArray = (data) => {
    try {
      const parsed = JSON.parse(data);
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  };

  useEffect(() => {
    const loadData = async () => {
      try {
        const token = localStorage.getItem("token");
        const job = await getJobOpeningById(id, token);

        setFormData({
          title: job.title,
          description: job.description,
          salaryRange: job.salaryRange,
          location: job.location,
          department: job.department,
          jobType: job.jobType,
          education: job.education,
          domain: job.domain,
          status: job.status,
          minDomainExperience: job.minDomainExperience,
          responsibilities: parseJsonArray(job.responsibilities),
          requirement: parseJsonArray(job.requirement),
          benefits: parseJsonArray(job.benefits),
          deadLine: job.deadLine ? job.deadLine.split("T")[0] : "",
        });
      } catch (err) {
        console.error("Failed to load job", err);
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [id]);

  const handleSave = async () => {
    try {
      const token = localStorage.getItem("token");
      console.log(formData);
      await updateJobFields(id, formData, token);
      alert("Job updated successfully!");
      navigate(`/job-openings/${id}`);
    } catch (err) {
      console.error(err);
      alert("Failed to update job. form");
    }
  };

  if (loading || !formData) {
    return (
      <div className="text-center py-10 text-gray-500">Loading job info...</div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-3xl mx-auto bg-white shadow-md border rounded-lg p-8">
        <h1 className="text-2xl font-bold text-indigo-700 mb-6">
          Edit Job Opening
        </h1>

        {/* TITLE */}
        <label className="font-medium text-gray-700">Title</label>
        <input
          type="text"
          value={formData.title}
          onChange={(e) => setFormData({ ...formData, title: e.target.value })}
          className="w-full border p-2 rounded mb-4"
        />

        {/* DESCRIPTION */}
        <label className="font-medium text-gray-700">Description</label>
        <textarea
          value={formData.description}
          onChange={(e) =>
            setFormData({ ...formData, description: e.target.value })
          }
          className="w-full border p-2 rounded mb-4"
          rows={4}
        ></textarea>
        {/* salaryRange */}
        <label className="font-medium text-gray-700">salaryRange</label>
        <input
          type="text"
          value={formData.salaryRange}
          onChange={(e) =>
            setFormData({ ...formData, salaryRange: e.target.value })
          }
          className="w-full border p-2 rounded mb-4"
        />

        {/* minDomainExperience */}
        <label className="font-medium text-gray-700">
          Minimum Domain Experience
        </label>
        <input
          type="text"
          value={formData.minDomainExperience}
          onChange={(e) =>
            setFormData({ ...formData, minDomainExperience: e.target.value })
          }
          className="w-full border p-2 rounded mb-4"
        />

        {/* {Domain} */}
        <div>
          <label className="block font-medium mb-1 text-sm">Domain</label>
          <select
            name="domain"
            value={formData.domain}
            onChange={(e) => {
              setFormData({ ...formData, domain: Number(e.target.value) });
            }}
            className="border p-2 rounded w-full"
          >
            <option value="">select domain</option>
            {DOMAIN_OPTIONS.map((opt) => (
              <option key={opt.id} value={opt.id}>
                {opt.label}
              </option>
            ))}
          </select>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
          {/* Location */}
          <div>
            <label className="block font-medium mb-1 text-sm">Location</label>
            <select
              name="location"
              value={formData.location}
              onChange={(e) => {
                setFormData({ ...formData, location: Number(e.target.value) });
              }}
              className="border p-2 rounded w-full"
            >
              <option value="">Select Location</option>
              {JOB_LOCATION_OPTIONS.map((opt) => (
                <option key={opt.id} value={opt.id}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          {/* Department */}
          <div>
            <label className="block font-medium mb-1 text-sm">Department</label>
            <select
              name="department"
              value={formData.department}
              onChange={(e)=>{
                setFormData({ ...formData, department: Number(e.target.value) })
              }
            }
              className="border p-2 rounded w-full"
            >
              <option value="">Select Department</option>
              {DEPARTMENT_OPTIONS.map((opt) => (
                <option key={opt.id} value={opt.id}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          {/* Job Type */}
          <div>
            <label className="block font-medium mb-1 text-sm">Job Type</label>
            <select
              name="jobType"
              value={formData.jobType}
              onChange={(e)=>{
                setFormData({ ...formData, jobType: Number(e.target.value) })
              }}
              className="border p-2 rounded w-full"
            >
              <option value="">Select Job Type</option>
              {JOB_TYPE_OPTIONS.map((opt) => (
                <option key={opt.id} value={opt.id}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          {/* Education */}
          <div>
            <label className="block font-medium mb-1 text-sm">
              Education level
            </label>
            <select
              name="education"
              value={formData.education}
              onChange={(e)=>{
                setFormData({ ...formData, education: Number(e.target.value) })
              }}
              className="border p-2 rounded w-full"
            >
              <option value="">Select Education</option>
              {EDUCATION_OPTIONS.map((opt) => (
                <option key={opt.id} value={opt.id}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* DEADLINE */}
        <label className="font-medium text-gray-700">Deadline</label>
        <input
          type="date"
          value={formData.deadLine}
          onChange={(e) =>
            setFormData({ ...formData, deadLine: e.target.value })
          }
          className="w-full border p-2 rounded mb-4"
        />

        {/* RESPONSIBILITIES */}
        <>
          <label className="font-medium text-gray-700">Responsibilities</label>
          <textarea
            value={formData.responsibilities.join("\n")}
            onChange={(e) =>
              setFormData({
                ...formData,
                responsibilities: e.target.value.split("\n"),
              })
            }
            className="w-full border p-2 rounded mb-4"
            rows={4}
          />
        </>

        {/* REQUIREMENTS */}
        <>
          <label className="font-medium text-gray-700">Requirements</label>
          <textarea
            value={formData.requirement.join("\n")}
            onChange={(e) =>
              setFormData({
                ...formData,
                requirement: e.target.value.split("\n"),
              })
            }
            className="w-full border p-2 rounded mb-4"
            rows={4}
          />
        </>

        {/* BENEFITS */}
        <label className="font-medium text-gray-700">Benefits</label>
        <textarea
          value={formData.benefits.join("\n")}
          onChange={(e) =>
            setFormData({
              ...formData,
              benefits: e.target.value.split("\n"),
            })
          }
          className="w-full border p-2 rounded mb-4"
          rows={4}
        />

        {/* ACTION BUTTONS */}
        <div className="flex gap-4 mt-6">
          <button
            onClick={handleSave}
            className=" bg-black text-white px-5 py-2 rounded hover:bg-black-700"
          >
            Save Changes
          </button>

          <button
            onClick={() => navigate(`/job-openings/${id}`)}
            className="bg-black text-white px-5 py-2 rounded hover:bg-gray-600"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
};

export default EditJobOpening;
