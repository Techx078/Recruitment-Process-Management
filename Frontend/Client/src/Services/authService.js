import axios from "axios";

const API_BASE_URL = "http://localhost:5233/api/Auth";

export const registerCandidate = async (candidateData, token) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/register-candidate`,
      candidateData,
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

export const registerOtherUser = async (userData, token) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/register-Users`,
      userData,
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

export const loginUser = async (email, password) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/login`,
      { email, password },
      {
        headers: {
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

export const CandidateBulkRegisterService = async (data, token) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/Candidate-bulk-register`,
      data,
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
