import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getCandidateInterviewHistory } from "../Services/JobCandidateService";
import { handleGlobalError } from "../Services/errorHandler";
export default function CandidateInterviewHistory() {
  const { jobCandidateId } = useParams();
  const navigate = useNavigate();

  const [interviewHistory, setInterviewHistory] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchHistory = async () => {
      try {
        const data = await getCandidateInterviewHistory(jobCandidateId);
        console.log(data);
        
        setInterviewHistory(data);
      } catch (err) {
        handleGlobalError(err);
        setError( err.data.Message || "Failed to load interview history");
      } finally {
        setLoading(false);
      }
    };

    fetchHistory();
  }, [jobCandidateId]);

  if (loading) {
    return <div className="p-6 text-gray-600">Loading interview history...</div>;
  }

  if (error) {
    return <div className="p-6 text-red-600">{error}</div>;
  }

  if (!interviewHistory) {
    return <div className="p-6 text-gray-600">No history found</div>;
  }

  return (
    <div className="max-w-5xl mx-auto mt-8 bg-white border border-gray-300 rounded">
      {/* Header */}
      <div className="p-4 border-b border-gray-300 flex justify-between items-center">
        <div>
          <h2 className="text-lg font-semibold text-gray-800">
            Interview History
          </h2>
          <p className="text-sm text-gray-600">
            {interviewHistory.candidateName} •{" "}
            {interviewHistory.jobTitle}
          </p>
        </div>

        <button
          onClick={() => navigate(-1)}
          className="px-3 py-1 border border-gray-400 rounded text-sm text-gray-700 hover:bg-gray-200"
        >
          Back
        </button>
      </div>

      {/* Current Status */}
      <div className="p-4">
        <span className="px-3 py-1 text-sm border border-gray-500 rounded text-gray-700">
          Current Status: {interviewHistory.currentStatus}
        </span>
      </div>

      {/* History List */}
      <div className="p-4">
        <div className="relative border-l border-gray-300 ml-4">
          {interviewHistory.timeline.map((event, index) => (
            <div key={index} className="mb-6 ml-6">
              <div className="absolute -left-[7px] w-3 h-3 bg-gray-600 rounded-full"></div>

              <div className="bg-gray-50 border border-gray-300 rounded p-3">
                <h3 className="text-sm font-semibold text-gray-800">
                  {event.title}
                </h3>

                <p className="text-sm text-gray-600 mt-1">
                  {event.description}
                </p>

                <p className="text-xs text-gray-500 mt-1">
                  {new Date(event.date).toLocaleString()}
                </p>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
