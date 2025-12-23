import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  getAllDocuments,
  getJobOpeningById,
  updateJobDocument,
} from "../../Services/JobOpeningService";
import { toast } from "react-toastify";
import { handleGlobalError } from "../../Services/errorHandler";

const EditJobDocument = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [documents, setDocuments] = useState([]);
  const [selected, setSelected] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const load = async () => {
      try {
        const token = localStorage.getItem("token");

        const allDocuments = await getAllDocuments(token);
        setDocuments(allDocuments);

        const job = await getJobOpeningById(id, token);
        console.log(job);

        const selected = job.documents.map((d) => ({
          documentId: d.id,
          isMandatory: d.isMandatory,
        }));
        setSelected(selected);
      } catch (e) {
        handleGlobalError(e);
        setError("server Error !");
      }
    };

    load();
  }, [id]);
  const toggleDocument = (documentId, isChecked) => {
    setSelected((prev) => {
      if (isChecked) {
        if (prev.some((d) => d.documentId === documentId)) {
          return prev;
        }
        return [...prev, { documentId, isMandatory: false }];
      } else {
        return prev.filter((d) => d.documentId !== documentId);
      }
    });
  };
  const toggleMandatoryDocument = (documentId) => {
    setSelected((prev) =>
      prev.map((doc) =>
        doc.documentId === documentId
          ? { ...doc, isMandatory: !doc.isMandatory }
          : doc
      )
    );
  };

  const save = async () => {
    try {
      const token = localStorage.getItem("token");
      console.log(selected);
      await updateJobDocument(id, selected, token);
      toast.success("Document updated!");
      navigate(`/job-openings/${id}`);
    } catch (e) {
      handleGlobalError(e);
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
      <h2 className="text-xl font-bold mb-4">Update Documents</h2>

      <div className="border p-3 rounded">
        <label className="block font-semibold mb-2">Required Documents</label>

        <div className="space-y-2 max-h-40 overflow-auto border p-2 rounded">
          {documents.map((d) => {
            const selectedDoc = selected.find((doc) => doc.documentId === d.id);
            const isSelected = selectedDoc !== undefined;
            const isMandatory = selectedDoc?.isMandatory || false;

            return (
              <div key={d.id}>
                {/* document checkbox */}
                <label className="flex items-center gap-3 p-2 cursor-pointer hover:bg-gray-100 rounded">
                  <input
                    type="checkbox"
                    checked={isSelected}
                    onChange={(e) => toggleDocument(d.id, e.target.checked)}
                  />
                  <div>
                    <div className="font-medium">{d.name}</div>
                  </div>
                </label>

                {/* Mandatory toggle */}
                {isSelected && (
                  <label className="flex items-center gap-2 text-sm cursor-pointer ml-8">
                    <input
                      type="checkbox"
                      checked={isMandatory}
                      onChange={() => toggleMandatoryDocument(d.id)}
                    />
                    Mandatory
                  </label>
                )}
              </div>
            );
          })}
        </div>
      </div>

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

export default EditJobDocument;
