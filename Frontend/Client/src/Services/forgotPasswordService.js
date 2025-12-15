import axios from "axios";

const API_BASE = "http://localhost:5233/api/ForgotPassword"; 


export const sendOtp = async (email) => {
  try {
    await axios.post(
      `${API_BASE}/forgot-password`,
       email , 
      {
        headers: { "Content-Type": "application/json" },
      }
    );
    return true;
  } catch (error) {
     const message =
      error.response?.data?.message || "Something went wrong";

    throw new Error(message);
  }
};


export const resetPassword = async (payload) => {
  try {
    await axios.post(
      `${API_BASE}/reset-password`,
      payload,
      {
        headers: { "Content-Type": "application/json" },
      }
    );
    return true;
  } catch (error) {
     const message =
      error.response?.data?.message || "Something went wrong";

    throw new Error(message);
  }
};
