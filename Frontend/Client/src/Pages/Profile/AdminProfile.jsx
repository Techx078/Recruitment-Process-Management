import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getAllJobOpenings } from "../../Services/JobOpeningService";
import { handleGlobalError } from "../../Services/errorHandler";

export default function AdminProfile() {
  const [jobOpenings, setJobOpenings] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchJobOpenings = async () => {
      try {
        setLoading(true);
        const data = await getAllJobOpenings(
          localStorage.getItem("token")
        );
        setJobOpenings(data);
      } catch (e) {
        handleGlobalError(e);
      } finally {
        setLoading(false);
      }
    };

    fetchJobOpenings();
  }, []);

  if (loading) {
    return <div className="p-6">Loading...</div>;
  }

  return (
    <div className="w-full min-h-screen bg-gray-100 p-6">
      <div className="max-w-6xl mx-auto space-y-6">
        {/* Admin Header */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold">Admin Dashboard</h2>
          <p className="text-gray-600 mt-1">
            View all job openings across the organization
          </p>
        </div>

        {/* Job Openings */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">
            All Job Openings
          </h2>

          {jobOpenings.length === 0 ? (
            <p className="text-gray-500">No job openings found.</p>
          ) : (
            <div className="space-y-4">
              {jobOpenings.map((job) => (
                <div
                  key={job.jobOpeningId}
                  className="border rounded-xl p-4 shadow-sm bg-white"
                >
                  <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-3">
                    <div>
                      <h3 className="text-lg sm:text-xl font-semibold">
                        {job.title}
                      </h3>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Department: {job.department}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Domain: {job.domain}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Experience: {job.minDomainExperience}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Created:{" "}
                        {new Date(job.createdAt).toLocaleDateString()}
                      </p>
                    </div>

                    <div className="flex flex-col items-start md:items-end gap-1">
                      <span className="bg-green-100 text-green-700 px-3 py-1 rounded-lg text-xs sm:text-sm font-medium">
                        {job.status}
                      </span>

                      <p className="text-xs sm:text-sm text-gray-700">
                        Candidates: {job.candidateCount}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-700">
                        Type: {job.jobType}
                      </p>
                    </div>
                  </div>

                  {/* ACTION BUTTON */}
                  <div className="mt-4">
                    <button
                      className="px-4 py-2 bg-black text-white text-sm rounded-lg hover:bg-gray-800"
                      onClick={() =>
                        navigate(`/Dashboard/${job.id}`)
                      }
                    >
                      Dashboard
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
