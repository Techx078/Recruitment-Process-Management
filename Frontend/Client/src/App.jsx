import "./App.css";
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
import InterviewerProtector from "./Protectors/InterviewerProtector.jsx";
import RecruiterProtector from "./Protectors/RecruiterProtector.jsx";
import ReviewerProtector from "./Protectors/ReviewerProtector.jsx";
import ReviewerProfile from "./Pages/Profile/ReviewerProfile.jsx";
import ForgotPassword from "./Pages/Auth/ForgotPassword.jsx";
import RecruiterProfile from "./Pages/Profile/RecruiterProfile.jsx";
import EditJobOpening from "./Pages/JobOpenings/EditJobOpening.jsx";
import CandidateBulkRegister from "./Pages/Auth/CandidateBulkRegister.jsx";
import OrganizationUserProtector from "./Protectors/OrganizationUserProtector.jsx";
import EditJobReviewers from "./Pages/JobOpenings/EditJobReviewers.jsx";
import EditJobInterviewers from "./Pages/JobOpenings/EditJobInterviewer.jsx";
import EditJobDocument from "./Pages/JobOpenings/EditJobDocument.jsx";
import EditJobSkill from "./Pages/JobOpenings/EditJobSkill.jsx";
import CandidateProfile from "./Pages/Profile/CandidateProfile.jsx";
import CandidateUpdate from "./Pages/Auth/CandidateUpdate.jsx";
function App() {
  return (
    <AuthUserContextProvider>
      <div className="flex flex-col min-h-screen bg-grey-100">
        {/* Navbar */}
        <Navbar />

        {/* Main content */}
        <main className="flex-grow pt-16 mt-5">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />

            <Route path="/Candidate-register" element={<CandidateRegister />} />
            <Route path="/Other-register" element={<OtherRegister />} />
            <Route path="/job-openings" element={<JobOpeningsListWrapper />} />

            <Route path="/job-openings/:id" element={<JobOpeningDetails />} />
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
                <RecruiterProtector>
                  <EditJobOpening />
                </RecruiterProtector>
              }
            />
            <Route
              path="/job-openings/:id/editReviewer"
              element={
                <RecruiterProtector>
                  <EditJobReviewers />
                </RecruiterProtector>
              }
            />
             <Route
              path="/job-openings/:id/editInterviewer"
              element={
                <RecruiterProtector>
                  <EditJobInterviewers />
                </RecruiterProtector>
              }
            />
             <Route
              path="/job-openings/:id/editDocument"
              element={
                <RecruiterProtector>
                  <EditJobDocument />
                </RecruiterProtector>
              }
            />
            <Route
              path="/job-openings/:id/editSkill"
              element={
                <RecruiterProtector>
                  <EditJobSkill />
                </RecruiterProtector>
              }
            />
            <Route
              path="/Recruiter/Profile/:UserId"
              element={
                <RecruiterProtector>
                  <RecruiterProfile />
                </RecruiterProtector>
              }
            />
            <Route
              path="/Interviewer/Profile/:UserId"
              element={
                <OrganizationUserProtector>
                  <InterviewerProfile />
                </OrganizationUserProtector>
              }
            />
            <Route
              path="/Reviewer/Profile/:UserId"
              element={
                <OrganizationUserProtector>
                  <ReviewerProfile />
                </OrganizationUserProtector>
              }
            />
             <Route
              path="/Candidate/Profile/:UserId"
              element={
               
                  <CandidateProfile />
               
              }
            />
            <Route
              path="/Candidate/update/:UserId"
              element={
                  <CandidateUpdate />
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
          </Routes>
        </main>

        {/* Footer */}
        <Footer />
      </div>
    </AuthUserContextProvider>
  );
}

export default App;
