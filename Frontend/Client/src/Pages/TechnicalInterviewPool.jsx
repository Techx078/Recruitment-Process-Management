import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getTechnicalInterviewPool } from "../Services/JobCandidateService";
import MyScheduledInterviews from "./MyScheduledInterviews";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { getJobOpeningById } from "../Services/JobOpeningService.js";
const TechnicalInterviewPool = () => {
  const { jobOpeningId } = useParams();
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const [candidates, setCandidates] = useState([]);
  const [jobOpeningInterviewer, setJobOpeningInterviewer] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchInterviewerList();
    fetchTechnicalPool();
  }, []);

  const fetchTechnicalPool = async () => {
    try {
      const data = await getTechnicalInterviewPool(jobOpeningId);
      setCandidates(data);
    } catch (error) {
      if (!error.status) {
        setError("Network error. Please try again.");
        return;
      }
      const { status, data } = error;
      if (status === 400 && data.errors) {
        if (Array.isArray(data.errors)) {
          data.errors.forEach((msg) => alert(msg));
        } else {
          Object.values(data.errors)
            .flat()
            .forEach(() => alert("fields are required"));
        }
        return;
      }
      setError(data.Message || "Something went wrong");
      return;
    } finally {
      setLoading(false);
    }
  };
  const fetchInterviewerList = async () => {
    await getJobOpeningById(jobOpeningId, localStorage.getItem("token"))
      .then((response) => {
        setJobOpeningInterviewer(response.interviewers);
      })
      .catch((error) => {
        console.error("Error fetching job opening reviewers:", error);
      });
  };

  if (
    authUser.role === "Interviewer" &&
    !jobOpeningInterviewer.some((i) => i.email === authUser.email)
  ) {
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        only assigned interviewer can access.
      </div>
    );
  }

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto mt-10 text-gray-600">
        Loading technical interview pool...
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
      <h2 className="text-2xl font-semibold text-black mb-6">
        Technical Interview Pool
      </h2>

      {candidates.length === 0 ? (
        <div className="bg-white border border-gray-300 rounded-lg p-6 text-gray-600">
          No candidates available for technical interview.
        </div>
      ) : (
        <div className="overflow-x-auto bg-white border border-gray-300 rounded-lg shadow-sm">
          <table className="min-w-full border-collapse">
            <thead className="bg-gray-100 border-b border-gray-300">
              <tr>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Candidate Name
                </th>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Job Title
                </th>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Round
                </th>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Last Updated
                </th>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Actions
                </th>
              </tr>
            </thead>

            <tbody>
              {candidates?.map((c) => (
                <tr
                  key={c?.jobCandidateId}
                  className="border-b border-gray-200 hover:bg-gray-50 transition"
                >
                  <td className="px-4 py-3 text-sm text-black">
                    {c?.candidateName}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {c?.jobTitle}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {c?.roundNumber}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {c?.lastUpdatedAt
                      ? new Date(c?.lastUpdatedAt).toLocaleDateString()
                      : "-"}
                  </td>

                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        onClick={() =>
                          window.open(
                            `/candidate/Profile/${c?.userId}`,
                            "_blank"
                          )
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
        </div>
      )}
      {authUser.role == "Interviewer" &&
      <MyScheduledInterviews jobOpeningId={jobOpeningId} />
      }
    </div>
  );
};

export default TechnicalInterviewPool;
