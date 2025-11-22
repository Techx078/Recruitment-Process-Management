import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getJobOpeningById , updateJobFields } from "../../Services/JobOpeningService";

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
          status: job.status,
          experience: job.experience,
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
      <div className="text-center py-10 text-gray-500">
        Loading job info...
      </div>
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

        {/* EXPERIENCE */}
        <label className="font-medium text-gray-700">Experience</label>
        <input
          type="text"
          value={formData.experience}
          onChange={(e) =>
            setFormData({ ...formData, experience: e.target.value })
          }
          className="w-full border p-2 rounded mb-4"
        />

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

        {/* REQUIREMENTS */}
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
