import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { handleGlobalError } from "../Services/errorHandler";
import { validateHrLevelAccess } from "../Services/protectorServices";
import { getJobCandidateById } from "../Services/JobCandidateService";

const HrAssinedProtector = ({ children }) => {
  let { jobOpeningId , jobCandidateId } = useParams();
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
      else if( authUser.role == "Interviewer" ){
      try {
        let jobCandidate;
        if( jobCandidateId ){
          jobCandidate = await getJobCandidateById(jobCandidateId);
          jobOpeningId = jobCandidate.jobOpeningId;
        }
         await validateHrLevelAccess(
          jobOpeningId,
          localStorage.getItem("token")
        );
        setAuthorized(true);
      } catch (err) {
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
  }, [authUser, jobOpeningId, navigate]);

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

export default HrAssinedProtector;
