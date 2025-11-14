import axios from "axios";


let API_BASE_URL = "http://localhost:5233/api/Interviewer";


export const fetchInterviewerService = async (token , id) => {
     try {
    const response = await axios.get(`${API_BASE_URL}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error in fetch Inteerviewer by ID:", error);
    throw error;
  }
};