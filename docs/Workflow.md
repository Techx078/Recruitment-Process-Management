##  Application Workflow

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

## Key Data Entities

### JobOpening
Contains job metadata, status, requirements, and navigation collections for the hiring team (Reviewers/Interviewers) and Candidates.

### JobCandidate
The link between a User and a Job. Tracks:
* **Status:** *Applied, Reviewed, ScheduledInterview, Selected, OfferSent, etc.*
* **Flags:** `IsDocumentUploaded`, `IsDocumentVerified`.
* **Offer Data:** `OfferExpiryDate`, `OfferRejectionReason`.

### JobInterview
Tracks individual interview rounds.
* Stores `MeetingLink`, `RoundNumber`, and `InterviewType` (Tech/HR).
* Captures `Marks` and `Feedback`.

---

## Highlights
* **Bulk Import:** Excel-based candidate registration.
* **Dynamic Workflow:** Interviewers control the flow (Next Round flags) rather than a rigid linear process.
* **Audit Trail:** Recruiters have full visibility of the interview lifecycle before sending offers.
