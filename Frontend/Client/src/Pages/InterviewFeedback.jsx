import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { submitInterviewFeedback } from "../Services/JobCandidateService";

const InterviewFeedback = () => {
  const { jobCandidateId } = useParams();
  const navigate = useNavigate();

  const [feedback, setFeedback] = useState("");
  const [marks, setMarks] = useState("");
  const [isPassed, setIsPassed] = useState(true);
  const [nextStep, setNextStep] = useState("Technical");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (!feedback || marks === "") {
      setError("Feedback and marks are required");
      return;
    }

    setLoading(true);

    try {
      const payload = {
        feedback,
        marks: Number(marks),
        isPassed,
        nextStep: isPassed ? nextStep : null,
      };

      const response = await submitInterviewFeedback(
        jobCandidateId,
        payload
      );

      setSuccess(response.message);

      setTimeout(() => {
        navigate(-1);
      }, 1500);
    } catch (err) {
     setError(err.data)
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto mt-10 px-4">
      <div className="bg-white border border-gray-300 rounded-lg shadow-sm p-6">
        <h2 className="text-2xl font-semibold text-black mb-6">
          Interview Feedback
        </h2>

        <form onSubmit={handleSubmit} className="space-y-5">
          {/* Feedback */}
          <div>
            <label className="block text-sm font-medium text-black mb-2">
              Feedback
            </label>
            <textarea
              rows={4}
              value={feedback}
              onChange={(e) => setFeedback(e.target.value)}
              className="w-full border border-gray-300 rounded-md p-3 text-sm text-black focus:outline-none focus:ring-1 focus:ring-black"
              placeholder="Write interview feedback..."
              required
            />
          </div>

          {/* Marks */}
          <div>
            <label className="block text-sm font-medium text-black mb-2">
              Marks
            </label>
            <input
              type="number"
              value={marks}
              onChange={(e) => setMarks(e.target.value)}
              className="w-full border border-gray-300 rounded-md p-2 text-sm text-black focus:outline-none focus:ring-1 focus:ring-black"
              placeholder="Enter marks"
              required
            />
          </div>

          {/* Pass / Fail */}
          <div>
            <label className="block text-sm font-medium text-black mb-2">
              Result
            </label>
            <select
              value={isPassed ? "Passed" : "Failed"}
              onChange={(e) => setIsPassed(e.target.value === "Passed")}
              className="w-full border border-gray-300 rounded-md p-2 text-sm text-black focus:outline-none focus:ring-1 focus:ring-black"
            >
              <option value="Passed">Passed</option>
              <option value="Failed">Failed</option>
            </select>
          </div>

          {/* Next Step (only if passed) */}
          {isPassed && (
            <div>
              <label className="block text-sm font-medium text-black mb-2">
                Next Step
              </label>
              <select
                value={nextStep}
                onChange={(e) => setNextStep(e.target.value)}
                className="w-full border border-gray-300 rounded-md p-2 text-sm text-black focus:outline-none focus:ring-1 focus:ring-black"
              >
                <option value="Technical">Next Technical Round</option>
                <option value="HR">HR Round</option>
                <option value="Finish">Finish (Shortlist)</option>
              </select>
            </div>
          )}

          {/* Error */}
          {error && (
            <div className="text-sm text-gray-700 border border-gray-300 bg-gray-100 p-3 rounded">
              {error}
            </div>
          )}

          {/* Success */}
          {success && (
            <div className="text-sm text-black border border-black bg-gray-100 p-3 rounded">
              {success}
            </div>
          )}

          {/* Actions */}
          <div className="flex justify-end gap-3 pt-4">
            <button
              type="button"
              onClick={() => navigate(-1)}
              className="px-4 py-2 text-sm border border-gray-400 text-gray-700 rounded-md hover:bg-gray-200 transition"
            >
              Cancel
            </button>

            <button
              type="submit"
              disabled={loading}
              className="px-4 py-2 text-sm border border-black bg-black text-white rounded-md hover:bg-gray-800 transition disabled:opacity-50"
            >
              {loading ? "Submitting..." : "Submit Feedback"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default InterviewFeedback;
