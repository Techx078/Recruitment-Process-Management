import apiClient from "./apiClient";
const CONTROLLER = "/authorization";

export const validateHrLevelAccess = (jobOpeningId, token) => {
  return apiClient.get(`${CONTROLLER}/hr-level/${jobOpeningId}`);
};

export const validateCandidate = ( candidateId ) => {
return apiClient.get(`${CONTROLLER}/candidate/${candidateId}`);
}
