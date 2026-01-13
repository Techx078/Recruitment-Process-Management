import apiClient from "./apiClient";
const CONTROLLER = "/Candidate";

export const getCandidateDetails =(userId, token) => {
  return apiClient.get(`${CONTROLLER}/${userId}`)
};
// Services/CandidateJobService.js
export const getCandidateJobOpenings =(userId, token) => {
  return apiClient.get(`${CONTROLLER}/jobOpening/${userId}`);
};

export const updateCandidateService =(userId, candidateData, token) => {
  return apiClient.put(`${CONTROLLER}/update/${userId}`, candidateData);
};
