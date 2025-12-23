
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuthUserContext } from "../Context/AuthUserContext";
import { toast } from "react-toastify";

const CandidateProtector = ({ children }) => {
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  const {UserId} = useParams();
  const [checking, setChecking] = useState(true);

  useEffect(() => {
    if ( authUser && (authUser.role == "Candidate" && authUser.id == parseInt(UserId))) {
      setChecking(false);
    } else {
      toast.warning("Access denied. Only Candidates can access.");
      navigate("/login");
    }
  }, []);

  if (checking) {
    return null;
  }

  return children;
};

export default CandidateProtector;
