# Database Schema Documentation

This document outlines the database entities, their purpose within the recruitment workflow, and their relationships.

## Core Identity & Roles

### User
**Purpose:** The base entity for authentication and authorization. It stores common information for all actors in the system (Recruiters, Candidates, Interviewers, Reviewers).
* **Workflow Role:** Used for login validation (`Email`, `PasswordHash`) and determining system access level via `RoleName`.
* **Relationships:**
    * **1-to-1:** Linked to `Candidate`, `Recruiter`, `Reviewer`, and `Interviewer` tables.
    * **1-to-Many:** Linked to `UserSkills` (tracks general skills associated with a user).

### Recruiter
**Purpose:** Represents the process owner who creates jobs and manages the hiring pipeline.
* **Workflow Role:** Creates `JobOpening` entries and manages the final selection/offer process.
* **Relationships:**
    * **Foreign Key:** `UserId` (links to User).
    * **1-to-Many:** `JobOpenings` (jobs created by this recruiter).

### Candidate
**Purpose:** Represents the job seeker. Stores professional profile links and resume paths.
* **Workflow Role:** Registers via Excel import or manual entry. Applied to jobs via the `JobCandidate` table.
* **Relationships:**
    * **Foreign Key:** `UserId` (links to User).
    * **1-to-Many:** `Educations` (academic history).
    * **1-to-Many:** `JobCandidates` (applications to specific jobs).

### Reviewer
**Purpose:** Staff members responsible for the initial screening of applications.
* **Workflow Role:** Assigned to specific jobs to review incoming resumes and provide initial comments.
* **Relationships:**
    * **Foreign Key:** `UserId` (links to User).
    * **Many-to-Many:** Linked to `JobOpening` via the `JobReviewer` table.

### Interviewer
**Purpose:** Technical or HR staff responsible for conducting interviews.
* **Workflow Role:** Assigned to jobs to conduct rounds, provide marks, and submit feedback.
* **Relationships:**
    * **Foreign Key:** `UserId` (links to User).
    * **Many-to-Many:** Linked to `JobOpening` via the `JobInterviewer` table.
    * **1-to-Many:** `JobInterviews` (specific interview sessions conducted).

---

## Job Management

### JobOpening
**Purpose:** The central entity defining a specific vacancy.
* **Workflow Role:** Contains all metadata (Requirements, Salary, Domain) and governs the configuration of the hiring team (Reviewers/Interviewers) and required documents.
* **Relationships:**
    * **Foreign Key:** `CreatedById` (links to Recruiter).
    * **1-to-Many:** `JobCandidates` (pool of applicants).
    * **1-to-Many:** `JobSkills` (required skills for this job).
    * **1-to-Many:** `JobDocuments` (documents required for this job).
    * **1-to-Many:** `JobReviewers` (review team).
    * **1-to-Many:** `JobInterviewers` (interview panel).

### JobSkill
**Purpose:** Associates a specific skill with a job opening.
* **Workflow Role:** Defines if a skill is mandatory (`IsRequired`) and the minimum years of experience needed.
* **Relationships:**
    * **Foreign Keys:** Links `JobOpening` and `Skill`.

### JobReviewer & JobInterviewer
**Purpose:** Junction tables to assign Reviewers and Interviewers to specific Job Openings.
* **Workflow Role:** Allows dynamic team assignment per job rather than global assignment.
* **Relationships:**
    * **Foreign Keys:** Links `JobOpening` to `Reviewer` or `Interviewer`.

---

## Application Workflow (The Core Process)

### JobCandidate
**Purpose:** Represents a unique application. This is the primary state machine for the candidate's progress.
* **Workflow Role:**
    * Tracks status (`Status` e.g., "Applied", "Shortlisted").
    * Controls flow via boolean flags: `IsNextTechnicalRound`, `IsNextHrRound`.
    * Manages offer lifecycle: `OfferExpiryDate`, `OfferRejectionReason`.
    * Manages onboarding: `IsDocumentVerified`, `DocumentUnVerificationReason`.
* **Relationships:**
    * **Foreign Keys:** Links `JobOpening` and `Candidate`.
    * **1-to-Many:** `JobInterviews` (history of rounds for this application).
    * **1-to-Many:** `JobCandidateDocument` (documents uploaded for this application).
    * **Foreign Key:** `ReviewerId` (the specific reviewer who screened this application).

### JobInterview
**Purpose:** Records the details of a specific interview round.
* **Workflow Role:** Stores scheduling data (`MeetingLink`, `ScheduledAt`) and outcome data (`Marks`, `Feedback`, `IsPassed`).
* **Relationships:**
    * **Foreign Keys:** Links `JobCandidate` (the application) and `Interviewer` (the conductor).

---

## Documents & Metadata

### Document
**Purpose:** A master list of document types (e.g., "Passport", "Degree Certificate", "Tax ID").
* **Workflow Role:** Used to define what *types* of documents exist in the system.
* **Relationships:**
    * **1-to-Many:** `JobDocuments` (definitions used in specific jobs).

### JobDocument
**Purpose:** Specifies that a `JobOpening` requires a specific `Document` type.
* **Workflow Role:** Configures the compliance requirements for a job.
* **Relationships:**
    * **Foreign Keys:** Links `JobOpening` and `Document`.

### JobCandidateDocument
**Purpose:** The actual file upload record.
* **Workflow Role:** Links the requirement (`JobDocument`) to the applicant (`JobCandidate`). Stores the URL to the uploaded file.
* **Relationships:**
    * **Foreign Keys:** Links `JobCandidate` and `JobDocument`.

### Skill & UserSkill
**Purpose:** `Skill` is the master dictionary of skills. `UserSkill` maps a user to a skill with proficiency levels.
* **Workflow Role:** Used for profile building and matching candidates to `JobSkill` requirements.
* **Relationships:**
    * `UserSkill` links `User` and `Skill`.

### Education
**Purpose:** Stores academic history for a candidate.
* **Relationships:**
    * **Foreign Key:** `CandidateId`.

### PasswordReset
**Purpose:** Temporary storage for OTPs during the password recovery process.
* **Workflow Role:** Independent utility table for security.
