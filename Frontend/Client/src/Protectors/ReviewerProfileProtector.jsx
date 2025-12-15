import { useEffect, useState  } from "react";
import { useNavigate , useParams } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";


const ReviewerProfileProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const {UserId} = useParams();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    console.log(authUser.id);
    
    if (authUser && ((authUser.role == "Reviewer" && authUser.id == parseInt(UserId)) || authUser.role == "Recruiter" || authUser.role == "Admin") ) {
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

export default ReviewerProfileProtector;
