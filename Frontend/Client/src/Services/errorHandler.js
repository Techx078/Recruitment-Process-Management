import { toast } from "react-toastify";

export const handleGlobalError = (error) => {
  
  const { status } = error;
  let errors = error?.data?.Errors;
  let message = error?.data?.Message;
  let traceId = error?.data?.TraceId;

  const showErrorsSequentially = (errors, delay = 1500) => {
    errors.forEach((err, index) => {
      setTimeout(() => {
        toast.error(err);
      }, index * delay);
    });
  };

  if (status === 400) {
    if (Array.isArray(errors) && errors.length > 0) {
      showErrorsSequentially(errors);
      return;
    }
    else if (message) {
      toast.error(message);
      return;
    }
    toast.error("Invalid request. Please check your input.");
    return;
  }

  switch (status) {
    case 401:
      toast.error(message);
      localStorage.clear();
      break;

    case 403:
      toast.error("You are not authorized to perform this action.");
      break;

    case 404:
      toast.error(message || "Resource not found.");
      break;

    case 408:
      toast.error("Request timeout. Try again.");
      break;
    case 409:
      toast.error(message);
      break;
    case 500:
      toast.error("server error !")
      break;
    default:
      console.error("TraceId:", traceId);
      toast.error("Something went wrong. Please contact support.");
      break;
  }
};
