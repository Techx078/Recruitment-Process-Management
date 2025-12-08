import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getAllJobOpenings } from "../../Services/JobOpeningService";
import { useJobOpeningContext } from "../../Context/JobOpeningContext";


const JobOpeningsList = () => {
  let { JobOpenings } = useJobOpeningContext();
  const [jobs, setJobs] = useState([]);
  const [filteredJobs, setFilteredJobs] = useState([]);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [JobTypeFilter, setJobTypeFilter] = useState("All");
  const [departmentFilter, setDepartmentFilter] = useState("All");
  const [locationFilter, setLocationFilter] = useState("All");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  //Fetch job openings
  useEffect(() => {
    setJobs(JobOpenings);
    setLoading(false);
  },[JobOpenings]);

  // Handle filters and search
  useEffect(() => { 
    if (!jobs) return;
    let result = [...jobs];

    // Search by title or department
    if (search.trim() !== "") {
      result = result.filter(
        (job) =>
          job.title.toLowerCase().includes(search.toLowerCase()) 
      );
    }

    // Filter by status
    if (statusFilter !== "All") {
      result = result.filter(
        (job) => job.status.toLowerCase() === statusFilter.toLowerCase()
      );
    }

    //filter by job type
    if (JobTypeFilter !== "All") {
      result = result.filter(
        (job) => job.jobType.toLowerCase() === JobTypeFilter.toLowerCase()
      );
    }

    // Filter by department
    if (departmentFilter !== "All") {
      result = result.filter(
        (job) => job.department.toLowerCase() === departmentFilter.toLowerCase()
      );
    }

    //filter by location
    if (locationFilter !== "All") {
      result = result.filter(
        (job) => job.location.toLowerCase() === locationFilter.toLowerCase()
      );
    }
    
    setFilteredJobs(result);
  }, [search, statusFilter,JobTypeFilter,departmentFilter,locationFilter, jobs]);

  if (loading)
    return <div className="text-center text-gray-500 py-10">Loading jobs...</div>;
  if (error)
    return <div className="text-center text-red-500 py-10">{error}</div>;

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      {/* Header section */}
      <div className="max-w-6xl mx-auto flex flex-col sm:flex-row sm:justify-between sm:items-center mb-10 gap-4">
        <h2 className="text-3xl font-semibold text-gray-800">Job Openings</h2>
        <button
          onClick={() => navigate("/create-job")}
          className="bg-indigo-600 hover:bg-indigo-700 text-white font-medium px-5 py-2.5 rounded-lg transition-all"
        >
          + Create New Job
        </button>
      </div>

      {/* Filters */}
      <div className="max-w-6xl mx-auto mb-8 flex flex-col sm:flex-row gap-7 sm:items-center sm:mx-52"> 
        {/* Search bar */}
        <input
          type="text"
          placeholder="Search by title..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="w-full sm:w-1/2 px-4 py-2 border rounded-lg shadow-sm focus:ring-2 focus:ring-indigo-500 focus:outline-none"
        />

        {/* Status dropdown */}
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          className="px-4 py-2 border rounded-lg shadow-sm focus:ring-2 focus:ring-indigo-500 focus:outline-none w-full sm:w-auto"
        >
          <option value="All">All Status</option>
          <option value="Open">Open</option>
          <option value="Closed">Closed</option>
          <option value="OnHold">On Hold</option>
        </select>
      </div>
      
      {/* Job Type Filter */}
      <div className="max-w-6xl mx-auto mb-8 flex flex-col sm:flex-row gap-7 sm:items-center sm:mx-52">
        <select
          value={JobTypeFilter}
          onChange={(e) => setJobTypeFilter(e.target.value)}
          className="px-4 py-2 border rounded-lg shadow-sm focus:ring-2 focus:ring-indigo-500 focus:outline-none w-full sm:w-auto"
        >
          <option value="All">All Job Types</option>
          <option value="FullTime">Full-Time</option>
          <option value="PartTime">Part-Time</option>
          <option value="Contract">Contract</option>
          <option value="Internship">Internship</option>
        </select>
        { /* Department Filter */ }
        <select
          value={departmentFilter}
          onChange={(e) => setDepartmentFilter(e.target.value)}
          className="px-4 py-2 border rounded-lg shadow-sm focus:ring-2 focus:ring-indigo-500 focus:outline-none w-full sm:w-auto"
        >
          <option value="All">All Departments</option>
          <option value="Engineering">Engineering</option>
          <option value="Marketing">Marketing</option>
          <option value="Sales">Sales</option>
          <option value="HumanResources">HR</option>
          <option value="Finance">Finance</option>
          <option value="Operations">Operations</option>
          <option value="Design">Design</option>
          <option value="ITSupport">ITSupport</option>
          <option value="ProductManagement">ProductManagement</option>
        </select>
        {/* Location Filter */}
        <select
          value={locationFilter}
          onChange={(e) => setLocationFilter(e.target.value)}
          className="px-4 py-2 border rounded-lg shadow-sm focus:ring-2 focus:ring-indigo-500 focus:outline-none w-full sm:w-auto"
        >
          <option value="All">All Locations</option>
          <option value="Remote">Remote</option>
          <option value="OnSite">OnSite</option>
          <option value="Hybrid">Hybrid</option>
        </select>
      </div>
      {/* Job cards */}
      {filteredJobs.length === 0 || jobs.length === 0 ? (
        <p className="text-gray-500 text-center mt-10">No job openings found.</p>
      ) : (
        <div className="space-y-6 max-w-4xl mx-auto">
          {filteredJobs.map((job) => (
            <div
              key={job.id}
              className="bg-white shadow-md border border-gray-200 rounded-2xl p-7 flex flex-col justify-between hover:shadow-lg transition-all duration-200 min-h-[240px]"
            >
              {/* Title */}
              <div>
                <h3 className="text-xl font-semibold text-indigo-700 mb-2">
                  {job.title}
                </h3>

                {/* Description */}
                <p className="text-gray-600 text-sm mb-3 line-clamp-3">
                  {job.description}
                </p>

                {/* Info */}
                <div className="flex flex-wrap justify-between items-center text-sm text-gray-500 mb-3">
                  <span>
                    Status:{" "}
                    <span
                      className={`font-medium ${
                        job.status === "Open"
                          ? "text-green-600"
                          : job.status === "Closed"
                          ? "text-red-600"
                          : "text-yellow-600"
                      }`}
                    >
                      {job.status}
                    </span>
                  </span>
                  {job.deadLine && (
                    <span>
                      Deadline :{" "}
                      <span className="font-medium text-gray-700">
                        {new Date(job.deadLine).toLocaleDateString()}
                      </span>
                    </span>
                  )}
                </div>
                <div className="flex flex-wrap justify-between items-center text-sm text-gray-500 mb-3" >
                <p className="text-sm text-gray-700">
                  <span className="font-medium">Salary Range:</span>{" "}
                  {job.salaryRange || "Not Disclosed"}
                </p>
                 <p className="text-sm text-gray-700">
                  <span className="font-medium">Experience:</span>{" "}
                  {job.minDomainExperience || "Not Disclosed"}
                </p>
                <p className="text-sm text-gray-700">
                  <span className="font-medium">Domain:</span>{" "}
                  {job.domain || "Not Disclosed"}
                </p>
                <p className="text-sm text-gray-700">
                  <span className="font-medium">Job Type:</span>{" "}
                  {job.jobType || "Not Disclosed"}
                </p>
                </div>
                <h4 className="flex flex-wrap gap-2 mt-2 text-sm text-gray-700" >
                  Skills 
                </h4>
                <div className="flex flex-wrap gap-2 mt-2">
                  {job.jobSkills && job.jobSkills.length > 0 ? (
                    job.jobSkills.map((jobSkills, index) => (
                      <span
                        key={index}
                        className="bg-indigo-100 text-indigo-800 text-xs font-medium px-2.5 py-0.5 rounded" 
                      >
                        {jobSkills.name}
                      </span>
                    ))
                  ) : (
                    <span className="text-gray-500 text-xs">No skills specified</span>
                  )}
                </div>
              </div>

              {/* Button */}
              <div className="mt-6 flex justify-end">
                <button
                  onClick={() => navigate(`/job-openings/${job.id}`)}
                  className="bg-indigo-600 text-white px-6 py-2 rounded-lg hover:bg-indigo-700 transition-colors text-sm font-medium"
                >
                  View Details
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default JobOpeningsList;
