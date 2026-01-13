import apiClient from "./apiClient";
const CONTROLLER = "/Interviewer";

export const fetchInterviewerService =(token , id) => {
  return apiClient.get(`${CONTROLLER}/${id}`);
};