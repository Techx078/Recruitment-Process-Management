import axios from "axios";
const API_BASE_URL = "http://localhost:5233/api/Auth"
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
    throw new Error(
      error.response?.data?.message ||
      error.response?.data ||
      "Registration failed"
    );
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
    console.log(error);
    throw new Error(
      error.response?.data?.message ||
      error.response?.data ||
      "Registration failed"
    );
  }
};

export const loginUser = async (email, password) => {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/login`,
      {
        email,
        password,
      },
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );

    return response.data;

  } catch (error) {
    throw new Error(
      error.response?.data?.message ||
      error.response?.data ||
      "Invalid credentials or server error"
    );
  }
};
