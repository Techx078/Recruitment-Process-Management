import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getSentOfferPool } from "../../../Services/JobCandidateService";
import RejectBySystem from "./RejectBySystem";
import { handleGlobalError } from "../../../Services/errorHandler";
import ExtendExpireDate from "./ExtendExpireDate";

const SentOfferPoolPage = () => {
  const { jobOpeningId } = useParams();
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [rejectingCandidateId, setRejectingCandidateId] = useState(null);
  const [rejectReason, setRejectReason] = useState("");
  const [extendingCandidateId, setExtendingCandidateId] = useState(null);
  const [newExpiryDate, setNewExpiryDate] = useState("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const data = await getSentOfferPool(jobOpeningId);
        setCandidates(data);
      } catch (err) {
        handleGlobalError(err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [jobOpeningId]);

  const isExpired = (expiryDate) => {
    if (!expiryDate) return false;
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const expiry = new Date(expiryDate);
    expiry.setHours(0, 0, 0, 0);

    return expiry < today;
  };

  if (loading) {
    return (
      <div className="max-w-7xl mx-auto mt-10 px-4 text-gray-600">
        Loading sent offer pool...
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto mt-10 px-4">
      <h2 className="text-xl font-semibold text-black mb-4">Sent Offer Pool</h2>

      {candidates.length === 0 ? (
        <div className="p-4 text-gray-600">No candidates with sent offers.</div>
      ) : (
        <table className="w-full border-collapse">
          <thead className="bg-gray-100 border-b border-gray-300">
            <tr SentofferclassName=" text-gray-700">
              <th className="p-3">Name</th>
              <th className="p-3">Email</th>
              <th className="p-3">Offer Expiry</th>
              <th className="p-3">Status</th>
              <th className="p-3">Actions</th>
            </tr>
          </thead>

          <tbody>
            {candidates.map((c) => {
              const expired = isExpired(c.offerExpiryDate);

              return (
                <tr
                  key={c.jobCandidateId}
                  className="border-b border-gray-200 hover:bg-gray-50"
                >
                  <td className="p-3 text-gray-800">{c.candidateName}</td>

                  <td className="p-3 text-gray-700">{c.email}</td>

                  <td className="p-3 text-gray-700">
                    {expired ? (
                      <span className="text-red-600 font-medium">Expired</span>
                    ) : (
                      new Date(c.offerExpiryDate).toLocaleDateString()
                    )}
                  </td>

                  <td className="p-3">
                    <span className="px-2 py-1 text-sm border border-gray-400 rounded text-gray-700">
                      {c.status}
                    </span>
                  </td>

                  <td className="p-3">
                    <div className="flex items-center gap-2">
                      <button
                        onClick={() =>
                          window.open(
                            `/candidate/Profile/${c.userId}`,
                            "_blank"
                          )
                        }
                        className="px-3 py-1.5 text-sm border border-gray-400 text-gray-700 rounded-md hover:bg-gray-200 transition"
                      >
                        View Profile
                      </button>
                      {rejectingCandidateId && (
                        <RejectBySystem
                          rejectReason={rejectReason}
                          setRejectReason={setRejectReason}
                          setCandidates={setCandidates}
                          rejectingCandidateId={rejectingCandidateId}
                          setRejectingCandidateId={setRejectingCandidateId}
                        />
                      )}
                      {extendingCandidateId && (
                        <ExtendExpireDate
                          extendingCandidateId={extendingCandidateId}
                          newExpiryDate={newExpiryDate}
                          setCandidates={setCandidates}
                          setExtendingCandidateId={setExtendingCandidateId}
                          setNewExpiryDate={setNewExpiryDate}
                        />
                      )}

                      {expired && (
                        <>
                          <button
                            onClick={() =>
                              setRejectingCandidateId(c.jobCandidateId)
                            }
                            className="px-3 py-1.5 text-sm border border-gray-400 text-gray-700 rounded-md hover:bg-gray-200 transition"
                          >
                            Reject
                          </button>

                          <button
                            onClick={() =>
                              setExtendingCandidateId(c.jobCandidateId)
                            }
                            className="px-3 py-1.5 text-sm border border-black bg-black text-white rounded-md hover:bg-gray-800 transition"
                          >
                            Extend
                          </button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default SentOfferPoolPage;
