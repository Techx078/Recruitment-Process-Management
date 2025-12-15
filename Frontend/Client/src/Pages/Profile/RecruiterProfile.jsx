import { useState, useEffect } from "react";
import { useAuthUserContext } from "../../Context/AuthUserContext";
import { fetchRecruiterService } from "../../Services/RecruiterService";
import { useNavigate, Link, useParams } from "react-router-dom";

export default function RecruiterProfile() {
  let { UserId } = useParams();
  const [recruiter, setRecruiter] = useState(null);
  let [notFound , SetNotFound] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchRecruiter = async () => {
      try {
        const data = await fetchRecruiterService(
          localStorage.getItem("token"),
          UserId
        );
        setRecruiter(data);
      } catch (e) {
        console.log("recruiter not found");
        SetNotFound(true)
      }
    };

    fetchRecruiter();
  }, []);

  if (!recruiter) {
    return <div>Loading...</div>;
  }
  if( notFound ){
    return <div>Not found...</div>;
  }
  return (
    <div className="w-full min-h-screen bg-gray-100 p-6">
      <div className="max-w-4xl mx-auto space-y-6">
        {/* Profile Card */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Recruiter Profile</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p className="text-gray-600">Full Name</p>
              <p className="text-lg font-medium">{recruiter.user.fullName}</p>
            </div>

            <div>
              <p className="text-gray-600">Email</p>
              <p className="text-lg font-medium">{recruiter.user.email}</p>
            </div>

            <div>
              <p className="text-gray-600">Phone</p>
              <p className="text-lg font-medium">
                {recruiter.user.phoneNumber}
              </p>
            </div>

            <div>
              <p className="text-gray-600">Department</p>
              <span className="inline-block bg-blue-100 text-blue-700 px-3 py-1 rounded-lg text-sm font-medium">
                {recruiter.department}
              </span>
            </div>

            <button
              className="px-4 bg-gray-800 py-2  text-white rounded-lg hover:bg-black"
              onClick={() => navigate("/Candidate-register")}
            >
              +Register Candidate
            </button>
            <button
              className="px-4 bg-gray-800 py-2  text-white rounded-lg hover:bg-black"
              onClick={() => navigate("/Other-register")}
            >
              +Other-Register(Reviewer , Interviewer)
            </button>
            <button
              className="px-4 bg-gray-800 py-2  text-white rounded-lg hover:bg-black"
              onClick={() => navigate("/Candidate-bulk-register")}
            >
              +Create candidate in bulk
            </button>
          </div>
        </div>

        {/* Created Job Openings */}
        <div className="bg-white rounded-2xl shadow p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-semibold">Created Job Openings</h2>

            {/* Create Job Button */}
            <button
              className="px-4 py-2 bg-gray-800 text-white rounded-lg hover:bg-black text-sm sm:text-base w-full sm:w-auto"
              onClick={() => navigate("/job-openings/create")}
            >
              + Create Job Opening
            </button>
          </div>

          <div className="space-y-4">
            {recruiter.assignedJobOpenings.length === 0 ? (
              <p className="text-gray-500">No job openings created yet.</p>
            ) : (
              recruiter.assignedJobOpenings.map((job) => (
                <div
                  key={job.jobOpeningId}
                  className="border rounded-xl p-4 shadow-sm bg-white"
                >
                  <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-3">
                    <div>
                      <h3 className="text-lg sm:text-xl font-semibold">
                        {job.title}
                      </h3>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Department: {job.department}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Domain: {job.domain}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Experience: {job.minDomainExperience}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-600">
                        Created: {new Date(job.createdAt).toLocaleDateString()}
                      </p>
                    </div>
                    <div className="flex flex-col items-start md:items-end gap-1">
                      <span className="bg-green-100 text-green-700 px-3 py-1 rounded-lg text-xs sm:text-sm font-medium">
                        {job.status}
                      </span>

                      <p className="text-xs sm:text-sm text-gray-700">
                        Candidates: {job.candidateCount}
                      </p>

                      <p className="text-xs sm:text-sm text-gray-700">
                        Type: {job.jobType}
                      </p>
                    </div>
                  </div>

                  {/* ACTION BUTTONS */}
                  <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-2 mt-4">
                    <button
                      className="px-3 py-2 bg-black text-white text-xs sm:text-sm rounded-lg hover:bg-gray-800"
                      onClick={() => navigate(`/job-openings/${job.jobOpeningId}`)}
                    >
                      Show
                    </button>

                    <button
                      className="px-3 py-2 bg-black text-white text-xs sm:text-sm rounded-lg hover:bg-gray-800"
                      onClick={() => navigate(`/job-openings/${job.jobOpeningId}/edit`)}
                    >
                      Update fields
                    </button>

                    <button
                      className="px-3 py-2 bg-black text-white text-xs sm:text-sm rounded-lg hover:bg-gray-800"
                      onClick={() =>
                        navigate(`/job-openings/${job.jobOpeningId}/editReviewer`)
                      }
                    >
                      Update Reviewer
                    </button>

                    <button
                      className="px-3 py-2 bg-black text-white text-xs sm:text-sm rounded-lg hover:bg-gray-800"
                      onClick={() =>
                        navigate(`/job-openings/${job.jobOpeningId}/editInterviewer`)
                      }
                    >
                      Update Interviewer
                    </button>

                    <button
                      className="px-3 py-2 bg-black text-white text-xs sm:text-sm rounded-lg hover:bg-gray-800"
                      onClick={() =>
                        navigate(`/job-openings/${job.jobOpeningId}/editDocument`)
                      }
                    >
                      Update Document
                    </button>
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
