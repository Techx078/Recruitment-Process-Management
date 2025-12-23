import axios from "axios";


let API_BASE_URL = "http://localhost:5233/api/Reviewer";


export  const fetchReviewerService = async (token , id) => {
     try {
    const response = await axios.get(`${API_BASE_URL}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
};