//implemet show job details
import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getJobOpeningById } from "../../Services/JobOpeningService";

const JobOpeningDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [job, setJob] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchJob = async () => {
      try {
        const token = localStorage.getItem("token");
        const data = await getJobOpeningById(id, token);
        setJob(data);
      } catch (err) {
        console.error(err);
        setError("Failed to fetch job details.");
      } finally {
        setLoading(false);
      }
    };
    fetchJob();
  }, [id]);

  const parseJsonArray = (data) => {
    try {
      const parsed = JSON.parse(data);
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  };

  if (loading)
    return (
      <div className="text-center py-10 text-gray-500">
        Loading job details...
      </div>
    );

  if (error)
    return <div className="text-center py-10 text-red-500">{error}</div>;

  if (!job)
    return (
      <div className="text-center py-10 text-gray-500">Job not found.</div>
    );

  // Parsed requirement and benefits
  const requirements = parseJsonArray(job.requirement);
  const benefits = parseJsonArray(job.benefits);

  const statusColors = {
    Open: "bg-green-100 text-green-700 border-green-300",
    Closed: "bg-red-100 text-red-700 border-red-300",
    OnHold: "bg-yellow-100 text-yellow-700 border-yellow-300",
  };

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-5xl mx-auto bg-white shadow-md border border-gray-200 rounded-2xl p-8">
        {/* Header */}
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6">
          <div>
            <h1 className="text-3xl font-semibold text-indigo-700 mb-2">
              {job.title}
            </h1>
          </div>
           <div
              className={`inline-block px-3  py-4 text-sm font-medium rounded-full border ${
                statusColors[job.status]
              }`}
            >
              {job.status}
            </div>
          
        </div>

        {/* Description */}
        <section className="mb-6">
          <h2 className="text-lg font-semibold text-gray-800 mb-2">
            Description
          </h2>
          <p className="text-gray-700 leading-relaxed">{job.description}</p>
        </section>

        {/* Requirements */}
        {requirements.length > 0 && (
          <section className="mb-6">
            <h2 className="text-lg text-start mx-4 font-semibold text-gray-800 mb-2">
              Requirements
            </h2>
            <ul className=" text-start list-disc pl-6 text-gray-700 space-y-1">
              {requirements.map((req, i) => (
                <li key={i}>{req}</li>
              ))}
            </ul>
          </section>
        )}

        {/* Benefits */}
        {benefits.length > 0 && (
          <section className="mb-6">
            <h2 className="text-lg text-start mx-4 font-semibold text-gray-800 mb-2">
              Perks & Benefits
            </h2>
            <ul className="list-disc text-start pl-6 text-gray-700 space-y-1">
              {benefits.map((b, i) => (
                <li key={i}>{b}</li>
              ))}
            </ul>
          </section>
        )}

        {/* Job Info */}
        <section className="text-start mx-4 ">
          <div >

            <p className="mb-2">
              <span className="font-medium text-gray-800">Department: </span>
              {job.department || "N/A"}
            </p>
            <p className="mb-2" >
              <span className="font-medium text-gray-800">Location: </span>
              {job.location || "N/A"}
            </p>
            <p className="mb-2" >
              <span className="font-medium text-gray-800">Job Type: </span>
              {job.jobType || "N/A"}
            </p>
          </div>
          <div>
            <p className="mb-2" >
              <span className="font-medium text-gray-800">Experience: </span>
              {job.experience || "N/A"}
            </p>
            <p className="mb-2" >
              <span className="font-medium text-gray-800">Education: </span>
              {job.education || "N/A"}
            </p>
            <p className="mb-2" >
              <span className="font-medium text-gray-800">Deadline: </span>
              {job.deadLine
                ? new Date(job.deadLine).toLocaleDateString()
                : "N/A"}
            </p>
          </div>
        </section>

        {/* Reviewers */}
        <section className="mb-6">
          <h2 className="text-lg text-start mx-3 font-semibold text-gray-800 mb-2">
            Reviewers
          </h2>
          {job.reviewers && job.reviewers.length > 0 ? (
            <ul className="list-disc text-start  pl-6 text-gray-700 space-y-1">
              {job.reviewers.map((r) => (
                <li key={r.id}>{r.email}</li>
              ))}
            </ul>
          ) : (
            <p className="text-gray-600">No reviewers assigned.</p>
          )}
        </section>

        {/* Interviewers */}
        <section className="mb-6">
          <h2 className="text-lg text-start mx-4 font-semibold text-gray-800 mb-2">
            Interviewers
          </h2>
          {job.interviewers && job.interviewers.length > 0 ? (
            <ul className="list-disc text-start pl-6 text-gray-700 space-y-1">
              {job.interviewers.map((i) => (
                <li key={i.id}>{i.email}</li>
              ))}
            </ul>
          ) : (
            <p className="text-gray-600">No interviewers assigned.</p>
          )}
        </section>

        {/* Documents */}
        <section className="mb-6">
          <h2 className="text-lg text-start mx-4 font-semibold text-gray-800 mb-2">
            Required Documents
          </h2>
          {job.documents && job.documents.length > 0 ? (
            <ul className="list-disc text-start pl-6 text-gray-700 space-y-1">
              {job.documents.map((d) => (
                <li key={d.id}>
                  <span className="font-medium">{d.name}</span> —{" "}
                  {d.description}
                  {d.isMandatory && (
                    <span className="text-red-500 text-sm ml-2">
                      (Mandatory)
                    </span>
                  )}
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-gray-600">No documents linked.</p>
          )}
        </section>

        {/* Skills */}
        {job.jobSkills && job.jobSkills.length > 0 && (
          <section className="mb-6">
            <h2 className="text-lg text-start font-semibold text-gray-800 mb-2">Key Skills</h2>
            <div className="flex flex-wrap gap-2">
              {job.jobSkills.map((s) => (
                <span
                  key={s.skillId}
                  className={`px-3 py-1 mt-4 rounded-full text-sm font-medium border ${
                    s.isRequired
                      ? "bg-indigo-100 text-indigo-700 border-indigo-300"
                      : "bg-gray-100 text-gray-700 border-gray-300"
                  }`}
                >
                  {s.name}
                </span>
              ))}
            </div>
          </section>
        )}
      </div>
    </div>
  );
};

export default JobOpeningDetails;
