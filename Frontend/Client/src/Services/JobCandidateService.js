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