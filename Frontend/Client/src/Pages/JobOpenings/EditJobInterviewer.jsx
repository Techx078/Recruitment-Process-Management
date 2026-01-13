import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getAllInterviewers,
  getJobOpeningById,
  updateJobInterviewers,
} from "../../Services/JobOpeningService";
import { handleGlobalError } from "../../Services/errorHandler";
import { toast } from "react-toastify";

const EditJobInterviewers = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [interviewers, setInterviewers] = useState([]);
  const [selected, setSelected] = useState([]);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    const load = async () => {
      try{
      const token = localStorage.getItem("token");

      // Load all interviewers
      const allInterviewers = await getAllInterviewers(token);
      setInterviewers(allInterviewers);

      // Load selected interviewers for this job
      const job = await getJobOpeningById(id, token);
      setSelected(job.interviewers.map(i => i.id));
      }catch(e){
        handleGlobalError(e);
        setError("server error !")
      }
    };

    load();
  }, [id]);

  // Toggle checkbox
  const toggle = (iid) => {
    setSelected((prev) =>
      prev.includes(iid)
        ? prev.filter((x) => x !== iid)
        : [...prev, iid]
    );
  };

  // Save to backend
  const save = async () => {
    try {
      setIsLoading(true);
      const token = localStorage.getItem("token");
      await updateJobInterviewers(id, selected, token);
      toast.success("Interviewers updated!");
      setIsLoading(false);
      navigate(`/job-openings/${id}`);
    } catch (error) {
      handleGlobalError(error);
    }finally {
      setIsLoading(false);
    }
  };
   
  if (error) {
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        {error}
      </div>
    );
  }
  return (
    <div className="p-6 max-w-xl mx-auto bg-white shadow rounded">
      <h2 className="text-xl font-bold mb-4">Update Interviewers</h2>

      {interviewers.map((i) => (
        <label
          key={i.id}
          className="flex items-center gap-3 border p-2 rounded mb-2 cursor-pointer hover:bg-gray-50"
        >
          <input
            type="checkbox"
            checked={selected.includes(i.id)}
            onChange={() => toggle(i.id)}
          />

          <div>
            <div className="font-medium">{i.user?.email}</div>
            <div className="text-sm text-gray-600">
              Interviewer ID: {i.id}
            </div>
          </div>
          <div>
            <button
              type="button"
              onClick={() => window.open(`/Interviewer/Profile/${i.user.id}`,"_blank")}
              className="px-5 py-1 ml-4 rounded bg-gray-600 text-white hover:bg-gray-800"
            >
              Profile
            </button>
          </div>
        </label>
      ))}

      <div className="flex gap-3 mt-4">
        <button
          disabled={isLoading}
          onClick={save}
          className="bg-black text-white px-4 py-2 rounded"
        >
          {isLoading ? "Saving..." : "Save Changes"}
          
        </button>

        <button
          onClick={() => navigate(-1)}
          className="bg-gray-500 text-white px-4 py-2 rounded"
        >
          Cancel
        </button>
      </div>
    </div>
  );
};

export default EditJobInterviewers;
