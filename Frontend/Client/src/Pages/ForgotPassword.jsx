import React, { useState } from "react";
import axios from "axios";
import {useNavigate} from "react-router-dom";

const API_BASE = "http://localhost:5233/api/ForgotPassword";

export default function ForgotPassword() {
  const [step, setStep] = useState(1);
  const [email, setEmail] = useState("");
  const [otp, setOtp] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const navigate = useNavigate();

  const handleSendOtp = async () => {
    if (!email) return alert("Please enter email");

    try {
      await axios.post(`${API_BASE}/forgot-password`, email, {
        headers: { "Content-Type": "application/json" },
      });
      alert("OTP sent to your email");
      setStep(2);
    } catch (err) {
      alert(err.response?.data?.message || "Something went wrong");
    }
  };

  const handleResetPassword = async () => {
    if (newPassword !== confirmPassword)
      return alert("Passwords do not match!");

    const payload = {
      email,
      otp,
      newPassword,
    };

    try {
      await axios.post(`${API_BASE}/reset-password`, payload);
      alert("Password reset successful. Redirecting...");
      navigate("/login");
    } catch (err) {
      alert(err.response?.data?.message || "Error resetting password");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <div className="bg-white p-8 rounded-xl shadow-md w-full max-w-md">
        <h2 className="text-2xl font-bold text-center mb-6">
          Forgot Password
        </h2>

        {step === 1 && (
          <>
            <label className="block mb-1 font-medium">Email</label>
            <input
              type="email"
              className="w-full p-3 border rounded-lg mb-4 focus:ring-2 focus:ring-blue-500"
              placeholder="Enter your email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />

            <button
              onClick={handleSendOtp}
              className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 font-medium"
            >
              Send OTP
            </button>
          </>
        )}

        {step === 2 && (
          <>
            <label className="block mb-1 font-medium">Email</label>
            <input
              type="email"
              disabled
              className="w-full p-3 border rounded-lg mb-4 bg-gray-100"
              value={email}
            />

            <label className="block mb-1 font-medium">OTP</label>
            <input
              type="text"
              placeholder="Enter OTP"
              className="w-full p-3 border rounded-lg mb-4 focus:ring-2 focus:ring-blue-500"
              value={otp}
              onChange={(e) => setOtp(e.target.value)}
            />

            <label className="block mb-1 font-medium">New Password</label>
            <input
              type="password"
              placeholder="New Password"
              className="w-full p-3 border rounded-lg mb-4 focus:ring-2 focus:ring-blue-500"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />

            <label className="block mb-1 font-medium">Confirm Password</label>
            <input
              type="password"
              placeholder="Confirm Password"
              className="w-full p-3 border rounded-lg mb-4 focus:ring-2 focus:ring-blue-500"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
            />

            <button
              onClick={handleResetPassword}
              className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 font-medium"
            >
              Reset Password
            </button>
          </>
        )}
      </div>
    </div>
  );
}
