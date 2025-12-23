import "./App.css";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { Routes, Route } from "react-router-dom";
import Login from "./Pages/Auth/Login";
import CandidateRegister from "./Pages/Auth/CandidateRegister.jsx";
import Home from "./Pages/Home";
import Navbar from "./Component/Navbar.jsx";
import Footer from "./Component/Footer.jsx";
import OtherRegister from "./Pages/Auth/OtherRegister.jsx";
import AuthUserContextProvider from "./Context/AuthUserContext.jsx";
import JobOpeningDetails from "./Pages/JobOpenings/JobOpeningDetails.jsx";
import CreateJobOpening from "./Pages/JobOpenings/CreateJobOpening.jsx";
import JobOpeningsListWrapper from "./Pages/JobOpenings/JobOpeningListWrapper.jsx";
import InterviewerProfile from "./Pages/Profile/InterviewerProfile.jsx";
import RecruiterProtector from "./Protectors/RecruiterProtector.jsx";
import ReviewerProtector from "./Protectors/ReviewerProtector.jsx";
import ReviewerProfile from "./Pages/Profile/ReviewerProfile.jsx";
import ForgotPassword from "./Pages/Auth/ForgotPassword.jsx";
import RecruiterProfile from "./Pages/Profile/RecruiterProfile.jsx";
import EditJobOpening from "./Pages/JobOpenings/EditJobOpening.jsx";
import CandidateBulkRegister from "./Pages/Auth/CandidateBulkRegister.jsx";
import EditJobReviewers from "./Pages/JobOpenings/EditJobReviewers.jsx";
import EditJobInterviewers from "./Pages/JobOpenings/EditJobInterviewer.jsx";
import EditJobDocument from "./Pages/JobOpenings/EditJobDocument.jsx";
import EditJobSkill from "./Pages/JobOpenings/EditJobSkill.jsx";
import CandidateProfile from "./Pages/Profile/CandidateProfile.jsx";
import CandidateUpdate from "./Pages/Auth/CandidateUpdate.jsx";
import InterviewerProfileProtector from "./Protectors/InterviewerProfileProtector.jsx";
import ReviewerProfileProtector from "./Protectors/ReviewerProfileProtector.jsx";
import RecruiterProfileProtector from "./Protectors/RecruiterProfileProtector.jsx";
import CandidateProfileProtector from "./Protectors/CandidateProfileProtector.jsx";
import CandidateProtector from "./Protectors/CandidateProtector.jsx";
import JobOpeningEditProtector from "./Protectors/JobOpeningEditProtector.jsx";
import PendingReviews from "./Pages/PendingReviews.jsx";
import ReviewCandidate from "./Pages/ReviewCandidate.jsx";
import ReviewerLevelProtector from "./Protectors/ReviewerLevelProtector.jsx";
import TechnicalInterviewPool from "./Pages/TechnicalInterviewPool.jsx";
import InterviewerLevelProtector from "./Protectors/InterviewerLevelProtector.jsx";
import ScheduleInterview from "./Pages/ScheduleInterview.jsx";
import InterviewFeedback from "./Pages/InterviewFeedback.jsx";
import InterviewerAssignedProtector from "./Protectors/InterviewerProtector.jsx";
import HrPool from "./Pages/HrPool.jsx";
import FinalPool from "./Pages/FinalPool.jsx";
import CandidateInterviewHistory from "./Pages/CandidateInterviewHistory.jsx";
import JobCandidateDashboard from "./Pages/JobCandidateDashboard/JobCandidateDashboard.jsx";
function App() {
  return (
    <AuthUserContextProvider>
      <div className="flex flex-col min-h-screen bg-grey-100">
        {/* Navbar */}
        <Navbar />

        {/* Main content */}
        <main className="flex-grow pt-16 mt-5">
          <ToastContainer
            position="top-right"
            autoClose={4000}
            hideProgressBar={false}
          />
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route
              path="/Candidate-register"
              element={
                <RecruiterProtector>
                  <CandidateRegister />
                </RecruiterProtector>
              }
            />
            <Route
              path="/Other-register"
              element={
                <RecruiterProtector>
                  <OtherRegister />
                </RecruiterProtector>
              }
            />
            <Route path="/job-openings" element={<JobOpeningsListWrapper />} />
            <Route
              path="/job-openings/:id"
              element={
                <authUserProtector>
                  <JobOpeningDetails />
                </authUserProtector>
              }
            />
            <Route
              path="/job-openings/Create"
              element={
                <RecruiterProtector>
                  <CreateJobOpening />
                </RecruiterProtector>
              }
            />
            <Route
              path="/job-openings/:id/edit"
              element={
                <JobOpeningEditProtector>
                  <EditJobOpening />
                </JobOpeningEditProtector>
              }
            />
            <Route
              path="/job-openings/:id/editReviewer"
              element={
                <JobOpeningEditProtector>
                  <EditJobReviewers />
                </JobOpeningEditProtector>
              }
            />
            <Route
              path="/job-openings/:id/editInterviewer"
              element={
                <JobOpeningEditProtector>
                  <EditJobInterviewers />
                </JobOpeningEditProtector>
              }
            />
            <Route
              path="/job-openings/:id/editDocument"
              element={
                <JobOpeningEditProtector>
                  <EditJobDocument />
                </JobOpeningEditProtector>
              }
            />
            <Route
              path="/job-openings/:id/editSkill"
              element={
                <JobOpeningEditProtector>
                  <EditJobSkill />
                </JobOpeningEditProtector>
              }
            />
            <Route
              path="/Recruiter/Profile/:UserId"
              element={
                <RecruiterProfileProtector>
                  <RecruiterProfile />
                </RecruiterProfileProtector>
              }
            />
            <Route
              path="/Interviewer/Profile/:UserId"
              element={
                <InterviewerProfileProtector>
                  <InterviewerProfile />
                </InterviewerProfileProtector>
              }
            />
            <Route
              path="/Reviewer/Profile/:UserId"
              element={
                <ReviewerProfileProtector>
                  <ReviewerProfile />
                </ReviewerProfileProtector>
              }
            />
            <Route
              path="/Candidate/Profile/:UserId"
              element={
                <CandidateProfileProtector>
                  <CandidateProfile />
                </CandidateProfileProtector>
              }
            />
            <Route
              path="/Candidate/update/:UserId"
              element={
                <CandidateProtector>
                  <CandidateUpdate />
                </CandidateProtector>
              }
            />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route
              path="/Candidate-bulk-register"
              element={
                <RecruiterProtector>
                  <CandidateBulkRegister />
                </RecruiterProtector>
              }
            />
            <Route
              path="/review/:jobCandidateId"
              element={
                <ReviewerProtector>
                  <ReviewCandidate />
                </ReviewerProtector>
              }
            />
            <Route
              path="/job-openings/:jobOpeningId/pending-reviews"
              element={
                <ReviewerLevelProtector>
                  <PendingReviews />
                </ReviewerLevelProtector>
              }
            />
            <Route
              path="/job-openings/:jobOpeningId/technical-pool"
              element={
                <InterviewerLevelProtector>
                  <TechnicalInterviewPool />
                </InterviewerLevelProtector>
              }
            />
            <Route
              path="/schedule-interview/:jobCandidateId"
              element={
                <InterviewerAssignedProtector>
                  <ScheduleInterview />
                </InterviewerAssignedProtector>
              }
            />
            <Route
              path="/interview-feedback/:jobCandidateId"
              element={
                <InterviewerAssignedProtector>
                  <InterviewFeedback />
                </InterviewerAssignedProtector>
              }
            />
            <Route path="/pool/hr/:jobOpeningId" element={<HrPool />} />
            <Route path="/final-pool/:jobOpeningId" element={<FinalPool />} />
            <Route
              path="/History/:jobCandidateId"
              element={<CandidateInterviewHistory />}
            />
            <Route
              path="/Dashboard/:jobOpeningId"
              element={<JobCandidateDashboard />}
            />
          </Routes>
        </main>

        {/* Footer */}
        <Footer />
      </div>
    </AuthUserContextProvider>
  );
}

export default App;
