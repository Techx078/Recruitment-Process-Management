import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getPostOfferPool } from "../../../Services/JobCandidateService";

const STATUS_ORDER = [
  "OfferAccepted",
  "OfferRejectedByCandidate",
  "OfferRejectedBySystem",
  "DocumentUploaded",
  "DocumentsVerified",
  "DocumentRejected",
];

const PostOfferPool = () => {
  const { jobOpeningId } = useParams();
  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

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
                </tr>
              </thead>

              <tbody>
                {filtered.map((c) => (
                  <tr
                    key={c.jobCandidateId}
                    className="border-b border-gray-200 hover:bg-gray-50"
                  >
                    <td className="p-3 text-gray-800">
                      {c.candidateName}
                    </td>

                    <td className="p-3 text-gray-700">{c.email}</td>

                    <td className="p-3">
                      <span className="px-2 py-1 text-sm border border-gray-400 rounded text-gray-700">
                        {c.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        );
      })}
    </div>
  );
};

export default PostOfferPool;
