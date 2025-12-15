import { useEffect, useState  } from "react";
import { useNavigate  , useParams} from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";


const RecruiterProfileProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const {UserId} = useParams();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if (authUser && ((authUser.role == "Recruiter" && authUser.id == parseInt(UserId)) || authUser.role == "Admin") ) {
      setChecking(false);
    } else {
    alert("Access denied");
      navigate("/login");
    }
  }, []);

  if (checking) {
    return null;
  }

  return children;
};

export default RecruiterProfileProtector;
