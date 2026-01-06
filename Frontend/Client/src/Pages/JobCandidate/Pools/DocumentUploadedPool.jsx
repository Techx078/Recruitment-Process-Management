import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getDocumentUploadedPool } from "../../../Services/JobCandidateService";
import { handleGlobalError } from "../../../Services/errorHandler";
import { useAuthUserContext } from "../../../Context/AuthUserContext";

const DocumentUploadedPool = () => {
  const { jobOpeningId } = useParams();
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();

  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchCandidates = async () => {
      try {
        setLoading(true);
        const data = await getDocumentUploadedPool(jobOpeningId);
        setCandidates(data);
      } catch (err) {
        handleGlobalError(err)
        setError(
          err?.data?.message || "Failed to load document uploaded candidates"
        );
      } finally {
        setLoading(false);
      }
    };

    fetchCandidates();
  }, [jobOpeningId]);

  if (loading) return <p className="text-center mt-10">Loading...</p>;

  if (error)
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        {error}
      </div>
    );

  return (
    <div className="max-w-7xl mx-auto mt-10 px-4">
      <h2 className="text-xl font-semibold text-black mb-4">
        Document Uploaded Candidates
      </h2>

      {candidates?.length === 0 ? (
        <div className="p-4 text-gray-600">
          No candidates have uploaded documents yet.
        </div>
      ) : (
        <table className="w-full border-collapse">
          <thead className="bg-gray-100 border-b border-gray-300">
            <tr className="text text-gray-700">
              <th className="p-3">Name</th>
              <th className="p-3">Email</th>
              <th className="p-3">Status</th>
              <th className="p-3">Actions</th>
            </tr>
          </thead>

          <tbody>
            {candidates?.map((c) => (
              <tr
                key={c?.jobCandidateId}
                className="border-b border-gray-200 hover:bg-gray-50"
              >
                <td className="p-3 text-gray-800">
                  {c?.candidateName}
                </td>

                <td className="p-3 text-gray-700">{c?.email}</td>

                <td className="p-3">
                  <span className="px-2 py-1 text-sm border border-gray-400 rounded text-gray-700">
                    {c?.status}
                  </span>
                </td>

                <td className="p-3">
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
                          navigate(
                            `/verify-documents/${c?.jobCandidateId}`
                          )
                        }
                        className="px-3 py-1.5 text-sm border border-black bg-black text-white rounded-md hover:bg-gray-800 transition"
                      >
                        Verify Documents
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default DocumentUploadedPool;
