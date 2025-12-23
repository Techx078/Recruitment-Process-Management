import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { toast } from "react-toastify";


const OrganizationUserProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if ( !authUser || (authUser.role == "Interviewer" && authUser.role == "Recruiter" && authUser.role == "Reviewer" && authUser.role == "Admin" )) {
      toast.warning("Access denied. Only Organizational user can access.");
      navigate("/login");
    } else {
      setChecking(false);
    }
  }, []);

  if (checking) {
    return null;
  }

  return children;
};

export default OrganizationUserProtector;
