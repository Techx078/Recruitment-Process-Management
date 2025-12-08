import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getAllReviewers,
  getJobOpeningById,
  updateJobReviewers,
} from "../../Services/JobOpeningService";

const EditJobReviewers = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [reviewers, setReviewers] = useState([]);
  const [selected, setSelected] = useState([]);

  useEffect(() => {
    const load = async () => {
      const token = localStorage.getItem("token");

      const allReviewers = await getAllReviewers(token);
      setReviewers(allReviewers);

      const job = await getJobOpeningById(id, token);
      console.log(job);
      setSelected(job.reviewers.map((r) => r.id));
    };

    load();
  }, [id]);

  const toggle = (rid) => {
    setSelected((prev) =>
      prev.includes(rid) ? prev.filter((i) => i !== rid) : [...prev, rid]
    );
  };

  const save = async () => {
    const token = localStorage.getItem("token");
    await updateJobReviewers(id, selected, token);
    alert("Reviewers updated!");
    navigate(`/job-openings/${id}`);
  };

  return (
    <div className="p-6 max-w-xl mx-auto bg-white shadow rounded">
      <h2 className="text-xl font-bold mb-4">Update Reviewers</h2>

      {reviewers.map((r) => (
        <label
          key={r.id}
          className="flex items-center gap-3 border p-2 rounded mb-2"
        >
          <input
            type="checkbox"
            checked={selected.includes(r.id)}
            onChange={() => toggle(r.id)}
          />
          <div>
            <div className="font-medium">{r.user?.email}</div>
            <div className="text-sm text-gray-600">ID: {r.id}</div>
          </div>
          <div>
            <button
              type="button"
              onClick={() => window.open(`/Reviewer/Profile/${r.user.id}`)}
              className="px-5 py-1 ml-4 rounded bg-gray-600 text-white hover:bg-gray-800"
            >
              Profile
            </button>
          </div>
        </label>
      ))}

      <button
        onClick={save}
        className="mt-4  bg-gray-900 text-white px-4 py-2 rounded"
      >
        Save Changes
      </button>

      <button
        onClick={() => navigate(-1)}
        className="mt-4 ml-3 bg-gray-500 text-white px-4 py-2 rounded"
      >
        Cancel
      </button>
    </div>
  );
};

export default EditJobReviewers;
