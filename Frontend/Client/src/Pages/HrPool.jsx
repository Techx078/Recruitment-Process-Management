import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { getHrPool } from "../Services/JobCandidateService";
import MyScheduledInterviews from "./MyScheduledInterviews";
const HrPool = () => {
  const { jobOpeningId } = useParams();
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();

  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchHrPool = async () => {
      try {
        setLoading(true);
        const data = await getHrPool(jobOpeningId);
        setCandidates(data);
      } catch (err) {
        setError(err.response?.data?.message || "Failed to load HR pool");
      } finally {
        setLoading(false);
      }
    };

    fetchHrPool();
  }, [jobOpeningId]);

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto mt-10 text-gray-600">
        Loading HR pool...
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        {error}
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto mt-10 px-4">
        <h2 className="text-xl font-semibold text-black mb-4">
          HR Interview Pool
        </h2>
      {candidates.length === 0 ? (
        <div className="p-4 text-gray-600">
          No candidates available for HR round.
        </div>
      ) : (
        <table className="w-full border-collapse">
          <thead className="bg-gray-100 border-b border-gray-300">
            <tr className="text-left text-gray-700">
              <th className="p-3">Name</th>
              <th className="p-3">Email</th>
              <th className="p-3">Job Title</th>
              <th className="p-3">Rounds Completed</th>
              <th className="p-3">Applied At</th>
              <th className="p-3">Actions</th>
            </tr>
          </thead>

          <tbody>
            {candidates.map((c) => (
              <tr
                key={c.jobCandidateId}
                className="border-b border-gray-200 hover:bg-gray-50"
              >
                <td className="p-3 text-gray-800">{c.candidateName}</td>

                <td className="p-3 text-gray-700">{c.email}</td>

                <td className="p-3 text-gray-700">{c.jobTitle}</td>

                <td className="p-3 text-gray-700">{c.roundCompleted}</td>

                <td className="p-3 text-gray-700">
                  {new Date(c.appliedAt).toLocaleDateString()}
                </td>

                <td className="p-3">
                  <div className="flex gap-2">
                    <button
                      onClick={() =>
                        window.open(`/candidate/Profile/${c?.userId}`, "_blank")
                      }
                      className="px-3 py-1.5 text-sm border border-gray-400 text-gray-700 rounded-md hover:bg-gray-200 transition"
                    >
                      View Profile
                    </button>
                    {authUser?.role === "Interviewer" && (
                      <button
                        onClick={() =>
                          navigate(`/schedule-interview/${c.jobCandidateId}`)
                        }
                        className="px-3 py-1.5 text-sm border border-black bg-black text-white rounded-md hover:bg-gray-800 transition"
                      >
                        Schedule / Interview
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    <MyScheduledInterviews jobOpeningId={jobOpeningId} />
    </div>
  );
};

export default HrPool;
