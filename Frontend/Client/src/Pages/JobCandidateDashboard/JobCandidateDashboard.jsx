import { useState } from "react";
import TechnicalInterviewPool from "../TechnicalInterviewPool";
import PendingReviews from "../PendingReviews";
import HrPool from "../HrPool";
import FinalPool from "../FinalPool";
import PoolTabs from "./PoolTabs"

export default function JobCandidateDashboard() {
  const POOLS = {
  TECHNICAL: "technical",
  REVIEW: "review",
  HR: "hr",
  FINAL: "final",
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
