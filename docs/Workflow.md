# ⚙️ Workflow – Recruitment Process Management System

This document explains the **complete workflow** of the Recruitment Process Management System (RPMS), covering every step from job creation to final onboarding.

---

## 🧭 Overview

The system streamlines the entire recruitment process for a company into a single, connected workflow involving multiple user roles:

- **Admin / Super Admin**
- **Recruiter**
- **Reviewer**
- **Interviewer**
- **HR**
- **Candidate**

Each role has distinct permissions and responsibilities managed through **Role-Based Access Control (RBAC)**.

---

## 🧱 1. Job Creation & Position Management

### Actors
- **Recruiter**

### Workflow
1. **Create Job Opening**
   - Recruiter creates a new job with details like title, description, required skills, salary, and benefits.
   - A deadline and status (`Open`, `On Hold`, or `Closed`) are defined.
   - The recruiter links one or more **skills** using the `JobSkills` table.

2. **Add Document Requirements**
   - The recruiter specifies required document types (from `DocumentsMaster`).
   - These are stored in `JobDocuments` for that particular job.

3. **Assign Reviewers**
   - One or more reviewers are manually assigned to the job opening.

4. **Job Notification**
   - Once a job is created, all **registered candidates** receive an automated email about the new job.
   - Candidates can log in and apply directly.

---

## 👤 2. Candidate Profile Management

### Actors
- **Recruiter**
- **Candidate**

### Workflow
1. **Candidate Profiles**
   - Recruiters can add candidate profiles in three ways:
     - Manual entry (enter details and upload CV)
     - Bulk upload (via Excel)
     - Automatic profile creation from CV parsing

2. **System Data Bank**
   - All candidate profiles are stored in a centralized database for future reference.

3. **Application to Jobs**
   - Candidates can apply manually through their dashboard.
   - Recruiters can also **link candidates** directly to open jobs.

4. **CV Upload**
   - The CV is **not stored** globally — it is uploaded only at the time of application and stored in `JobCandidates.CVPath`.

---

## 🔍 3. Resume Screening & Shortlisting

### Actors
- **Reviewer**
- **Recruiter**

### Workflow
1. **Reviewer Assignment**
   - Reviewers assigned to the job receive a notification email.
   - They can view all candidates linked to that job.

2. **Candidate Review**
   - Reviewer reviews the CV.
   - They tick the candidate’s skills and mark years of experience (via UI).
   - They add **review comments** stored in `JobCandidates.ReviewerComment`.

3. **Shortlisting**
   - Reviewer changes candidate status to:
     - `Shortlisted` → Moves to interview stage.
     - `Rejected` → Removed from further consideration.

4. **Previous History Check**
   - If the candidate was previously reviewed or interviewed for another job, a system notification alerts the reviewer.

---

## 🎯 4. Interview Scheduling & Process

### Actors
- **Interviewer**
- **Recruiter**
- **Candidate**

### Workflow
1. **Interviewer Assignment**
   - Interviewers are already assigned to each job at creation.
   - They get notified when candidates are shortlisted.

2. **View Candidates**
   - Interviewers see all shortlisted candidates in their dashboard.

3. **Schedule Interview**
   - Interviewer clicks “Schedule Interview”.
   - Enters date, time, and meeting link.
   - Candidate and recruiter receive email notifications.

4. **Conduct Interview**
   - Interview takes place via the provided link.

5. **Feedback & Decision**
   - Interviewer provides:
     - Marks (stored in `JobInterviews.Marks`)
     - Text feedback (`JobInterviews.Feedback`)
     - Decision flags:
       - `RecommendNextRound` – If next technical round required
       - `RecommendHRRound` – If candidate should move to HR
       - `IsRejected` – If rejected

6. **Multiple Rounds Support**
   - Each interview round is stored separately in `JobInterviews`.
   - Final score for a candidate is calculated using **average marks** across rounds.

---

## 💬 5. HR Round & Evaluation

### Actors
- **HR**
- **Candidate**

### Workflow
1. **HR Interview**
   - If `RecommendHRRound` = True, HR is notified automatically.
   - HR schedules and conducts the HR round (soft skills, culture fit, negotiation).

2. **Feedback**
   - HR provides marks and comments, stored in a new entry in `JobInterviews`.

3. **Final Evaluation**
   - System calculates final score using:
     - Average technical round marks
     - HR evaluation marks

4. **Selection**
   - Recruiter reviews candidate list and selects top candidates based on:
     - Merit (highest marks)
     - Or manual choice by recruiter

---

## 📄 6. Document Upload & Verification

### Actors
- **Candidate**
- **HR**

### Workflow
1. **Upload Request**
   - Selected candidates receive an email to upload required documents.
   - The list of required documents comes from `JobDocuments`.

2. **Candidate Upload**
   - Candidates upload files through the portal.
   - Each document is stored in `CandidateDocuments`.

3. **HR Verification**
   - HR reviews uploaded documents.
   - Marks verification status in `CandidateDocuments.IsVerified`.

4. **Completion**
   - Once all required documents are verified:
     - `JobCandidates.Status` → `Verified`
     - Candidate is ready for onboarding.

---

## 👔 7. Final Selection & Onboarding

### Actors
- **HR**
- **Recruiter**
- **Candidate**

### Workflow
1. **Joining Confirmation**
   - HR provides joining date.
   - Record stored in `Employees`.

2. **Offer Letter Generation**
   - System generates a PDF offer letter.
   - Stored at `Employees.OfferLetterPath`.
   - Sent to the candidate via email.

3. **Transition**
   - Candidate’s record is moved from `JobCandidates` to `Employees`.

---

## 🧠 8. Notifications & Status Management

### Triggers
- New Job Created → Email to all candidates
- Reviewer Assigned → Email to reviewer
- Interview Scheduled → Email to candidate and recruiter
- Document Upload Required → Email to candidate
- Offer Letter Generated → Email to candidate

### Status Lifecycle
Applied → Under Review → Shortlisted → Interview Scheduled →
Interviewed → HR Round → Selected → Verified → Joined / Rejected


Each transition updates `JobCandidates.Status` and triggers respective notifications.

---

## 📊 9. Reporting & Analytics

### Actors
- **Admin**
- **HR**
- **Recruiter**

### Key Reports
- Job-wise candidate summary  
- Technology-wise candidate analysis  
- Interviewer feedback summary  
- Experience and college-wise report  
- Date-wise activity logs  
- Document verification report  

---

## 🧾 Notes

- The system is modular — each stage can evolve independently (e.g., AI-based auto-shortlisting later).  
- The workflow supports both **manual** and **automated** decisions.  
- Email and notification services are decoupled from the main logic for scalability.  
- Auditing timestamps (`CreatedAt`, `UpdatedAt`) track all activity.

---
