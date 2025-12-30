let API_BASE_URL = "http://localhost:5233/api/JobCandidate";
import axios from "axios";

export const CreateJobCandidateService = async (token, JobcandidateData) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/create`,
      JobcandidateData,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const CreateJobCandidateBulkService = async (token, Data) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/CreateBulk`, Data, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const getJobCandidateById = async (jobCandidateId) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/get/${jobCandidateId}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const getPendingReviewCandidate = async (jobOpeningId, token) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/pool/review/${jobOpeningId}`,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const updateReviewStatus = async (jobCandidateId, data) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/review/${jobCandidateId}`,
      data,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const getTechnicalInterviewPool = async (jobOpeningId) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/pool/technical/${jobOpeningId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const getMyScheduledInterviews = async (jobOpeningId) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/pool/my-scheduled/${jobOpeningId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const scheduleInterview = async (jobCandidateId, data) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/schedule/${jobCandidateId}`,
      data,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const submitInterviewFeedback = async (jobCandidateId, data) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/Interview-feedback/${jobCandidateId}`,
      data,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const getHrPool = async (jobOpeningId) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/pool/hr/${jobOpeningId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const getFinalPool = async (jobOpeningId) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/pool/shortlist/${jobOpeningId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const selectCandidate = async (jobCandidateId) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/select/${jobCandidateId}`,
      {},
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const getCandidateInterviewHistory = async (jobCandidateId) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/history/${jobCandidateId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const sendOffer = async (jobCandidateId, offerExpiryDate) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/send-offer/${jobCandidateId}`,
      { offerExpiryDate },
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;
    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};
export const getSentOfferPool = async (jobOpeningId) => {
  try {
    const res = await axios.get(
      `${API_BASE_URL}/pool/offerSend/${jobOpeningId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
    return res.data;
  } catch (error) {
    if (error.response) throw error.response;
    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const rejectOfferBySystem = (jobCandidateId, reason) => {
  try {
    return axios.put(
      `${API_BASE_URL}/reject-by-system/${jobCandidateId}`,
      { reason },
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
  } catch (error) {
    if (error.response) throw error.response;
    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const extendOfferExpiry = (jobCandidateId, newExpiryDate) => {
  try {
    return axios.put(
      `${API_BASE_URL}/${jobCandidateId}/extend-expiry`,
      { newExpiryDate },
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
  } catch (error) {
    if (error.response) throw error.response;
    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const respondToOffer = async (
  jobCandidateId,
  isAccepted,
  rejectionReason = null
) => {
  try {
    const payload = {
      isAccepted,
      rejectionReason: isAccepted ? null : rejectionReason,
    };

    const response = await axios.put(
      `${API_BASE_URL}/respond-offer/${jobCandidateId}`,
      payload,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
          "Content-Type": "application/json",
        },
      }
    );

    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw {
      status: 0,
      message: "Something went wrong. Please try again.",
    };
  }
};