// src/services/jobOpeningService.js
import axios from "axios";
import { resolvePath } from "react-router-dom";

const API_BASE_URL = "http://localhost:5233/api/JobOpening"; 

//  Fetch all job openings
export const getAllJobOpenings = async (token) => {
  const response = await axios.get(`${API_BASE_URL}/list`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  // console.log(response.data);
  return response.data;
};

export const getJobOpeningById = async (id, token) => {
  const response = await axios.get(`${API_BASE_URL}/${id}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return response.data;
};
