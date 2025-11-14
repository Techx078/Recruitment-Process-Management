import { useState, useEffect } from "react";
import { useAuthUserContext } from "../../Context/AuthUserContext";
import { fetchReviewerService } from "../../Services/ReviewerService";
import { useNavigate } from "react-router-dom";

export default function ReviewerProfile({}) {
  let [reviewer, setReviewer] = useState(null);

  let { authUser } = useAuthUserContext();
  let navigate = useNavigate();
  useEffect(() => {
    let fetchReviewer = async () => {
      let data = await fetchReviewerService(localStorage.getItem("token"), authUser.id);
      setReviewer(data);
    };
    fetchReviewer();
  }, []);

  if (!reviewer) {
    return <div>Loading...</div>;
  }

  return (
    <div className="w-full min-h-screen bg-gray-100 p-6">
      <div className="max-w-4xl mx-auto space-y-6">
        {/* Profile Card */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Reviewer Profile</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p className="text-gray-600">Full Name</p>
              <p className="text-lg font-medium">{reviewer.user.fullName}</p>
            </div>

            <div>
              <p className="text-gray-600">Email</p>
              <p className="text-lg font-medium">{reviewer.user.email}</p>
            </div>

            <div>
              <p className="text-gray-600">Phone</p>
              <p className="text-lg font-medium">{reviewer.user.phoneNumber}</p>
            </div>

            <div>
              <p className="text-gray-600">Department</p>
              <span className="inline-block bg-blue-100 text-blue-700 px-3 py-1 rounded-lg text-sm font-medium">
                {reviewer.department}
              </span>
            </div>
          </div>
        </div>

        {/* Assigned Job Openings */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Assigned Job Openings</h2>

          <div className="space-y-4">
            {reviewer.assignedJobOpenings.length === 0 ? (
              <p className="text-gray-600">No job openings assigned.</p>
            ) : (
              reviewer.assignedJobOpenings.map((job) => (
                <div
                  key={job.jobOpeningId}
                  className="border rounded-xl p-4 shadow-sm bg-white"
                >
                  <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-3">
                    <div>
                      <h3 className="text-xl font-semibold">{job.title}</h3>
                      <p className="text-gray-600">
                        Department: {job.department}
                      </p>
                      <p className="text-gray-600">
                        Experience: {job.experience}
                      </p>
                      <p className="text-gray-600">
                        Created: {new Date(job.createdAt).toLocaleDateString()}
                      </p>
                    </div>

                    <div className="flex flex-col items-start md:items-end gap-2">
                      <span className="bg-green-100 text-green-700 px-3 py-1 rounded-lg text-sm font-medium">
                        {job.status}
                      </span>
                      <p className="text-gray-700 text-sm">
                        Candidates: {job.candidateCount}
                      </p>
                      <p className="text-gray-700 text-sm">
                        Reviewer: {job.reviewerCount}
                      </p>
                      <p className="text-gray-700 text-sm">
                        Type: {job.jobType}
                      </p>

                      {/* Show Button */}
                      <button
                        className="px-4 py-1 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700"
                        onClick={() => {
                          navigate(`/job-openings/${job.jobOpeningId}`);
                        }}
                      >
                        Show
                      </button>
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
