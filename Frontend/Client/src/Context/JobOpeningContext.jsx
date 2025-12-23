import axios from "axios";
import React, { createContext, useContext, useState, useEffect ,  } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthUserContext } from "./AuthUserContext";
import { getAllJobOpenings } from "../Services/JobOpeningService";
import { Navigate } from "react-router-dom";
import { handleGlobalError } from "../Services/errorHandler";

export const JobOpeningContext = createContext();

export default function JobOpeningContextProvider({ children }) {
 const [JobOpenings, setJobOpenings] = useState([]);
 let {authUser} = useAuthUserContext();
  const navigate =  useNavigate();
 let [loading , setLoading] = useState(false);
  const fetchJobOpenings =  async () => {
   
    try {
    const token = localStorage.getItem("token");
    const data = await getAllJobOpenings(token);
    setJobOpenings(() => data);
    } catch (error) { 
      if (authUser == null) {
        alert("Login to continue");
        navigate("/login");
      }
     handleGlobalError(error)
    }
  }

  useEffect( () => {
    const fethcdata = async()=> { 
      setLoading(true);
      await fetchJobOpenings();
      setLoading(false);
    }
    fethcdata();
  }, [authUser]);
  
  if(loading){
    return <div>Loading...</div>;
  }
  return (
    <JobOpeningContext.Provider value={{ JobOpenings , setJobOpenings }}>
      { children} 
    </JobOpeningContext.Provider>
  );
}

export const useJobOpeningContext = () => useContext(JobOpeningContext);