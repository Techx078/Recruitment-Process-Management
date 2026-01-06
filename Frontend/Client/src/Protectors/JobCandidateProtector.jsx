import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { handleGlobalError } from "../Services/errorHandler";
import { validateCandidate } from "../Services/protectorServices";
import { getJobCandidateById } from "../Services/JobCandidateService";

const JobCandidateProtector = ({ children }) => {
  const { jobCandidateId } = useParams();
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();

  const [checking, setChecking] = useState(true);
  const [authorized, setAuthorized] = useState(false);

  useEffect(() => {
    const checkAccess = async () => {
      if (!authUser) {
        navigate("/login");
        return;
      }
      else if( authUser.role == "Candidate" ){
    try {
        const res = await getJobCandidateById( jobCandidateId );
        if( res.status == "OfferAccepted" || res.status == "DocumentRejected" ){
        await validateCandidate(res.candidateId);
        setAuthorized(true);
        }
      } catch (err) {
        console.log(err);
        handleGlobalError(err)
        setAuthorized(false);
      } finally {
        setChecking(false);
      }
    }else{
        setChecking(false);
        setAuthorized(false);
    }
    };

    checkAccess();
  }, [authUser, jobCandidateId, navigate]);

  if (checking) return null;

  if (!authorized) {
    return (
      <div className="max-w-6xl mx-auto mt-10 border p-4 rounded bg-gray-100">
        Access denied
      </div>
    );
  }

  return children;
};

export default JobCandidateProtector;
