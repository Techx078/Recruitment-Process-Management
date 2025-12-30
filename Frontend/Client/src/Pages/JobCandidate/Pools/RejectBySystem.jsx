import { toast } from "react-toastify";
import { rejectOfferBySystem } from "../../../Services/JobCandidateService";
import { handleGlobalError } from "../../../Services/errorHandler";
const RejectBySystem = ({
  rejectReason,
  setRejectReason,
  setCandidates,
  rejectingCandidateId,
  setRejectingCandidateId,
}) => {
  const handleConfirmReject = async () => {
    if (!rejectReason.trim()) {
      toast.warning("Rejection reason is required");
      return;
    }

    try {
      await rejectOfferBySystem(rejectingCandidateId, rejectReason);

      toast.success("Offer rejected successfully");

      setCandidates((prev) =>
        prev.map((c) =>
          c.jobCandidateId === rejectingCandidateId
            ? { ...c, status: "RejectedBySystem" }
            : c
        )
      );

      setRejectingCandidateId(null);
      setRejectReason("");
    } catch (err) {
      handleGlobalError(err);
    }
  };
  return (
    <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-md w-[400px]">
        <h3 className="text-lg font-semibold mb-3">Reject Offer</h3>

        <textarea
          placeholder="Enter rejection reason"
          value={rejectReason}
          onChange={(e) => setRejectReason(e.target.value)}
          className="w-full border border-gray-300 rounded p-2 mb-4"
        />

        <div className="flex justify-end gap-2">
          <button
            onClick={() => {
              setRejectingCandidateId(null);
              setRejectReason("");
            }}
            className="px-3 py-1 border border-gray-400 rounded"
          >
            Cancel
          </button>

          <button
            onClick={handleConfirmReject}
            className="px-3 py-1 bg-black text-white rounded"
          >
            Reject
          </button>
        </div>
      </div>
    </div>
  );
};

export default RejectBySystem;
