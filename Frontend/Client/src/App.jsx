import "./App.css";
import { Routes, Route } from "react-router-dom";
import Login from "./Pages/Login";
import CandidateRegister from "./Pages/CandidateRegister.jsx";
import Home from "./Pages/Home";
import Navbar from "./Component/Navbar.jsx";
import Footer from "./Component/Footer.jsx";
import OtherRegister from "./Pages/OtherRegister.jsx";
import AuthUserContextProvider from "./Context/AuthUserContext.jsx";
import JobOpeningsList from "./Pages/JobOpenings/JobOpeningsList.jsx";
import JobOpeningContextProvider from "./Context/JobOpeningContext.jsx";
import JobOpeningDetails from "./Pages/JobOpenings/JobOpeningDetails.jsx";
import CreateJobOpening from "./Pages/JobOpenings/CreateJobOpening.jsx";
import JobOpeningsListWrapper from "./Pages/JobOpenings/JobOpeningListWrapper.jsx";
import InterviewerProfile from "./Pages/Profile/InterviewerProfile.jsx";
import InterviewerProtector from "./Protectors/InterviewerProtector.jsx";
import RecruiterProtector from "./Protectors/RecruiterProtector.jsx";
import ReviewerProtector from "./Protectors/ReviewerProtector.jsx";
import ReviewerProfile from "./Pages/Profile/ReviewerProfile.jsx";
import ForgotPassword from "./Pages/ForgotPassword.jsx";
import RecruiterProfile from "./Pages/Profile/RecruiterProfile.jsx";
import EditJobOpening from "./Pages/JobOpenings/EditJobOpening.jsx";

function App() {
  return (
    <AuthUserContextProvider>
      
    <div className="flex flex-col min-h-screen">
      {/* Navbar */}
      <Navbar />

      {/* Main content */}
      <main className="flex-grow pt-16 mt-5">
        
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          
          <Route path="/Candidate-register" element={<CandidateRegister />} />
          <Route path="/Other-register"element={<OtherRegister  />} />
          <Route path="/job-openings" element={<JobOpeningsListWrapper />} />

          <Route path="/job-openings/:id" element={<JobOpeningDetails />} />
          <Route path="/job-openings/Create" element={
            <RecruiterProtector>
              <CreateJobOpening />
            </RecruiterProtector>
            } 
          />
          <Route path="/job-openings/:id/edit" element={
            <RecruiterProtector>
                <EditJobOpening />
            </RecruiterProtector>
          } />
          <Route path = "Recruiter/Profile" element={
            <RecruiterProtector>
                <RecruiterProfile />
            </RecruiterProtector>
          } />
          <Route path="/Interviewer/Profile" element={
              <InterviewerProtector>
                  <InterviewerProfile />
              </InterviewerProtector>
              } 
            />
          <Route path="/Reviewer/Profile" element= {
            <ReviewerProtector>
                <ReviewerProfile />
            </ReviewerProtector>
          } />

          <Route path="/forgot-password" element={<ForgotPassword />} />
        </Routes>
        
      </main>

      {/* Footer */}
      <Footer />
    </div>
      
    </AuthUserContextProvider>
  );
}

export default App;
