import apiClient from "./apiClient";
const CONTROLLER = "/Reviewer";

export  const fetchReviewerService =(token , id) => {
  return apiClient.get(`${CONTROLLER}/${id}`);
};