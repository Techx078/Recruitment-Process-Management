import { useState, useEffect } from "react";
import { useAuthUserContext } from "../../Context/AuthUserContext";
import { fetchInterviewerService } from "../../Services/InterviewerService";
import { useNavigate, useParams } from "react-router-dom";
import { handleGlobalError } from "../../Services/errorHandler";

export default function InterviewerProfile({}) {
  let { UserId } = useParams();
  let [interviewer, setInterviewer] = useState(null);
  let [notFound, SetNotFound] = useState(false);
  let { authUser } = useAuthUserContext();
  let navigate = useNavigate();

  useEffect(() => {
    let fetchInterviewer = async () => {
      try {
        let data = await fetchInterviewerService(
          localStorage.getItem("token"),
          UserId
        );
        setInterviewer(data);
      } catch (e) {
        handleGlobalError(e);
        SetNotFound(true);
      }
    };
    fetchInterviewer();
  }, []);

  const navigateToPendingInterview = (department, jobOpeningId) => {
    if (department == "HR") {
      navigate(`/pool/hr/${jobOpeningId}`);
    } else {
      navigate(`/job-openings/${jobOpeningId}/technical-pool`);
    }
  };

  if (!interviewer) {
    return <div>Loading...</div>;
  }
  if (notFound) {
    return <div>Not found...</div>;
  }
  return (
    <div className="w-full min-h-screen bg-gray-100 p-6">
      <div className="max-w-4xl mx-auto space-y-6">
        {/* Profile Card */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Interviewer Profile</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p className="text-gray-600">Full Name</p>
              <p className="text-lg font-medium">{interviewer.user.fullName}</p>
            </div>

            <div>
              <p className="text-gray-600">Email</p>
              <p className="text-lg font-medium">{interviewer.user.email}</p>
            </div>

            <div>
              <p className="text-gray-600">Phone</p>
              <p className="text-lg font-medium">
                {interviewer.user.phoneNumber}
              </p>
            </div>

            <div>
              <p className="text-gray-600">Department</p>
              <span className="inline-block bg-blue-100 text-blue-700 px-3 py-1 rounded-lg text-sm font-medium">
                {interviewer.department}
              </span>
            </div>
          </div>
        </div>

        {/* Assigned Job Openings */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Assigned Job Openings</h2>

          <div className="space-y-4">
            {interviewer.assignedJobOpenings.length === 0 ? (
              <p className="text-gray-500">No assigned job openings.</p>
            ) : (
              interviewer?.assignedJobOpenings.map((job) => (
                <div
                  key={job.jobOpeningId}
                  className="border rounded-xl p-4 shadow-sm bg-white"
                >
                  <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-3">
                    <div>
                      <h3 className="text-xl font-semibold">{job?.title}</h3>
                      <p className="text-gray-600">
                        Department: {job?.department}
                      </p>
                      <p className="text-gray-600">Domain: {job?.domain}</p>
                      <p className="text-gray-600">
                        Experience: {job?.minDomainExperience}
                      </p>
                      <p className="text-gray-600">
                        Created: {new Date(job?.createdAt).toLocaleDateString()}
                      </p>
                    </div>

                    <div className="flex flex-col items-start md:items-end gap-2">
                      <span className="bg-green-100 text-green-700 px-3 py-1 rounded-lg text-sm font-medium">
                        {job?.status}
                      </span>
                      <p className="text-gray-700 text-sm">
                        Candidates: {job?.candidateCount}
                      </p>
                      <p className="text-gray-700 text-sm">
                        Interviewers: {job?.interviewerCount}
                      </p>
                      <p className="text-gray-700 text-sm">
                        Type: {job?.jobType}
                      </p>

                      {/* Show Button */}
                      <button
                        className="px-4 py-1 bg-gray-800 text-white text-sm rounded-lg hover:bg-gray-700"
                        onClick={() =>
                          navigate(`/job-openings/${job.jobOpeningId}`)
                        }
                      >
                        Show
                      </button>
                      {authUser && authUser.role === "Interviewer" && (
                        <button
                          className="px-4 py-1 bg-gray-800 text-white text-sm rounded-lg hover:bg-gray-700"
                          onClick={() => {
                            navigateToPendingInterview(
                              interviewer.department,
                              job.jobOpeningId
                            );
                          }}
                        >
                          Pending-Interview
                        </button>
                      )}
                      {authUser &&
                        authUser.role === "Interviewer" &&
                        interviewer.department == "HR" && (
                          <button
                            className="px-4 py-1 bg-gray-800 text-white text-sm rounded-lg hover:bg-gray-700"
                            onClick={() =>
                              navigate(
                                `/pool/document-upload/${job.jobOpeningId}`
                              )
                            }
                          >
                            verify-documents
                          </button>
                        )}
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
