import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { toast } from "react-toastify";


const ReviewerProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if ( !authUser || authUser.role !== "Reviewer") {
      toast.warning("Access denied. Only Reviewer can access.");
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

export default ReviewerProtector;
