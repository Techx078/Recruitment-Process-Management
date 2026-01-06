import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getCandidateDocuments,
  verifyCandidateDocuments,
} from "../../../Services/JobCandidateService";
import { toast } from "react-toastify";
import { handleGlobalError } from "../../../Services/errorHandler";

const VerifyCandidateDocuments = () => {
  const { jobCandidateId } = useParams();
  const navigate = useNavigate();

  const [documents, setDocuments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [rejectReason, setRejectReason] = useState("");
  const [showRejectBox, setShowRejectBox] = useState(false);

  useEffect(() => {
    fetchDocuments();
  }, []);

  const fetchDocuments = async () => {
    try {
      setLoading(true);
      const res = await getCandidateDocuments(jobCandidateId);
      setDocuments(res.documents);
    } catch (err) {
      handleGlobalError(err)
    } finally {
      setLoading(false);
    }
  };

  const handleVerify = async () => {
    try {
      await verifyCandidateDocuments(jobCandidateId, true);
      toast.success("Documents verified successfully");
      navigate(-1);
    } catch (err) {
      handleGlobalError(err)
    }
  };

  const handleReject = async () => {
    if (!rejectReason.trim()) {
      toast.error("Rejection reason is required");
      return;
    }

    try {
      await verifyCandidateDocuments(
        jobCandidateId,
        false,
        rejectReason
      );
      toast.success("Documents rejected");
      navigate(-1);
    } catch (err) {
      handleGlobalError(err)
    }
  };

  return (
    <div className="max-w-6xl mx-auto mt-10 px-4">
      <h2 className="text-xl font-semibold text-black mb-4">
        Verify Candidate Documents
      </h2>

      {loading ? (
        <p className="text-gray-600">Loading documents...</p>
      ) : documents?.length === 0 ? (
        <p className="text-gray-600">No documents uploaded yet.</p>
      ) : (
        <table className="w-full border-collapse">
          <thead className="bg-gray-100 border-b border-gray-300">
            <tr className="text text-gray-700">
              <th className="p-3">Document</th>
              <th className="p-3">Description</th>
              <th className="p-3">Uploaded At</th>
              <th className="p-3">View</th>
            </tr>
          </thead>

          <tbody>
            {documents?.map((d) => (
              <tr
                key={d?.jobCandidateDocumentId}
                className="border-b border-gray-200 hover:bg-gray-50"
              >
                <td className="p-3 text-gray-800">{d?.documentName}</td>
                <td className="p-3 text-gray-700">
                  {d?.documentDescription || "—"}
                </td>
                <td className="p-3 text-gray-700">
                  {new Date(d?.uploadedAt).toLocaleDateString()}
                </td>
                <td className="p-3">
                  <button
                    onClick={() =>
                      window.open(d?.documentUrl, "_blank")
                    }
                    className="px-3 py-1.5 text-sm border border-gray-400 text-gray-700 rounded hover:bg-gray-200"
                  >
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* ACTIONS */}
      {!loading && documents?.length > 0 && (
        <div className="flex gap-3 mt-6">
          <button
            onClick={handleVerify}
            className="px-4 py-2 border border-black bg-black text-white rounded hover:bg-gray-800"
          >
            Verify Documents
          </button>

          <button
            onClick={() => setShowRejectBox(true)}
            className="px-4 py-2 border border-gray-400 text-gray-700 rounded hover:bg-gray-200"
          >
            Reject Documents
          </button>
        </div>
      )}

      {/* REJECT MODAL */}
      {showRejectBox && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-md w-full max-w-md p-5 space-y-4">
            <h3 className="text-lg font-semibold">
              Reject Documents
            </h3>

            <textarea
              value={rejectReason}
              onChange={(e) => setRejectReason(e.target.value)}
              rows={3}
              className="w-full border border-gray-300 rounded p-2"
              placeholder="Enter rejection reason"
            />

            <div className="flex justify-end gap-2">
              <button
                onClick={() => {
                  setShowRejectBox(false);
                  setRejectReason("");
                }}
                className="px-4 py-2 border border-gray-400 rounded"
              >
                Cancel
              </button>

              <button
                onClick={handleReject}
                className="px-4 py-2 border border-black bg-black text-white rounded"
              >
                Reject
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default VerifyCandidateDocuments;
