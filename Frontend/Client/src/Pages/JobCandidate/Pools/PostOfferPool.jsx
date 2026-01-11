import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  getPostOfferPool,
  sendJoiningDate,
  addCandidateToEmployee
} from "../../../Services/JobCandidateService";
import { handleGlobalError } from "../../../Services/errorHandler";
import { toast } from "react-toastify";

const STATUS_ORDER = [
  "OfferAccepted",
  "OfferRejectedByCandidate",
  "OfferRejectedBySystem",
  "DocumentUploaded",
  "DocumentsVerified",
  "DocumentRejected",
  "JoiningDateSend",
  "Employee"
];

const PostOfferPool = () => {
  const { jobOpeningId } = useParams();
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [joiningcandidateId, setjoiningcandidateId] = useState(null);
  const [joiningDate, setjoiningDate] = useState(null);
  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const data = await getPostOfferPool(jobOpeningId);
        setCandidates(data);
      } catch (err) {
        setError(err?.data?.message || "Failed to load candidates");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [jobOpeningId]);

  if (loading) return <p className="text-center mt-10">Loading...</p>;

  if (error)
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        {error}
      </div>
    );

  return (
    <div className="max-w-7xl mx-auto mt-10 px-4 space-y-10">
      <h2 className="text-xl font-semibold text-black">
        Post-Offer Candidate Pool
      </h2>

      {STATUS_ORDER.map((status) => {
        const filtered = candidates.filter((c) => c.status === status);

        if (filtered.length === 0) return null;

        return (
          <div key={status}>
            <h3 className="text-lg font-semibold text-gray-800 mb-3">
              {status}
            </h3>

            <table className="w-full bg-white rounded-lg shadow-sm">
              <thead className="bg-gray-100 border-b border-gray-300">
                <tr className="text text-gray-700">
                  <th className="p-3">Name</th>
                  <th className="p-3">Email</th>
                  <th className="p-3">Status</th>
                  <th className="p-3">Actions</th>
                </tr>
              </thead>

              <tbody>
                {filtered.map((c) => (
                  <tr
                    key={c.jobCandidateId}
                    className="border-b border-gray-200 hover:bg-gray-50"
                  >
                    <td className="p-3 text-gray-800">{c.candidateName}</td>

                    <td className="p-3 text-gray-700">{c.email}</td>

                    <td className="p-3">
                      <span className="px-2 py-1 text-sm border border-gray-400 rounded text-gray-700">
                        {c.status}
                      </span>
                    </td>
                    {c.status === "DocumentsVerified" && (
                      <td>
                        <button
                          onClick={() => setCandidateId(c.jobCandidateId)}
                          className="px-2 py-1 text-sm border border-gray-400 rounded text-gray-700"
                        >
                          Send Joining Date
                        </button>
                      </td>
                    )}
                    {c.status === "JoiningDateSend" && (
                      <td>
                        <button
                          onClick={async () => {
                            try {
                              await addCandidateToEmployee(c.jobCandidateId);
                              toast.success("Candidate added to employee list");
                            } catch (err) {
                              handleGlobalError(err);
                            }
                          }}
                          className="px-2 py-1 text-sm border border-gray-400 rounded text-gray-700"
                        >
                          Add to employee
                        </button>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
            {/* send joining date */}
            {joiningcandidateId && (
              <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
                <div className="bg-white p-6 rounded-md w-[400px]">
                  <h4 className="text-md font-semibold mb-2">
                    Send Joining Date
                  </h4>
                  <input
                    type="date"
                    className="w-full border border-gray-300 rounded p-2 mb-4"
                    onChange={(e) => setjoiningDate(e.target.value)}
                  />
                  <div className="flex justify-end gap-2">
                    <button
                      onClick={() => {
                        setjoiningcandidateId(null);
                        setjoiningDate("");
                      }}
                      className="px-3 py-1 border border-gray-400 rounded"
                    >
                      Cancel
                    </button>

                    <button
                      onClick={async () => {
                        try {
                          await sendJoiningDate(
                            joiningcandidateId,
                            joiningDate
                          );
                          toast.success("Joining date sent successfully");
                          setjoiningcandidateId(null);
                        } catch (err) {
                          handleGlobalError(err);
                        }
                      }}
                      className="px-3 py-1 bg-black text-white rounded"
                    >
                      Send Joining Date
                    </button>
                  </div>
                </div>
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
};

export default PostOfferPool;
