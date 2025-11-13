// src/services/jobOpeningService.js
import axios from "axios";
import { resolvePath } from "react-router-dom";

const API_BASE_URL = "http://localhost:5233/api/JobOpening";

//  Fetch all job openings
export const getAllJobOpenings = async (token) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/list`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error in fetching job openings:", error);
    throw error;
  }
};

export const getJobOpeningById = async (id, token) => {
  try {
    const response = await axios.get(`${API_BASE_URL}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error in fetching job opening by ID:", error);
    throw error;
  }
};

export const getAllSkills = async () => {
  try{
  const response = await axios.get(`http://localhost:5233/api/Skill/All`);
  return response.data;
  } catch (error) {
    console.error("Error in fetching skills:", error);
    throw error;
  }
};

export const getAllReviewers = async () => {
  try {
  const response = await axios.get(`http://localhost:5233/api/Reviewer/All`);
  return response.data;
  } catch (error) {
    console.error("Error in fetching reviewers:", error);
    throw error;
  }
};

export const getAllInterviewers = async () => {
  try {
  const response = await axios.get(`http://localhost:5233/api/Interviewer/All`);
  return response.data;
  } catch (error) {
    console.error("Error in fetching interviewers:", error);
    throw error;
  }
};

export const getAllDocuments = async () => {
  try {
  const response = await axios.get(`http://localhost:5233/api/Document/All`);
  return response.data;
  } catch (error) {
    console.error("Error in fetching documents:", error);
    throw error;
  }
};

export const createJobOpening = async (jobData, token) => {
  try {
    console.log(jobData);
    
    const response = await axios.post(
      `${API_BASE_URL}/create`,
      jobData,
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
        error.response.data.message || "Failed to create job opening."
      );
  }
};
