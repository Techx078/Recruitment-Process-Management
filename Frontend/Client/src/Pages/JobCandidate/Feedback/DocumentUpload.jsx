import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  uploadJobCandidateDocument,
  submitAllDocuments,
} from "../../../Services/JobCandidateService";
import { getJobOpeningById } from "../../../Services/JobOpeningService";
import { toast } from "react-toastify";
import { useAuthUserContext } from "../../../Context/AuthUserContext";
import { handleGlobalError } from "../../../Services/errorHandler";
export default function DocumentUpload() {
  const { jobOpeningId , jobCandidateId } = useParams();
  const {authUser} = useAuthUserContext();
  const [jobOpening, setJobOpening] = useState(null);
  const [files, setFiles] = useState({});
  const [uploadedDocs, setUploadedDocs] = useState({});
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  useEffect(() => {
    fetchJobOpening();
  }, []);

  const fetchJobOpening = async () => {
    try {
      const token = localStorage.getItem("token")
      const res = await getJobOpeningById(jobOpeningId,token);
      setJobOpening(res);
    } catch (err) {
      handleGlobalError(err)
    }
  };

  const handleFileChange = (jobDocumentId, file) => {
    setFiles((prev) => ({ ...prev, [jobDocumentId]: file }));
  };

  const handleUpload = async (jobDocumentId) => {
    if (!files[jobDocumentId]) return;

    try {
      setLoading(true);
      const res = await uploadJobCandidateDocument(
        jobCandidateId,
        jobDocumentId,
        files[jobDocumentId]
      );
      toast.success("document uploaded successfully!")
      setUploadedDocs((prev) => ({
        ...prev,
        [jobDocumentId]: res.data.documentUrl,
      }));
    } catch (err) {
      handleGlobalError(err)
    } finally {
      setLoading(false);
    }
  };

  const allMandatoryUploaded = () => {
    return jobOpening.documents.every(
      (d) => !d.IsMandatory || uploadedDocs[d.jobDocumentId]
    );
  };

  const handleSubmitAll = async () => {
    try {
      await submitAllDocuments(jobCandidateId);
      toast.success("Documents submitted successfully");
      navigate(`/Candidate/Profile/${authUser.id}`)
    } catch (err) {
      handleGlobalError(err)
    }
  };

  if (!jobOpening) return <p className="p-6">Loading...</p>;

  return (
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      <h1 className="text-2xl font-semibold">{jobOpening.Title}</h1>
      <p className="text-gray-600">
        Upload the required documents for this job application.
      </p>

      {/* DOCUMENT LIST */}
      <div className="space-y-4">
        {jobOpening?.documents?.map((doc) => (
          <div
            key={doc?.jobDocumentId}
            className="border border-gray-300 rounded-lg p-4 space-y-3"
          >
            <div>
              <h2 className="font-medium">{doc?.Name}</h2>
              <p className="text-sm text-gray-600">{doc?.Description}</p>
              {doc?.IsMandatory && (
                <span className="text-xs text-red-600">Mandatory</span>
              )}
            </div>

            {uploadedDocs[doc?.jobDocumentId] ? (
              <div className="flex items-center justify-between">
                <a
                  href={uploadedDocs[doc?.jobDocumentId]}
                  target="_blank"
                  rel="noreferrer"
                  className="text-sm text-blue-600 underline"
                >
                  View uploaded document
                </a>
                <button
                  onClick={() => handleUpload(doc?.jobDocumentId)}
                  className="px-3 py-1 border border-gray-400 rounded text-sm"
                >
                  Re-upload
                </button>
              </div>
            ) : (
              <div className="flex items-center gap-3">
                <p>{doc?.name}</p>
                <input
                  type="file"
                  onChange={(e) =>
                    handleFileChange(doc?.jobDocumentId, e.target.files[0])
                  }
                  className="text-sm border"
                />
                <button
                  disabled={loading}
                  onClick={() => handleUpload(doc?.jobDocumentId)}
                  className="px-3 py-1 bg-black text-white rounded text-sm"
                >
                  Upload
                </button>
              </div>
            )}
          </div>
        ))}
      </div>

      {/* SUBMIT ALL */}
      <button
        disabled={!allMandatoryUploaded()}
        onClick={handleSubmitAll}
        className={`w-full py-3 rounded text-white ${
          allMandatoryUploaded()
            ? "bg-black"
            : "bg-gray-400 cursor-not-allowed"
        }`}
      >
        Submit All Documents
      </button>
    </div>
  );
}