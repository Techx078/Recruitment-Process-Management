import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "react-toastify";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { handleGlobalError } from "../Services/errorHandler";
import { getJobOpeningById } from "../Services/JobOpeningService";
import { getJobCandidateById } from "../Services/JobCandidateService";

const RecruiterLevelProtector = ({ children }) => {
  const { jobCandidateId, jobOpeningId } = useParams();
  let jobOpeningIdTemp = jobOpeningId;
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();

  const [checking, setChecking] = useState(true);
  const [authorized, setAuthorized] = useState(false);

  useEffect(() => {
    const checkAccess = async () => {
      if (!authUser) {
        toast.warning("Access denied.");
        navigate("/login");
        return;
      }
      try {
        if (jobCandidateId) {
          const jobCandidate = await getJobCandidateById(jobCandidateId);
          jobOpeningIdTemp = jobCandidate?.jobOpeningId;
        }
        let jobOpening = await getJobOpeningById(
          jobOpeningIdTemp,
          localStorage.getItem("token")
        );
        if (
          authUser &&
          ((authUser.role == "Recruiter" &&
            jobOpening.recruiter.userId == authUser.id) ||
            authUser.role == "Admin")
        ) {
          setChecking(false);
        } else {
          toast.warning("Access denied. Only Recruiter and admin can access");
          navigate("/login");
        }
        setAuthorized(true);
      } catch (e) {
        console.log(e);
        handleGlobalError(e);
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

export default RecruiterLevelProtector;
