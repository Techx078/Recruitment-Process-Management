let API_BASE_URL = "http://localhost:5233/api/JobCandidate";
import axios from "axios";

export const CreateJobCandidateService = async (token, JobcandidateData) => {
    try {
    const response = await axios.post(`${API_BASE_URL}/create`, JobcandidateData, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
        },
    });
    return response.data;
    } catch (error) {
    console.error("Error in Create Job Candidate:", error);
    throw error;
  } 
};