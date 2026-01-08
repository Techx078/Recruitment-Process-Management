import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getFinalPool , selectCandidate } from "../../../Services/JobCandidateService";
import { handleGlobalError } from "../../../Services/errorHandler";

const FinalPool = () => {
  const { jobOpeningId } = useParams();
  const navigate = useNavigate();

  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchFinalPool = async () => {
      try {
        setLoading(true);
        const data = await getFinalPool(jobOpeningId);
        setCandidates(data);
      } catch (err) {
        handleGlobalError(err);
        setError(err?.data?.Message || "Failed to load final pool");
      } finally {
        setLoading(false);
      }
    };

    fetchFinalPool();
  }, [jobOpeningId]);

const handleSelect = async (jobCandidateId) => {
  if (!window.confirm("Are you sure you want to select this candidate?"))
    return;

  try {
    const response = await selectCandidate(jobCandidateId);

    // update UI
    setCandidates((prev) =>
      prev.map((c) =>
        c.jobCandidateId === jobCandidateId
          ? { ...c, status: "Selected" }
          : c
      )
    );

    alert(response.message);
  } catch (err) {
    console.log(err);
    
    alert(
      err.response?.data?.message ||
      "Failed to select candidate"
    );
  }
};
const handleSendOffer = async (jobCandidateId)=>{
   if (!window.confirm("Are you sure you want to select this candidate?"))
    return;

  try {
    navigate(`/send-offer/${jobCandidateId}`)
  } catch (err) {
    handleGlobalError(err)
  }
}

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto mt-10 text-gray-600">
        Loading final pool...
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
    
    <div className="max-w-6xl mx-auto mt-10 ">
      <h2 className="text-lg font-semibold text-gray-800">
          Final Pool Candidates
        </h2>
      {candidates.length === 0 ? (
        <div className="p-4 text-gray-600">
          No candidates found.
        </div>
      ) : (
        <div className="w-full bg-white rounded-lg shadow-sm">
        <table className="w-full border-collapse">
          <thead className="bg-gray-100 border-b border-gray-300">
            <tr className="text-left text-gray-700">
              <th className="p-3">Name</th>
              <th className="p-3">Email</th>
              <th className="p-3">Total Rounds</th>
              <th className="p-3">Last Interview</th>
              <th className="p-3">Last Interview Date</th>
              <th className="p-3">Status</th>
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

                <td className="p-3 text-gray-700">{c.totalRounds}</td>

                <td className="p-3 text-gray-700">
                  {c.lastInterviewType || "—"}
                </td>

                <td className="p-3 text-gray-700">
                  {c.lastInterviewDate
                    ? new Date(c.lastInterviewDate).toLocaleDateString()
                    : "—"}
                </td>

                <td className="p-3">
                  <span className="px-2 py-1 text-sm border border-gray-400 rounded text-gray-700">
                    {c.status}
                  </span>
                </td>

                <td className="p-3 space-x-2">
                  <button
                    onClick={() =>
                      navigate(`/History/${c.jobCandidateId}`)
                    }
                    className="px-3 py-1 text-sm border border-gray-400 rounded text-gray-700 hover:bg-gray-200"
                  >
                    Interview-History
                  </button>
                  {c.status === "Shortlisted" && (
                    <button
                      onClick={() => handleSelect(c.jobCandidateId)}
                      className="px-3 py-1 text-sm border border-gray-600 rounded text-gray-800 hover:bg-gray-300"
                    >
                      Select
                    </button>
                  )}
                  {c.status === "Selected" && (
                    <button
                      onClick={() => handleSendOffer(c.jobCandidateId)}
                      className="px-3 py-1 text-sm border border-gray-600 rounded text-gray-800 hover:bg-gray-300"
                    >
                      Send-Offer Letter
                    </button>
                  )}
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

export default FinalPool;
