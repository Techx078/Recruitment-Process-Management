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

function App() {
  return (
    <AuthUserContextProvider>
      <JobOpeningContextProvider>
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
          <Route path="/job-openings" element={<JobOpeningsList />} />
          <Route path="/job-openings/:id" element={<JobOpeningDetails />} />
        </Routes>
        
      </main>

      {/* Footer */}
      <Footer />
    </div>
      </JobOpeningContextProvider>
    </AuthUserContextProvider>
  );
}

export default App;
