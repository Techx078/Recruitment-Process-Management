import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { toast } from "react-toastify";


const authUserProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if ( !authUser) {
      toast.warning("Access denied. Please log in.");
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

export default authUserProtector;
