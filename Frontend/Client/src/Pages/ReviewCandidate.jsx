import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { updateReviewStatus } from "../Services/JobCandidateService.js";
const ReviewCandidate = () => {
  const { jobCandidateId } = useParams();
  const navigate = useNavigate();

  const [comment, setComment] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const submitReview = async (isApproved) => {
    setError("");
    setSuccess("");
    setLoading(true);

    try {
      const payload = {
        isApproved,
        reviewerComment: comment,
      };

      const response = await updateReviewStatus(jobCandidateId, payload);

      setSuccess(response.message);

      // redirect after success
      setTimeout(() => {
        navigate(-1); // go back to pending reviews
      }, 1500);
    } catch (error) {
        console.log(error);
        
      if (!error.status) {
        setError("Network error. Please try again.");
        return;
      }
      const { status, data } = error;
      if (status === 400 && data.errors) {
        if (Array.isArray(data.errors)) {
          data.errors.forEach((msg) => setError(msg));
        }
        return;
      } 
      setError(data.Message || "Something went wrong");
      return;
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto mt-10 px-4">
      <div className="bg-white border border-gray-300 rounded-lg shadow-sm p-6">
        <h2 className="text-2xl font-semibold text-black mb-4">
          Review Candidate
        </h2>

        <p className="text-sm text-gray-600 mb-6">
          Job Candidate ID: <span className="text-black">{jobCandidateId}</span>
        </p>

        {/* Comment Box */}
        <div className="mb-6">
          <label className="block text-sm font-medium text-black mb-2">
            Reviewer Comment
          </label>
          <textarea
            rows={5}
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Write your feedback here..."
            className="w-full border border-gray-300 rounded-md p-3 text-sm text-black focus:outline-none focus:ring-1 focus:ring-black"
          />
        </div>

        {/* Messages */}
        {error && (
          <div className="mb-4 text-sm text-gray-700 border border-gray-300 bg-gray-100 p-3 rounded">
            {error}
          </div>
        )}

        {success && (
          <div className="mb-4 text-sm text-black border border-black bg-gray-100 p-3 rounded">
            {success}
          </div>
        )}

        {/* Actions */}
        <div className="flex justify-end gap-3">
          <button
            disabled={loading}
            onClick={() => submitReview(false)}
            className="px-4 py-2 text-sm border border-gray-400 text-gray-700 rounded-md hover:bg-gray-200 transition disabled:opacity-50"
          >
            Reject
          </button>

          <button
            disabled={loading}
            onClick={() => submitReview(true)}
            className="px-4 py-2 text-sm border border-black bg-black text-white rounded-md hover:bg-gray-800 transition disabled:opacity-50"
          >
            Approve
          </button>
        </div>
      </div>
    </div>
  );
};

export default ReviewCandidate;
