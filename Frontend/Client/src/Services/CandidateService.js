// Services/CandidateService.js
import axios from "axios";
let API_BASE_URL = "http://localhost:5233/api/Candidate";

export const getCandidateDetails = async (userId, token) => {
  try {
    const res = await axios.get(`${API_BASE_URL}/${userId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return res.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};
// Services/CandidateJobService.js
export const getCandidateJobOpenings = async (userId, token) => {
  try {
    const res = await axios.get(`${API_BASE_URL}/jobOpening/${userId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return res.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};

export const updateCandidateService = async (userId, candidateData, token) => {
  try {
    const response = await axios.put(
      `${API_BASE_URL}/update/${userId}`,
      candidateData,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );

    // Success: return data to caller
    return response.data;

  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};
