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
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
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
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
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
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
  }
};

export const getPendingReviewCandidate = async (jobOpeningId, token) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/pending-review/${jobOpeningId}`,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
  }
};

export const updateReviewStatus = async (jobCandidateId, data) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/review-status/${jobCandidateId}`,
      data,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.log(error);

    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
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
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
  }
};

export const getMyScheduledInterviews = async (jobOpeningId) => {
  try {
    const response = await axios.get(
      `${API_BASE_URL}/my-scheduled/${jobOpeningId}`,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
  }
};

export const scheduleInterview = async (jobCandidateId, data) => {
  try {
    const response = await axios.post(
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
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
  }
};

export const submitInterviewFeedback = async (jobCandidateId, data) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/feedback/${jobCandidateId}`,
      data,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    );

    return response.data;
  } catch (error) {
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
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
    // Throw error to be handled in frontend catch
    if (error.response) {
      throw error.response; // include status and data
    } else {
      throw error; // network or other error
    }
  }
};
