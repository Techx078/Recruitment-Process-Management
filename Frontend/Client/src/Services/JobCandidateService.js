import apiClient from "./apiClient";
const CONTROLLER = "/JobCandidate";

export const CreateJobCandidateService = (token, JobcandidateData) => {
return apiClient.post(`${CONTROLLER}/create`, JobcandidateData);
};

export const CreateJobCandidateBulkService = (token, Data) => {
  return apiClient.post(`${CONTROLLER}/CreateBulk`, Data);
};

export const getJobCandidateById = (jobCandidateId) => {
  return apiClient.get(`${CONTROLLER}/get/${jobCandidateId}`);
};

export const getPendingReviewCandidate = (jobOpeningId, token) => {
  return apiClient.get(`${CONTROLLER}/pool/review/${jobOpeningId}`);
};

export const updateReviewStatus = (jobCandidateId, data) => {
  return apiClient.put(`${CONTROLLER}/review/${jobCandidateId}`, data)
};

export const getTechnicalInterviewPool = (jobOpeningId) => {
  return apiClient.get(`${CONTROLLER}/pool/technical/${jobOpeningId}`)
};

export const getMyScheduledInterviews = (jobOpeningId) => {
  return apiClient.get(`${CONTROLLER}/pool/my-scheduled/${jobOpeningId}`)
};

export const scheduleInterview = (jobCandidateId, data) => {
  return apiClient.put(`${CONTROLLER}/schedule/${jobCandidateId}`, data)
};

export const submitInterviewFeedback = (jobCandidateId, data) => {
  return apiClient.put(`${CONTROLLER}/Interview-feedback/${jobCandidateId}`, data)
};

export const getHrPool = (jobOpeningId) => {
  return apiClient.get(`${CONTROLLER}/pool/hr/${jobOpeningId}`)
};

export const getFinalPool = (jobOpeningId) => {
  return apiClient.get(`${CONTROLLER}/pool/shortlist/${jobOpeningId}`)
};

export const selectCandidate = (jobCandidateId) => {
  return apiClient.put(`${CONTROLLER}/select/${jobCandidateId}`,{});
};

export const getCandidateInterviewHistory = (jobCandidateId) => {
  return apiClient.get(`${CONTROLLER}/history/${jobCandidateId}`);
};

export const sendOffer = (jobCandidateId, offerExpiryDate) => {
  return apiClient.put(`${CONTROLLER}/send-offer/${jobCandidateId}`, { offerExpiryDate });
};

export const getSentOfferPool = (jobOpeningId) => {
  return apiClient.get(`${CONTROLLER}/pool/offerSend/${jobOpeningId}`);
};

export const rejectOfferBySystem = (jobCandidateId, reason) => {
  return apiClient.put(`${CONTROLLER}/reject-by-system/${jobCandidateId}`, { reason });
};

export const extendOfferExpiry = (jobCandidateId, newExpiryDate) => {
  return apiClient.put(`${CONTROLLER}/${jobCandidateId}/extend-expiry`, { newExpiryDate });
};

export const respondToOffer = (
  jobCandidateId,
  isAccepted,
  rejectionReason = null
) => {
    const payload = {
      isAccepted,
      rejectionReason: isAccepted ? null : rejectionReason,
    };
    return apiClient.put(`${CONTROLLER}/respond-offer/${jobCandidateId}`, payload);
};

export const uploadJobCandidateDocument = (
  jobCandidateId,
  jobDocumentId,
  file
) => {
    const formData = new FormData();
    formData.append("file", file);
    return apiClient.post(`${CONTROLLER}/${jobCandidateId}/documents/${jobDocumentId}`, formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    ); 
};

export const submitAllDocuments = (jobCandidateId) => {
  return apiClient.put(`${CONTROLLER}/${jobCandidateId}/documents/submit`,{});
};

export const getCandidateDocuments = (jobCandidateId) => {
  return apiClient.get(`${CONTROLLER}/${jobCandidateId}/documents`);  
};

export const verifyCandidateDocuments = (
  jobCandidateId,
  isVerified,
  rejectionReason = null
) => {
  return apiClient.put(`${CONTROLLER}/${jobCandidateId}/documents/verify`, {
    isVerified,
    rejectionReason,
  });
};

export const getDocumentUploadedPool = (jobOpeningId) => {
  return apiClient.get(`${CONTROLLER}/pool/documentUploaded/${jobOpeningId}`);
};

export const getPostOfferPool = (jobOpeningId) => {
  return apiClient.get(`${CONTROLLER}/pool/postOffer/${jobOpeningId}`);
};

export const sendJoiningDate = (jobCandidateId, joiningDate) => {
  return apiClient.put(`${CONTROLLER}/send-Joining-Date/${jobCandidateId}`, { joiningDate });
};

export const addCandidateToEmployee = (jobCandidateId) => {
  return apiClient.post(`${CONTROLLER}/Create-Employee/${jobCandidateId}`, {}); 
};