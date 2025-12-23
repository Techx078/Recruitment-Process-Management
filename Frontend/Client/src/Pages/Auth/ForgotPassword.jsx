import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { resetPassword, sendOtp } from "../../Services/forgotPasswordService";
import { handleGlobalError } from "../../Services/errorHandler";
import { toast } from "react-toastify";

export default function ForgotPassword() {
  const [step, setStep] = useState(1);
  const [email, setEmail] = useState("");
  const [otp, setOtp] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const navigate = useNavigate();

  const handleSendOtp = async () => {
    if (isLoading) return;
    setIsLoading(true);
    if (!email) {
      setIsLoading(false);
      return toast.warning("Please enter email");
    }

    try {
      await sendOtp(email);
      toast.success("OTP sent to your email");
      setStep(2);
    } catch (err) {
      handleGlobalError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleResetPassword = async () => {
    if (isLoading) return;
    setIsLoading(true);
    if (newPassword !== confirmPassword) {
      setIsLoading(false);
      return toast.warning("Passwords do not match!");
    }

    const payload = {
      email,
      otp,
      newPassword,
    };

    try {
      await resetPassword(payload);
      toast.success("Password reset successful. Redirecting...");
      navigate("/login");
    } catch (err) {
      handleGlobalError(err);
    } finally {
      setIsLoading(false);
    }
  };

  if (error) {
    return (
      <div className="max-w-6xl mx-auto mt-10 bg-gray-100 border border-gray-300 p-4 rounded text-gray-700">
        {error}
      </div>
    );
  }
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <div className="bg-white p-8 rounded-xl shadow-md w-full max-w-md">
        <h2 className="text-2xl font-bold text-center mb-6">Forgot Password</h2>

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
              disabled={!email || isLoading}
              className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 font-medium
              disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? "Sending..." : "Send OTP"}
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
              disabled={
                !email || isLoading || !otp || !newPassword || !confirmPassword
              }
              className="w-full bg-green-600 text-white py-3 rounded-lg hover:bg-green-700 font-medium disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? "Resetting..." : "Reset Password"}
            </button>
          </>
        )}
      </div>
    </div>
  );
}
