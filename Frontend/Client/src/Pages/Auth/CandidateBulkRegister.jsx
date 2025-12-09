import React, { useState } from "react";
import * as XLSX from "xlsx";
import DataInTable from "../../Component/DataInTable";
import { CandidateBulkRegisterService } from "../../Services/authService";
import { Navigate } from "react-router-dom";

export default function CandidateBulkRegister() {
  const [basicData, setBasicData] = useState([]);
  const [educationData, setEducationData] = useState([]);
  const [skillsData, setSkillsData] = useState([]);
  const [finalJson, setFinalJson] = useState([]);

  const handleExcelUpload = (e, type) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    reader.onload = (event) => {
      const binaryStr = event.target.result;
      const workbook = XLSX.read(binaryStr, { type: "binary" });
      const sheetName = workbook.SheetNames[0];
      const sheetData = XLSX.utils.sheet_to_json(workbook.Sheets[sheetName]);

      if (type === "basic") setBasicData(sheetData);
      if (type === "education") setEducationData(sheetData);
      if (type === "skills") setSkillsData(sheetData);
    };

    reader.readAsArrayBuffer(file);
  };

  const mergeJsonData = () => {
    const merged = basicData.map((basic) => {
      const userEmail = basic.Email?.trim().toLowerCase();

      const educations = educationData
        .filter((e) => e.Email?.trim().toLowerCase() === userEmail)
        .map((e) => ({
          Degree: e.Degree,
          University: e.University,
          College: e.College,
          PassingYear: Number(e.PassingYear),
          Percentage: Number(e.Percentage),
        }));

      const skills = skillsData
        .filter((s) => s.Email?.trim().toLowerCase() === userEmail)
        .map((s) => ({
          Name: s.Name,
          Experience: Number(s.Experience),
        }));

      return {
        FullName: basic.FullName,
        Email: basic.Email,
        PhoneNumber: basic.PhoneNumber,
        Password: basic.Password,
        ResumePath: basic.ResumePath,
        Domain : basic.Domain,
        DomainExperienceYears : basic.DomainExperienceYears,
        RoleName: "Candidate",
        LinkedInProfile: basic.LinkedInProfile || null,
        GitHubProfile: basic.GitHubProfile || null,
        Educations: educations,
        Skills: skills,
      };
    });

    setFinalJson(merged);
  };

  const sendToBackend = async () => {
    try {
      let token = localStorage.getItem("token");
      if (!token) {
        alert("login as a recruiter!");
        Navigate("/login");
        return;
      }
      //register candidates
      const response = await CandidateBulkRegisterService(finalJson, token);

      alert("Bulk candidates created successfully!");

      //show red box of user which are already exits
      const emailSet = new Set(
        response.skipped.map((e) => e.email.toLowerCase())
      );
      //add succeeded field based on response
      const updatedData = finalJson.map((item) => ({
        ...item,
        Succeeded: emailSet.has(item.Email.toLowerCase()) ? 0 : 1,
      }));

      setFinalJson(updatedData);
    } catch (err) {
      console.error(err);
      alert("Failed to upload bulk candidates!");
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-6">
      <h1 className="text-3xl font-bold mb-5">Bulk Candidate Upload</h1>

      <div className="mb-6 p-4 border rounded-lg bg-gray-100">
        <h2 className="font-semibold mb-2"> Required Excel Structure</h2>

        <pre className="text-sm bg-white p-3 rounded border mb-3">
          BasicDetails.xlsx: FullName | Email | PhoneNumber | Password |
          LinkedInProfile | GitHubProfile{" "} 
          <div>| ResumePath(google drive links) | Domain | DomainExperienceYears |</div>
        </pre>

        <pre className="text-sm bg-white p-3 rounded border mb-3">
          Educations.xlsx: Email | Degree | University | College | PassingYear |
          Percentage(in integer)
        </pre>

        <pre className="text-sm bg-white p-3 rounded border mb-3">
          Skills.xlsx: Email | Name | Experience(in decimal)
        </pre>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div>
          <label className="font-semibold">Upload Basic Details *</label>
          <input
            type="file"
            accept=".xlsx,.xls"
            onChange={(e) => handleExcelUpload(e, "basic")}
            className="block mt-2 rounded border p-3 m-4 w-[250px] "
          />
        </div>

        <div>
          <label className="font-semibold">Upload Education Sheet *</label>
          <input
            type="file"
            accept=".xlsx,.xls"
            onChange={(e) => handleExcelUpload(e, "education")}
            className="block mt-2 rounded border p-3 w-[250px]"
          />
        </div>

        <div>
          <label className="font-semibold">Upload Skills Sheet *</label>
          <input
            type="file"
            accept=".xlsx,.xls"
            onChange={(e) => handleExcelUpload(e, "skills")}
            className="block mt-2 rounded border p-3 w-[250px]"
          />
        </div>
      </div>

      <button
        onClick={mergeJsonData}
        className="mt-6 bg-gray-800 text-white px-4 py-2 rounded hover:bg-black"
      >
        Merge & Create Final JSON
      </button>
      <p>*Preview once merged</p>
      <p>*scroll down to create candidates in bulk</p>

      {finalJson.length > 0 && (
        <>
          <DataInTable finalJson={finalJson} />
          <button
            onClick={sendToBackend}
            className="mt-4 bg-gray-800 text-white px-4 py-2 rounded hover:bg-black"
          >
            Create candidates
          </button>
          <p className="text-md text-gray-600 mb-2">
            Note:Highlighted rows indicate candidates that already existed and were
            not created.
          </p>
        </>
      )}
    </div>
  );
}
