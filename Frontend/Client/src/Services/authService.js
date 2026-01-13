import axios from "axios";
import apiClient from "./apiClient";
const API_BASE_URL = "http://localhost:5233/api/Auth";

const CONTROLLER = "/Auth";

export const registerCandidate = (candidateData, token) => {
  return apiClient.post(`${CONTROLLER}/register-candidate`, candidateData);
};

export const registerOtherUser = (userData, token) => {
  return apiClient.post(`${CONTROLLER}/register-Users`, userData);
};

export const loginUser = (email, password) => {
  return apiClient.post(`${CONTROLLER}/login`, { email, password });
};

export const CandidateBulkRegisterService = (data, token) => {
  return apiClient.post(`${CONTROLLER}/Candidate-bulk-register`, data);
}
