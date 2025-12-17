import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { getJobOpeningById } from "../Services/JobOpeningService.js";
import { getJobCandidateById } from "../Services/JobCandidateService.js";
import { useParams } from "react-router-dom";
const InterviewerAssignedProtector = ({ children }) => {
  const navigate = useNavigate();
  const { jobCandidateId } = useParams();
  const { authUser } = useAuthUserContext();

  const [checking, setChecking] = useState(true);
  const [authorized, setAuthorized] = useState(false);

  useEffect(() => {
    const checkAccess = async () => {
      if (!authUser || authUser.role !== "Interviewer") {
        alert("Access denied. Only Interviewer can access.");
        navigate("/login");
        return;
      }

      try {
        const jobCandidate = await getJobCandidateById(jobCandidateId);

        const jobOpening = await getJobOpeningById(
          jobCandidate.jobOpeningId,
          localStorage.getItem("token")
        );

        const isAssigned = jobOpening.interviewers.some(
          (i) => i.email === authUser.email
        );

        setAuthorized(isAssigned);
      } catch (error) {
        console.error(error);
        setAuthorized(false);
      } finally {
        setChecking(false);
      }
    };

    checkAccess();
  }, [authUser, jobCandidateId, navigate]);

  if (checking) return null;

  if (!authorized) {
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        Only assigned interviewer can access.
      </div>
    );
  }

  return children;
};

export default InterviewerAssignedProtector;

