import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";

const InterviewerLevelProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if (
      authUser &&
      (authUser.role == "Interviewer" ||
        authUser.role == "Recruiter" ||
        authUser.role == "Admin")
    ) {
      setChecking(false);
    } else {
      alert("Access denied. Only Interviewer, Recruiter or Admin can access.");
      navigate("/login");
    }
  }, []);

  if (checking) {
    return null;
  }

  return children;
};

export default InterviewerLevelProtector;
