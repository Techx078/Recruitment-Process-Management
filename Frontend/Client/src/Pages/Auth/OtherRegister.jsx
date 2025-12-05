import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { registerOtherUser } from "../../Services/authService";

export default function OtherRegister() {
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    password: "",
    roleName: "",
    Department: "",
  });
  const [skills, setSkills] = useState([{ name: "", experience: "" }]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const navigateTo = useNavigate();
  const DEPARTMENT_OPTIONS = [
    "IT",
    "HR",
    "Finance",
    "Marketing",
    "Sales",
    "Operations",
    "Administration",
    "Support",
  ];

  // Handle skill input changes
  const handleSkillChange = (index, field, value) => {
    const updatedSkills = [...skills];
    updatedSkills[index][field] = value;
    setSkills(updatedSkills);
  };

  // Add new skill input fields
  const addSkillField = () => {
    setSkills([...skills, { name: "", experience: "" }]);
  };

  // Handle input field changes
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  // Handle form submission for registration
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      // Prepare User data for registration
      const UserData = {
        ...formData,
        skills: skills.map((skill) => ({
          name: skill.name,
          experience: skill.experience,
        })),
      };
      console.log(UserData);
      const token = localStorage.getItem("token");
      if (!token) {
        alert("Login as a recruiter");
        navigateTo("/login");
      }
      const response = await registerOtherUser(UserData, token);
      alert("registered successfully!");

      // Reset form
      setFormData({
        fullName: "",
        email: "",
        phoneNumber: "",
        password: "",
        roleName: "",
        Department: "",
      });
      setSkills([{ name: "", experience: "" }]);
      navigateTo("/Recruiter/Profile");
    } catch (err) {
      console.log(err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100 p-4">
      <div className="w-full max-w-2xl bg-white rounded-2xl shadow-lg p-8">
        <h2 className="text-3xl font-semibold text-center mb-6 text-gray-800">
          Recruiter/reviewer/Interviewer Registration
        </h2>

        <form
          onSubmit={handleSubmit}
          className="grid grid-cols-1 md:grid-cols-2 gap-5"
        >
          {/* Full Name */}
          <div>
            <label className="block text-gray-700 mb-1">Full Name</label>
            <input
              type="text"
              name="fullName"
              value={formData.fullName}
              onChange={handleChange}
              placeholder="Enter your full name"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Email */}
          <div>
            <label className="block text-gray-700 mb-1">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="Enter your email"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Phone */}
          <div>
            <label className="block text-gray-700 mb-1">Phone Number</label>
            <input
              type="Number"
              name="phoneNumber"
              value={formData.phoneNumber}
              onChange={handleChange}
              placeholder="Enter phone number"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Password */}
          <div>
            <label className="block text-gray-700 mb-1">Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Enter password"
              required
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Role-Name */}
          <div>
            <label className="block text-gray-700 mb-1">Role-Name</label>
            <select
              name="roleName"
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500 mb-2"
              value={formData.roleName || ""}
              onChange={handleChange}
            >
              <option value="" disabled>
                Select Role
              </option>
              <option value="Recruiter">Recruiter</option>
              <option value="Reviewer">Reviewer</option>
              <option value="Interviewer">Interviewer</option>
            </select>
          </div>

          {/* Department */}
          <div>
            <label className="block text-gray-700 mb-1">Department</label>
            <select
              name="Department"
              value={formData.Department}
              onChange={handleChange}
              className="w-full border border-gray-300 rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Select Department</option>

              {DEPARTMENT_OPTIONS.map((dept, idx) => (
                <option key={idx} value={dept}>
                  {dept}
                </option>
              ))}
            </select>
          </div>
          <div className="col-span-2">
            <label className="block text-gray-700 mb-2">Skills</label>

            {skills.map((skill, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <input
                  type="text"
                  placeholder="Skill Name (e.g. React)"
                  value={skill.name}
                  onChange={(e) =>
                    handleSkillChange(index, "name", e.target.value)
                  }
                  className="w-1/2 border border-gray-300 rounded-lg p-2"
                />
                <input
                  type="number"
                  placeholder="Experience (in years)"
                  value={skill.experience}
                  onChange={(e) =>
                    handleSkillChange(index, "experience", e.target.value)
                  }
                  className="w-1/2 border border-gray-300 rounded-lg p-2"
                />
              </div>
            ))}

            <button
              type="button"
              onClick={addSkillField}
              className="text-blue-600 text-sm mt-1"
            >
              + Add Another Skill
            </button>
          </div>

          {/* Submit */}
          <div className="col-span-2 mt-4">
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-all"
            >
              {loading ? "Registering..." : "Register"}
            </button>
          </div>
        </form>

        {error && <p className="text-red-600 text-center mt-3">{error}</p>}

        <p className="text-center text-sm text-gray-600 mt-4">
          Already have an account?{" "}
          <Link
            to="/login"
            className="text-blue-600 hover:underline font-medium"
          >
            Login
          </Link>
        </p>
      </div>
    </div>
  );
}
