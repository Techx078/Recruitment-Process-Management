// Services/CandidateService.js
import axios from "axios";
let API_BASE_URL = "http://localhost:5233/api/Candidate";

export const getCandidateDetails = async (userId) => {
  try {
    const res = await axios.get(`${API_BASE_URL}/${userId}`);
    return res.data;
  } catch (error) {
    console.error("Error fetching candidate details:", error);
    throw error.response?.data || "Something went wrong";
  }
};
// Services/CandidateJobService.js
export const getCandidateJobOpenings = async (userId) => {
  try {
    const res = await axios.get(`${API_BASE_URL}/jobOpening/${userId}`);
    return res.data;
  } catch (error) {
    console.error("Error fetching candidate job openings:", error);
    throw error.response?.data || "Something went wrong";
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
        console.log(response.data);
        
        return response.data;
    } catch (error) {
        if (error.response && error.response.status === 400) {

            console.log(error.response.data);
            
        const validationErrors = error.response.data.errors;

        // Loop through errors
        Object.keys(validationErrors).forEach(field => {
            alert(validationErrors[field][0]);
        });
    }
        throw error.response?.data || "Something went wrong";
    }
};
