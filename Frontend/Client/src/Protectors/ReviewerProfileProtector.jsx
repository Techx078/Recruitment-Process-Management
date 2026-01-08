import { useEffect, useState  } from "react";
import { useNavigate , useParams } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { toast } from "react-toastify";


const ReviewerProfileProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const {UserId} = useParams();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if (authUser && ((authUser.role == "Reviewer" && authUser.id == parseInt(UserId)) || authUser.role == "Recruiter" || authUser.role == "Admin") ) {
      setChecking(false);
    } else {
    toast.warning("Access denied. Only Organizational user can access.");
      navigate("/login");
    }
  }, []);

  if (checking) {
    return null;
  }

  return children;
};

export default ReviewerProfileProtector;
