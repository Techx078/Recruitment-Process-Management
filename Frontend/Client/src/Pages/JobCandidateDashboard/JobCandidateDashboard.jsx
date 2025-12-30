import { useState } from "react";
import TechnicalInterviewPool from "../JobCandidate/Pools/TechnicalInterviewPool";
import PendingReviews from "../JobCandidate/Pools/PendingReviews";
import HrPool from "../JobCandidate/Pools/HrPool";
import FinalPool from "../JobCandidate/Pools/FinalPool";
import PoolTabs from "./PoolTabs"
import SentOfferPoolPage from "../JobCandidate/Pools/SentOfferPoolPage"
export default function JobCandidateDashboard() {
  const POOLS = {
  TECHNICAL: "technical",
  REVIEW: "review",
  HR: "hr",
  FINAL: "final",
  OFFER:"Sentoffer"
};
  const [activePool, setActivePool] = useState(POOLS.TECHNICAL);
 

  const renderPool = () => {
    switch (activePool) {
      case POOLS.TECHNICAL:
        return <TechnicalInterviewPool />;
      case POOLS.REVIEW:
        return <PendingReviews />;
      case POOLS.HR:
        return <HrPool />;
      case POOLS.FINAL:
        return <FinalPool />;
      case POOLS.OFFER:
        return <SentOfferPoolPage />;
      default:
        return null;
    }
  };

  return (
    <div className="p-6">
      <PoolTabs activePool={activePool} setActivePool={setActivePool} />
      <div className="mt-6">{renderPool()}</div>
    </div>
  );
}
