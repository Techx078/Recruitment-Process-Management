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

export const getAllSkills = async (token) => {
  try {
    const response = await axios.get(`http://localhost:5233/api/Skill/All`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error in fetching skills:", error);
    throw error;
  }
};

export const getAllReviewers = async (token) => {
  try {
    const response = await axios.get(`http://localhost:5233/api/Reviewer/All`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error in fetching reviewers:", error);
    throw error;
  }
};

export const getAllInterviewers = async (token) => {
  try {
    const response = await axios.get(
      `http://localhost:5233/api/Interviewer/All`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.error("Error in fetching interviewers:", error);
    throw error;
  }
};

export const getAllDocuments = async (token) => {
  try {
    const response = await axios.get(`http://localhost:5233/api/Document/All`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error in fetching documents:", error);
    throw error;
  }
};

export const createJobOpening = async (jobData, token) => {
  try {
    console.log(jobData);

    const response = await axios.post(`${API_BASE_URL}/create`, jobData, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    throw new Error(
      error.response.data.message || "Failed to create job opening."
    );
  }
};

export const updateJobFields = async (id, data, token) => {
  try {
    const res = await axios.put(`${API_BASE_URL}/${id}/fields`, data, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });
    return res.data;
  } catch (error) {
    throw new Error(
      error.response.data.message || "Failed to update job opening."
    );
  }
};

export const updateJobReviewers = async (id, reviewerIds, token) => {
  try {
    const res = await axios.patch(
      `${API_BASE_URL}/update-reviewers/${id}`,
      reviewerIds,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );
  } catch (error) {
    console.log(error);

    throw new Error(
      error.response.data.message || "Failed to reviewer job opening."
    );
  }
};

export const updateJobInterviewers = async (id, interviewerIds, token) => {
  try {
    const res = await axios.patch(
      `${API_BASE_URL}/update-interviewers/${id}`,
      interviewerIds,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );
  } catch (error) {
    throw new Error(
      error.response.data.message || "Failed to update Interviewer opening."
    );
  }
};

export const updateJobDocument = async (id, documents, token) => {
  try {
    const res = await axios.patch(
      `${API_BASE_URL}/update-documents/${id}`,
      documents,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );
  } catch (error) {
    throw new Error(
      error.response.data.message || "Failed to update Interviewer opening."
    );
  }
};

export const updateJobSkill = async (id, skills, token) => {
  try {
    const res = await axios.patch(
      `${API_BASE_URL}/update-skills/${id}`,
      skills,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      }
    );
  } catch (error) {
    console.log(error);
    
    throw new Error(
      error.response.data.message || "Failed to update Interviewer opening."
    );
  }
};