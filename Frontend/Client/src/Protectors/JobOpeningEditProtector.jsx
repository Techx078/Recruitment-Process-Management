import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { getJobOpeningById } from "../Services/JobOpeningService";

const JobOpeningEditProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const { id } = useParams();

  const [checking, setChecking] = useState(true);
  const [jobNotFound, setJobNotFound] = useState(false);

  useEffect(() => {
    const checkAccess = async () => {
      try {
        if (!authUser) {
          navigate("/login");
          return;
        }

        const token = localStorage.getItem("token");
        const job = await getJobOpeningById(id, token);

        // Job not found
        if (!job) {
          setJobNotFound(true);
          setChecking(false);
          return;
        }

        const role = authUser.role;
        const loggedInUserId = authUser.userId;
        const recruiterUserId = job?.Recruiter?.UserId;

        // Admin → full access
        if (role === "Admin") {
          setChecking(false);
          return;
        }

        // Recruiter → only creator
        if (
          role === "Recruiter" &&
          recruiterUserId &&
          loggedInUserId === recruiterUserId
        ) {
          setChecking(false);
          return;
        }

        // Everyone else → denied
        navigate("/login");
      } catch (error) {
        console.error(error);
        navigate("/login");
      }
    };

    checkAccess();
  }, [authUser, id, navigate]);

  if (checking) return null;

  if (jobNotFound) {
    return (
      <div className="flex flex-col items-center justify-center h-64">
        <h1 className="text-2xl font-semibold text-gray-700">
          Job Opening Not Found
        </h1>
        <p className="text-gray-500 mt-2">
          The job you are trying to access does not exist or was removed.
        </p>
      </div>
    );
  }

  return children;
};

export default JobOpeningEditProtector;
