import { toast } from "react-toastify";
import { handleGlobalError } from "../../../Services/errorHandler";
import { extendOfferExpiry } from "../../../Services/JobCandidateService";
const ExtendExpireDate = ({
  extendingCandidateId,
  newExpiryDate,
  setCandidates,
  setExtendingCandidateId,
  setNewExpiryDate
}) => {

const handleConfirmExtend = async () => {
  if (!newExpiryDate) {
    toast.error("Please select a new expiry date");
    return;
  }

  try {
    const res = await extendOfferExpiry(
      extendingCandidateId,
      newExpiryDate
    );
    
    toast.success("Offer expiry extended");

    setCandidates((prev) =>
      prev.map(c =>
        c.jobCandidateId === extendingCandidateId
          ? { ...c, offerExpiryDate: res.data.newExpiryDate }
          : c
      )
    );

    setExtendingCandidateId(null);
    setNewExpiryDate("");
  } catch (err) {
    
    handleGlobalError(err)
  }
};

  return (
     <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
    <div className="bg-white p-6 rounded-md w-[400px]">
      <h3 className="text-lg font-semibold mb-3">
        Extend Offer Expiry
      </h3>

      <input
        type="date"
        value={newExpiryDate}
        onChange={(e) => setNewExpiryDate(e.target.value)}
        className="w-full border border-gray-300 rounded p-2 mb-4"
      />

      <div className="flex justify-end gap-2">
        <button
          onClick={() => {
            setExtendingCandidateId(null);
            setNewExpiryDate("");
          }}
          className="px-3 py-1 border border-gray-400 rounded"
        >
          Cancel
        </button>

        <button
          onClick={handleConfirmExtend}
          className="px-3 py-1 bg-black text-white rounded"
        >
          Extend
        </button>
      </div>
    </div>
    </div>
  );
};

export default ExtendExpireDate;
