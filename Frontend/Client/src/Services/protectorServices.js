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

export const validateCandidate = ( candidateId ) => {
  try{
    return axios.get(
      `${API_BASE_URL}/candidate/${candidateId}`,
      {
        headers : {
          Authorization : `Bearer ${localStorage.getItem("token")}`
        },
      }
    );
  }catch(error){
    console.log(error);
     if (error.response) throw error.response;

    throw { status: 0, message: "Something went wrong. Please try again." };
  }
}
