import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {scheduleInterview} from "../../../Services/JobCandidateService.js";
import { handleGlobalError } from "../../../Services/errorHandler.js";

const ScheduleInterview = () => {
  const { jobCandidateId } = useParams();
  const navigate = useNavigate();

  const [interviewDate, setInterviewDate] = useState("");
  const [meetingLink, setMeetingLink] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (!interviewDate || !meetingLink) {
      setError("All fields are required");
      return;
    }

    setLoading(true);

    try {
      const payload = {
        interviewDate,
        meetingLink,
      };

      const response = await scheduleInterview(jobCandidateId, payload);
      setSuccess(response.message);

      setTimeout(() => {
        navigate(-1);
      }, 1500);
    } catch (err) {
      handleGlobalError(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto mt-10 px-4">
      <div className="bg-white border border-gray-300 rounded-lg shadow-sm p-6">
        <h2 className="text-2xl font-semibold text-black mb-6">
          Schedule Interview
        </h2>

        <form onSubmit={handleSubmit} className="space-y-5">
          {/* Interview Date */}
          <div>
            <label className="block text-sm font-medium text-black mb-2">
              Interview Date & Time
            </label>
            <input
              type="datetime-local"
              value={interviewDate}
              onChange={(e) => setInterviewDate(e.target.value)}
              className="w-full border border-gray-300 rounded-md p-2 text-sm text-black focus:outline-none focus:ring-1 focus:ring-black"
              required
            />
          </div>

          {/* Meeting Link */}
          <div>
            <label className="block text-sm font-medium text-black mb-2">
              Meeting Link
            </label>
            <input
              type="url"
              value={meetingLink}
              onChange={(e) => setMeetingLink(e.target.value)}
              placeholder="https://meet.google.com/..."
              className="w-full border border-gray-300 rounded-md p-2 text-sm text-black focus:outline-none focus:ring-1 focus:ring-black"
              required
            />
          </div>

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
              {loading ? "Scheduling..." : "Schedule Interview"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ScheduleInterview;
