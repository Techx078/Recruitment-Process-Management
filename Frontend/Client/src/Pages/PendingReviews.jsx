import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getPendingReviewCandidate } from "../Services/JobCandidateService.js";
import { useAuthUserContext } from "../Context/AuthUserContext.jsx";
import { getJobOpeningById } from "../Services/JobOpeningService.js";

const PendingReviews = () => {
  const { jobOpeningId } = useParams();
  const navigate = useNavigate();

  const [candidates, setCandidates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [jobOpeningReviewer, serjobOpeningReviewer] = useState([]);
  const [error, setError] = useState("");
  const { authUser } = useAuthUserContext();
  useEffect(() => {
    fetchPendingReviews();
    fetchReviewerList();
  }, []);

  const fetchReviewerList = async () => {
    await getJobOpeningById(jobOpeningId, localStorage.getItem("token"))
      .then((response) => {
        serjobOpeningReviewer(response.reviewers);
      })
      .catch((error) => {
        console.error("Error fetching job opening reviewers:", error);
      });
  };

  const fetchPendingReviews = async () => {
    try {
      const response = await getPendingReviewCandidate(
        jobOpeningId,
        localStorage.getItem("token")
      );
      setCandidates(response);
    } catch (error) {
      if (!error.status) {
        setError("Network error. Please try again.");
        return;
      }
      const { status, data } = error;
      if (status === 400 && data.errors) {
        if (Array.isArray(data.errors)) {
          data.errors.forEach((msg) => alert(msg));
        }
        return;
      }
      setError(data.Message || "Something went wrong");
      return;
    } finally {
      setLoading(false);
    }
  };

  const openResume = (cvPath) => {
    window.open(cvPath, "_blank");
  };
  if (
    authUser.role === "Reviewer" &&
    !jobOpeningReviewer.some((i) => i.email === authUser.email)
  ) {
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        only assigned reviewer can access.
      </div>
    );
  }
  if (loading) return <p>Loading pending reviews...</p>;
  if (error)
    return (
      <p className="bg-white border border-gray-300 rounded-lg p-6 text-center text-gray-600">
        {error}
      </p>
    );

  return (
    <div className="max-w-7xl mx-auto mt-8 px-4">
      <h3 className="text-2xl font-semibold text-black mb-6">
        Pending Review Candidates
      </h3>

      {candidates.length === 0 ? (
        <div className="bg-white border border-gray-300 rounded-lg p-6 text-center text-gray-600">
          No pending candidates for pending review found.
        </div>
      ) : (
        <div className="overflow-x-auto bg-white border border-gray-300 rounded-lg shadow-sm">
          <table className="min-w-full border-collapse">
            <thead className="bg-gray-100 border-b border-gray-300">
              <tr>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Name
                </th>
                <th className="px-4 py-3 text-sm font-medium text-black">
                  Email
                </th>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Job Title
                </th>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Applied At
                </th>
                <th className="px-4 py-3  text-sm font-medium text-black">
                  Actions
                </th>
              </tr>
            </thead>

            <tbody>
              {candidates.map((c) => (
                <tr
                  key={c.jobCandidateId}
                  className="border-b border-gray-200 hover:bg-gray-50 transition"
                >
                  <td className="px-4 py-3 text-sm text-black">
                    {c.candidateName}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {c.candidateEmail}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {c.jobTitle}
                  </td>

                  <td className="px-4 py-3 text-sm text-gray-700">
                    {new Date(c.appliedAt).toLocaleDateString()}
                  </td>
                  <td className="px-4 py-3">
                    <div className="flex flex-wrap gap-2">
                      <button
                        onClick={() => openResume(c.cvPath)}
                        className="px-3 py-1.5 text-sm border border-gray-400 text-black rounded-md hover:bg-gray-100 transition"
                      >
                        View Resume
                      </button>

                      <button
                        onClick={() =>
                          window.open(
                            `/Candidate/Profile/${c.userId}`,
                            "_blank"
                          )
                        }
                        className="px-3 py-1.5 text-sm border border-gray-400 text-gray-700 rounded-md hover:bg-gray-200 transition"
                      >
                        View Profile
                      </button>
                      {authUser &&
                        authUser.role === "Reviewer" 
                         && (
                          <button
                            onClick={() =>
                              navigate(`/review/${c.jobCandidateId}`)
                            }
                            className="px-3 py-1.5 text-sm border border-black text-white bg-black rounded-md hover:bg-gray-800 transition"
                          >
                            Review & Feedback
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
    </div>
  );
};

export default PendingReviews;
