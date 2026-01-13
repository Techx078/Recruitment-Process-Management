import apiClient from "./apiClient";
const CONTROLLER = "/Recruiter";

export const fetchRecruiterService =(token , id) => {
  return apiClient.get(`${CONTROLLER}/${id}`);
};
export const fetchJobOpeningsByRecruiter = async (token, id) => {
  const data = await apiClient.get(`${CONTROLLER}/${id}`);
  return data.assignedJobOpenings;
};