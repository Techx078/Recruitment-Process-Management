import { useEffect, useState } from "react";
import { getMyScheduledInterviews } from "../Services/JobCandidateService";
import { useNavigate } from "react-router-dom";
import { handleGlobalError } from "../Services/errorHandler";
const MyScheduledInterviews = ({ jobOpeningId }) => {
  const [interviews, setInterviews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();
  useEffect(() => {
    fetchMyInterviews();
  }, []);

  const fetchMyInterviews = async () => {
    try {
      const data = await getMyScheduledInterviews(jobOpeningId);
      setInterviews(data);
    } catch (error) {
      handleGlobalError(error)
      setError(error?.data?.Message || "not able to fetch scheduled interview !")
      return;
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="text-gray-600 mt-4">Loading scheduled interviews...</div>
    );
  }

  if (error) {
    return (
      <div className="mt-4 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        {error}
      </div>
    );
  }

  return (
    <div className="mt-8">
      <h3 className="text-xl font-semibold text-black mb-4">
        My Scheduled Interviews
      </h3>

      {interviews.length === 0 ? (
        <div className="bg-white border border-gray-300 rounded-lg p-5 text-gray-600">
          No scheduled interviews found.
        </div>
      ) : (
        <div className="overflow-x-auto bg-white border border-gray-300 rounded-lg">
          <table className="min-w-full border-collapse">
            <thead className="bg-gray-100 border-b border-gray-300">
              <tr>
                <th className="px-4 py-3 text-sm font-medium text-black">
                  Candidate
                </th>
                <th className="px-4 py-3 text-sm font-medium text-black">
                  Email
                </th>
                <th className="px-4 py-3 text-sm font-medium text-black">
                  Type
                </th>
                <th className="px-4 py-3 text-sm font-medium text-black">
                  Round
                </th>
                <th className="px-4 py-3 text-sm font-medium text-black">
                  Scheduled At
                </th>
                <th className="px-4 py-3 text-sm font-medium text-black">
                  Meeting
                </th>
              </tr>
            </thead>

            <tbody>
              {interviews.map((i) => (
                <tr
                  key={i.jobInterviewId}
                  className="border-b border-gray-200 hover:bg-gray-50 transition"
                >
                  <td className="px-4 py-3 text-sm text-black">
                    {i.candidateName}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {i.candidateEmail}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {i.interviewType}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {i.roundNumber}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {new Date(i.scheduledAt).toLocaleString()}
                  </td>

                  <td className="px-4 py-3">
                     <div className="flex flex-wrap gap-2">
                    {i.meetingLink ? (
                      <a
                        href={i.meetingLink}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="px-3 py-1.5 text-sm border border-black bg-black text-white rounded-md hover:bg-gray-800 transition"
                      >
                        Join
                      </a>
                    ) : (
                      <span className="text-gray-500 text-sm">N/A</span>
                    )}
                    <button
                      onClick={() => navigate(`/interview-feedback/${i.jobCandidateId}`)}
                      className="px-3 py-1.5 text-sm border border-black text-white bg-black rounded-md hover:bg-gray-800 transition"
                    >
                      Give Feedback
                    </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default MyScheduledInterviews;
