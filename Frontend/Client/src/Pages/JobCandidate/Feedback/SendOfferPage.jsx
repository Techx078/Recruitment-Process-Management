import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { sendOffer } from "../../../Services/JobCandidateService";
import { toast } from "react-toastify";
import { handleGlobalError } from "../../../Services/errorHandler";

const SendOfferPage = () => {
  const { jobCandidateId } = useParams();
  const navigate = useNavigate();

  const [expiryDate, setExpiryDate] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, SetError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!expiryDate) {
      toast.error("Please select offer expiry date");
      return;
    }

    try {
      setLoading(true);
      await sendOffer(jobCandidateId, expiryDate);

      toast.success("Offer sent successfully");
      navigate(-1);
    } catch (error) {
      handleGlobalError(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-lg mx-auto mt-10 bg-white border border-gray-300 rounded shadow-sm">
      {/* Header */}
      <div className="p-4 border-b border-gray-300">
        <h2 className="text-lg font-semibold text-gray-800">Send Offer</h2>
        <p className="text-sm text-gray-600">
          Select offer expiry date for the candidate
        </p>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit} className="p-4 space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Offer Expiry Date
          </label>
          <input
            type="date"
            value={expiryDate}
            min={new Date().toISOString().split("T")[0]}
            onChange={(e) => setExpiryDate(e.target.value)}
            className="w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-1 focus:ring-gray-400"
            required
          />
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
          <button
            type="button"
            onClick={() => navigate(-1)}
            className="px-4 py-2 text-sm border border-gray-400 rounded text-gray-700 hover:bg-gray-200"
          >
            Cancel
          </button>

          <button
            type="submit"
            disabled={loading}
            className="px-4 py-2 text-sm border border-gray-600 rounded text-gray-800 hover:bg-gray-300 disabled:opacity-50"
          >
            {loading ? "Sending..." : "Send Offer"}
          </button>
        </div>
      </form>
      {error && (
        <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
          {error}
        </div>
      )}
    </div>
  );
};

export default SendOfferPage;
