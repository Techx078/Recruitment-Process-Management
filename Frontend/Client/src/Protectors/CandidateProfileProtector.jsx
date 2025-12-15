import { useEffect, useState } from "react";
import { useNavigate , useParams } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";


const CandidateProfileProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const {UserId} = useParams();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if (authUser && ((authUser.role == "Candidate" && authUser.id == parseInt(UserId)) || authUser.role == "Recruiter" || authUser.role == "Admin" || authUser.role == "Interviewer" || authUser.role == "Reviewer") ) {
      setChecking(false);
    } else {
    alert("Access denied. Only Organizational user can access.");
      navigate("/login");
    }
  }, []);

  if (checking) {
    return null;
  }

  return children;
};

export default CandidateProfileProtector;
