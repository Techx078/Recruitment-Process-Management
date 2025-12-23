import axios from "axios";
import { handleGlobalError } from "./errorHandler";

const apiClient = axios.create({
  baseURL: "http://localhost:5233/api",
  headers: {
    "Content-Type": "application/json",
  },
});

// Attach token automatically
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const data = error.response?.data;

    const apiError = {
      status: error.response?.status,
      message: data?.message,
      errorCode: data?.errorCode,
      errors: data?.errors || [],
      traceId: data?.traceId,
    };

    handleGlobalError(apiError);

    return Promise.reject(apiError);
  }
);

export default apiClient;
