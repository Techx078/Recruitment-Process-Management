import axios from "axios";

let API_BASE_URL = "http://localhost:5233/api/Recruiter";

export const fetchRecruiterService = async (token , id) => {
    console.log(token);
    console.log("Fetching recruiter with ID:", id);
    
     try {
    const response = await axios.get(`${API_BASE_URL}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    return response.data;
  } catch (error) {
    console.error("Error in fetch Recruiter by ID:", error);
    throw error;
  }
};