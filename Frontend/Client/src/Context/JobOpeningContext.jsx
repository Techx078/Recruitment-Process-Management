import axios from "axios";
import React, { createContext, useContext, useState, useEffect } from "react";
import { useAuthUserContext } from "./AuthUserContext";
import { getAllJobOpenings } from "../Services/JobOpeningService";

export const JobOpeningContext = createContext();

export default function JobOpeningContextProvider({ children }) {
 const [JobOpenings, setJobOpenings] = useState([]);
 let {authUser} = useAuthUserContext();

  const fetchJobOpenings =  async () => {
    const token = localStorage.getItem("token");
    const data = await getAllJobOpenings(token);
    setJobOpenings( ()=> data );
  }

  useEffect( () => {
    const fethcdata = async()=> { await fetchJobOpenings();}
    fethcdata();
  }, [authUser]);

  return (
    <JobOpeningContext.Provider value={{ JobOpenings , setJobOpenings }}>
      { children} 
    </JobOpeningContext.Provider>
  );
}

export const useJobOpeningContext = () => useContext(JobOpeningContext);