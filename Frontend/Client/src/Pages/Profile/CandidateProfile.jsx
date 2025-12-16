import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { getCandidateDetails } from "../../Services/CandidateService";
import { getCandidateJobOpenings } from "../../Services/CandidateService";
import { useAuthUserContext } from "../../Context/AuthUserContext";
export default function CandidateProfile({}) {
  let { UserId } = useParams();
  let [candidate, setCandidate] = useState(null);
  let [jobApplications, setJobApplications] = useState([]);
  let [notFound , SetNotFound] = useState(false);
  const navigate = useNavigate();
  const { authUser } = useAuthUserContext();
  useEffect(() => {
    let token = localStorage.getItem("token");
    let fetchCandidate = async () => {
      try{
        let data = await getCandidateDetails(UserId,token);
        setCandidate(data);
      }catch(e){
        alert("detail not found !");
        SetNotFound(true);
      }
    };
    let fetchJobApplications = async () => {
      try{
        let jobs = await getCandidateJobOpenings(UserId,token);
        setJobApplications(jobs);
      }catch(e){
        alert("detail not found of job opening")
      }
    };
    fetchCandidate();
    fetchJobApplications();
  }, [UserId]);

  if (!candidate) {
    return <div>Loading...</div>;
  }
   if( notFound ){
  return <div>Not found...</div>;
}
  return (
    <div className="w-full min-h-screen bg-gray-100 p-6">
      <div className="max-w-4xl mx-auto space-y-6">
        {/* Candidate Profile Card */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Candidate Profile</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p className="text-gray-600">Full Name</p>
              <p className="text-lg font-medium">{candidate?.user?.fullName}</p>
            </div>

            <div>
              <p className="text-gray-600">Email</p>
              <p className="text-lg font-medium">{candidate?.user?.email}</p>
            </div>

            <div>
              <p className="text-gray-600">Phone</p>
              <p className="text-lg font-medium">
                {candidate?.user?.phoneNumber}
              </p>
            </div>

            <div>
              <p className="text-gray-600">Domain</p>
              <span className="inline-block bg-purple-100 text-purple-700 px-3 py-1 rounded-lg text-sm font-medium">
                {candidate?.user?.domain}
              </span>
            </div>

            <div>
              <p className="text-gray-600">Experience</p>
              <p className="text-lg font-medium">
                {candidate?.user?.domainExperienceYears} years
              </p>
            </div>

            <div>
              <p className="text-gray-600">Role</p>
              <p className="text-lg font-medium">{candidate?.user?.roleName}</p>
            </div>
          </div>

          {/* Links */}

          <div className="m-7 space-y-2 space-x-4 ">
            {candidate?.linkedInProfile && (
              <a
                href={candidate?.linkedInProfile}
                target="_blank"
                className="text-white-500 text-md m-6  p-2 rounded-lg bg-gray-500"
              >
                LinkedIn Profile
              </a>
            )}
            {candidate?.gitHubProfile && (
              <a
                href={candidate?.gitHubProfile}
                target="_blank"
                className="text-white-500 text-md m-6  p-2 rounded-lg bg-gray-500"
              >
                GitHub Profile
              </a>
            )}

            {candidate?.resumePath && (
              <div className="flex justify-center mt-8">
                <a
                  href={candidate.resumePath}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="block text-white-500 text-md m-2 p-2 rounded-lg bg-gray-500"
                >
                  Resume
                </a>
              </div>
            )}
            {authUser &&
              authUser.roleName === "Candidate" &&
              authUser.id === candidate.userId && (
                <div>
                  <button
                    className="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-blue-700"
                    onClick={() => navigate(`/candidate/update/${UserId}`)}
                  >
                    Update-Profile
                  </button>
                </div>
              )}
          </div>
        </div>

        {/* Skills Section */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Skills</h2>

          {candidate?.user?.skills?.length === 0 ? (
            <p className="text-gray-500">No skills added.</p>
          ) : (
            <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
              {candidate?.user?.skills?.map((sk, index) => (
                <div
                  key={index}
                  className="border rounded-xl p-3 bg-gray-50 shadow-sm"
                >
                  <p className="font-semibold">{sk?.name}</p>
                  <p className="text-gray-600 text-sm">
                    Experience: {sk?.experience} years
                  </p>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Education Section */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Education</h2>

          {candidate?.educations?.length === 0 ? (
            <p className="text-gray-500">No education details.</p>
          ) : (
            <div className="space-y-4">
              {candidate?.educations?.map((edu, index) => (
                <div
                  key={index}
                  className="border rounded-xl p-4 shadow-sm bg-white"
                >
                  <h3 className="text-lg font-semibold">{edu.degree}</h3>
                  <p className="text-gray-700">University: {edu.university}</p>
                  <p className="text-gray-700">College: {edu.college}</p>
                  <p className="text-gray-700">
                    Passing Year: {edu.passingYear}
                  </p>
                  <p className="text-gray-700">Percentage: {edu.percentage}%</p>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Job Applications Section */}
        <div className="bg-white rounded-2xl shadow p-6">
          <h2 className="text-2xl font-semibold mb-4">Job Applications</h2>

          {jobApplications?.length === 0 ? (
            <p className="text-gray-500">No job applications yet.</p>
          ) : (
            <div className="space-y-4">
              {jobApplications.map((job) => (
                <div
                  key={job.id}
                  className="border rounded-xl p-4 shadow-sm bg-white"
                >
                  <div className="flex justify-between items-center">
                    <div>
                      <h3 className="text-xl font-semibold">{job.jobTitle}</h3>
                      <p className="text-gray-600">Round: {job.roundNumber}</p>
                      <p className="text-gray-600">
                        Job Status: {job.jobStatus}
                      </p>
                      <p className="text-gray-600">My Status: {job.status}</p>
                    </div>

                    <button
                      className="px-4 py-1 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700"
                      onClick={() =>
                        navigate(`/job-openings/${job.jobOpeningId}`)
                      }
                    >
                      View Job
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
