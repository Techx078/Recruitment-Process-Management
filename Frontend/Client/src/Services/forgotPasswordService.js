import apiClient from "./apiClient";
const CONTROLLER = "/ForgotPassword";

export const sendOtp = async (email) => {
  return apiClient.post(`${CONTROLLER}/forgot-password`, { email }); 
};

export const resetPassword = async (payload) => {
  return apiClient.post(`${CONTROLLER}/reset-password`, payload);
};
