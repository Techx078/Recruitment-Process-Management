import axios from "axios";
const API_BASE_URL = "http://localhost:5233/api/authorization";


export const validateHrLevelAccess = (jobOpeningId, token) => {
  try {
    return axios.get(
      `${API_BASE_URL}/hr-level/${jobOpeningId}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );
  } catch (e) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};
