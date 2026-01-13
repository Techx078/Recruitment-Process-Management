import apiClient from "./apiClient";
const CONTROLLER = "/JobOpening";

export const getAllJobOpenings = async (token) => {
  return apiClient.get(`${CONTROLLER}/list`);
};

export const getJobOpeningById = async (id, token) => {
  return apiClient.get(`${CONTROLLER}/${id}`);
};

export const getAllSkills = async (token) => {
  return apiClient.get(`Skill/All`);
};

export const getAllReviewers = async (token) => {
  return apiClient.get(`Reviewer/All`);
};

export const getAllInterviewers = async (token) => {
  return apiClient.get(`Interviewer/All`);
};

export const getAllDocuments = async (token) => {
  return apiClient.get(`Document/All`);
};

export const createJobOpening = async (jobData, token) => {
  return apiClient.post(`${CONTROLLER}/create`, jobData);
};

export const updateJobFields = async (id, data, token) => {
  return apiClient.put(`${CONTROLLER}/${id}/fields`, data);
};

export const updateJobReviewers = async (id, reviewerIds, token) => {
  return apiClient.patch(`${CONTROLLER}/update-reviewers/${id}`, reviewerIds);
};

export const updateJobInterviewers = async (id, interviewerIds, token) => {
  return apiClient.patch(`${CONTROLLER}/update-interviewers/${id}`, interviewerIds);
};
 
export const updateJobDocument = async (id, documents, token) => {
  return apiClient.patch(`${CONTROLLER}/update-documents/${id}`, documents);
};

export const updateJobSkill = async (id, skills, token) => {
  return apiClient.patch(`${CONTROLLER}/update-skills/${id}`, skills);
};