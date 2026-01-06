# RecruitmentProcessManagement
Make recruitment online
# 🧠 Recruitment Process Management System

This project aims to design and implement a **Recruitment Process Management System** that streamlines the entire hiring workflow — from job creation to final candidate selection and onboarding.

---

## 📘 Overview

The system allows companies to manage their recruitment process efficiently by integrating job openings, candidate tracking, interviewer feedback, and document verification — all within one platform.

---

## 🏗️ Project Goals

1. Automate the hiring process — from job posting to offer letter generation.  
2. Provide separate roles for **Admin, Recruiter, Reviewer, Interviewer, and Candidate**.  
3. Enable skill-based evaluation and feedback system.  
4. Simplify document verification and onboarding process.  
5. Provide transparency at every stage of recruitment.

---
## 🔄 Application Workflow

### 1. Job Opening Management
* **Creation:** Recruiters create a `JobOpening` with essential details:
    * **Basic:** Title, Description, Salary Range, Location, Job Type, Education.
    * **Metadata:** Domain, Min Experience, Deadline.
    * **Status:** Default set to "Open".
* **Team Assignment:** During creation, the Recruiter attaches specific **Reviewers** and **Interviewers** to the job.
* **Updates:** Post-creation, the Recruiter can update fields, attach documents, or modify the assigned hiring team.

### 2. Candidate Onboarding
Recruiters add candidates to the system using two methods:
1.  **Manual Registration:** Entry of individual basic info and skills.
2.  **Bulk Excel Import:** Uploading an Excel sheet to create multiple candidate profiles instantly.
* *System Action:* Triggers an email to the candidate upon account creation.
* *Assignment:* Candidates are mapped to specific Job Openings (`JobCandidate` table).

### 3. Review Process
* **Screening:** Assigned Reviewers access the candidate list for a specific job.
* **Action:** Reviewers analyze the Resume/CV, add comments (`ReviewerComment`), and update the status (e.g., *Reviewed*, *Rejected*).

### 4. Interview Loop (Technical & HR)
Reviewers pass candidates to the interview stage.
* **Scheduling:** Interviewer provides a meeting link and time, sending an email invite to the candidate.
* **Feedback:** After the round, the Interviewer logs:
    * `Marks`
    * `Feedback`
    * `IsPassed` status
* **Progression Logic:** The Interviewer decides the next step using specific flags:
    * `IsNextTechnicalRound`: Schedules another technical interview.
    * `IsNextHrRound`: Moves the candidate to the HR round.
    * *If both false:* Candidate is rejected/process closes.

### 5. Selection & Offer
* **History Check:** Before making a decision, the Recruiter views the full **Interview History** (all rounds, marks, and comments).
* **Selection:** Recruiter moves candidate from Shortlisted to Selected.
* **Offer Letter:** Recruiter sends the offer.
* **Candidate Response:**
    * **Accept:** Moves to documentation.
    * **Reject:** Must provide an `OfferRejectionReason`.

### 6. Onboarding
* **Documentation:** Candidate uploads required documents.
* **Verification:** HR verifies documents (`IsDocumentVerified`). If rejected, a reason is logged (`DocumentUnVerificationReason`).
* **Conversion:** Verified candidates are moved to the Employee table.

---
