# 🗃️ Database Schema – Recruitment Process Management System

This document describes the complete **database schema** used in the Recruitment Process Management System.  
It includes all entities, their attributes, relationships, and functional roles in the system.

---

## 🧩 Overview

The database follows a **fully normalized relational structure**, designed to:

- Support multiple user roles (Admin, Recruiter, Reviewer, Interviewer, Candidate)
- Track job openings, candidate applications, interviews, and feedback
- Manage document verification and onboarding
- Ensure data consistency, scalability, and flexibility for future automation

---

## 📋 Entity List

| Entity | Description |
|--------|-------------|
| **Users** | Stores all system users (Recruiters, Candidates, Reviewers, Interviewers, HRs, Admins). |
| **Skills** | Contains a master list of skills used across jobs and candidates. |
| **UserSkills** | Junction table linking users to their skills. |
| **JobOpenings** | Represents an open position created by a recruiter. |
| **JobSkills** | Links required skills to a specific job opening. |
| **JobDocuments** | Lists document requirements for a job opening. |
| **JobCandidates** | Connects candidates to specific job openings and tracks their progress. |
| **JobInterviews** | Tracks interviews, marks, feedback, and round progression. |
| **DocumentsMaster** | Master list of available document types (e.g., Resume, ID Proof). |
| **CandidateDocuments** | Stores uploaded documents per candidate per job. |
| **Employees** | Stores information of hired candidates who joined successfully. |

---

## 🧱 Table Definitions

### 1. **Users**
| Column | Type | Description |
|--------|------|-------------|
| UserId | INT (PK) | Unique ID for each user. |
| Name | VARCHAR(100) | Full name of the user. |
| Email | VARCHAR(100) | Unique email address for login. |
| PasswordHash | VARCHAR(255) | Encrypted password. |
| Role | ENUM('Admin','Recruiter','Reviewer','Interviewer','Candidate','HR') | Defines user type. |
| ContactNumber | VARCHAR(15) | Optional contact info. |
| CreatedAt | DATETIME | Record creation timestamp. |
| IsActive | BIT | Indicates if user is active. |

---

### 2. **Skills**
| Column | Type | Description |
|--------|------|-------------|
| SkillId | INT (PK) | Unique ID of the skill. |
| SkillName | VARCHAR(100) | Name of the skill (e.g., Java, SQL, .NET). |

---

### 3. **UserSkills**
| Column | Type | Description |
|--------|------|-------------|
| UserSkillId | INT (PK) | Unique record ID. |
| UserId | INT (FK → Users.UserId) | Reference to user. |
| SkillId | INT (FK → Skills.SkillId) | Reference to skill. |
| YearsOfExperience | DECIMAL(3,1) | Optional skill experience in years. |

---

### 4. **JobOpenings**
| Column | Type | Description |
|--------|------|-------------|
| JobId | INT (PK) | Unique ID for each job. |
| Title | VARCHAR(150) | Job title. |
| Description | TEXT | Detailed job description. |
| Requirements | TEXT | Technical and non-technical requirements. |
| Salary | VARCHAR(100) | Salary range or package. |
| Benefits | TEXT | Job benefits or perks. |
| CreatedBy | INT (FK → Users.UserId) | Recruiter who created the job. |
| Status | ENUM('Open','On Hold','Closed') | Current job status. |
| Deadline | DATE | Application closing date. |
| CreatedAt | DATETIME | Job creation timestamp. |

---

### 5. **JobSkills**
| Column | Type | Description |
|--------|------|-------------|
| JobSkillId | INT (PK) | Unique record ID. |
| JobId | INT (FK → JobOpenings.JobId) | Related job. |
| SkillId | INT (FK → Skills.SkillId) | Required skill. |

---

### 6. **DocumentsMaster**
| Column | Type | Description |
|--------|------|-------------|
| DocumentTypeId | INT (PK) | Unique document ID. |
| DocumentName | VARCHAR(100) | Example: Resume, Aadhaar, Experience Letter. |

---

### 7. **JobDocuments**
| Column | Type | Description |
|--------|------|-------------|
| JobDocumentId | INT (PK) | Unique record ID. |
| JobId | INT (FK → JobOpenings.JobId) | Related job. |
| DocumentTypeId | INT (FK → DocumentsMaster.DocumentTypeId) | Required document. |

---

### 8. **JobCandidates**
| Column | Type | Description |
|--------|------|-------------|
| JobCandidateId | INT (PK) | Unique record ID. |
| JobId | INT (FK → JobOpenings.JobId) | Related job. |
| CandidateId | INT (FK → Users.UserId) | Candidate linked to job. |
| CVPath | VARCHAR(255) | Path to uploaded CV (if available). |
| ReviewerComment | TEXT | Reviewer’s feedback. |
| Status | ENUM('Applied','Under Review','Shortlisted','Interview Scheduled','Interviewed','HR Round','Selected','Rejected','Verified','Joined') | Candidate progress stage. |
| AssignedReviewerId | INT (FK → Users.UserId, nullable) | Reviewer assigned to candidate. |
| UpdatedAt | DATETIME | Last status update. |

---

### 9. **JobInterviews**
| Column | Type | Description |
|--------|------|-------------|
| InterviewId | INT (PK) | Unique interview record ID. |
| JobCandidateId | INT (FK → JobCandidates.JobCandidateId) | Candidate being interviewed. |
| InterviewerId | INT (FK → Users.UserId) | Interviewer conducting the interview. |
| RoundNumber | INT | Round count (e.g., 1, 2, 3). |
| Feedback | TEXT | Interviewer’s comments. |
| Marks | DECIMAL(5,2) | Score given by interviewer. |
| RecommendNextRound | BIT | True if next technical round required. |
| RecommendHRRound | BIT | True if candidate moves to HR. |
| IsRejected | BIT | True if candidate is rejected. |
| InterviewDate | DATETIME | Date/time of interview. |
| MeetingLink | VARCHAR(255) | URL for interview meeting. |

---

### 10. **CandidateDocuments**
| Column | Type | Description |
|--------|------|-------------|
| CandidateDocumentId | INT (PK) | Unique record ID. |
| JobCandidateId | INT (FK → JobCandidates.JobCandidateId) | Candidate-job relation. |
| DocumentTypeId | INT (FK → DocumentsMaster.DocumentTypeId) | Type of document uploaded. |
| DocumentPath | VARCHAR(255) | File storage path. |
| IsVerified | BIT | Verification flag (set by HR). |
| VerifiedBy | INT (FK → Users.UserId, nullable) | HR who verified. |
| VerifiedAt | DATETIME | Timestamp of verification. |

---

### 11. **Employees**
| Column | Type | Description |
|--------|------|-------------|
| EmployeeId | INT (PK) | Unique employee ID. |
| UserId | INT (FK → Users.UserId) | Linked user record. |
| JobId | INT (FK → JobOpenings.JobId) | Position they joined for. |
| JoiningDate | DATE | Date of joining. |
| OfferLetterPath | VARCHAR(255) | Path to generated offer letter. |
| CreatedAt | DATETIME | Record creation time. |

---

## 🔗 Relationships Summary

- **Users** → one-to-many → **JobOpenings**  
- **Users** → many-to-many → **Skills** (via UserSkills)  
- **JobOpenings** → many-to-many → **Skills** (via JobSkills)  
- **JobOpenings** → one-to-many → **JobCandidates**  
- **JobCandidates** → one-to-many → **JobInterviews**  
- **JobCandidates** → one-to-many → **CandidateDocuments**  
- **JobOpenings** → one-to-many → **JobDocuments**  
- **DocumentsMaster** → one-to-many → **JobDocuments**, **CandidateDocuments**

---

## 📘 Notes

- Recruiters, Reviewers, Interviewers, and HRs are all **user roles** under the same `Users` table.  
- `JobCandidates.Status` defines each candidate’s lifecycle.  
- `JobInterviews` isolates multi-round interview data, allowing flexible round tracking.  
- `CandidateDocuments` supports verification workflows for HRs.  
- All timestamps (`CreatedAt`, `UpdatedAt`) ensure tracking for auditing.

---

## 📄 ER Diagram

Refer to `/docs/ER_Diagram.jpeg` for a visual representation of this schema.

---
