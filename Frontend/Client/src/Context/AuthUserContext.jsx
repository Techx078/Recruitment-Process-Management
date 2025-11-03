import axios from "axios";
import React, { createContext, useContext, useState, useEffect } from "react";

export const AuthUserContext = createContext();

export default function AuthUserContextProvider({ children }) {
  const [authUser, setAuthUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      axios
        .get("http://localhost:5233/api/Auth/profile", {
          headers: { Authorization: `Bearer ${token}` },
        })
        .then((response) => {
          setAuthUser(response.data); 
        })
        .catch((error) => {
          setAuthUser(null);
        })
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, []);

  return (
    <AuthUserContext.Provider value={{ authUser, setAuthUser }}>
      {!loading && children} 
    </AuthUserContext.Provider>
  );
}

export const useAuthUserContext = () => useContext(AuthUserContext);