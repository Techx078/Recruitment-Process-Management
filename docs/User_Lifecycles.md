#  RPMS - User Lifecycles & System Workflow

This document details the operational flow for every user role within the platform. It outlines the "Happy Path" from job creation to employee onboarding, highlighting the logic gates and state transitions.

---

## 1.  Recruiter Lifecycle (The Orchestrator)
The Recruiter owns the end-to-end process. They initialize the data and make the final decisions.

1.  **Job Initialization:**
    * Creates a `JobOpening`.
    * **Critical Step:** Assigns the specific **Hiring Team** (Reviewers and Interviewers) to the job.
2.  **Candidate Onboarding:**
    * **Bulk Import:** Uploads an Excel file. The system parses rows and generates User accounts.
    * **Mapping:** Candidates are automatically linked to the `JobOpening` via the `JobCandidate` table.
3.  **Pipeline Monitoring:**
    * Uses the **Job Dashboard** to track candidates across stages (Applied -> Reviewed -> Interview -> Offer).
4.  **Selection & History:**
    * Views the consolidated **Interview History** (all rounds, marks, comments).
    * Moves candidate status to `Selected` or `Rejected`.
5.  **Offer Management:**
    * Generates and sends the Offer Letter via email.
    * Tracks `OfferExpiryDate` and Candidate response.
6.  **Onboarding:**
    * **Final Action:** Converts the Candidate entity into an Employee record.

---

## 2. 👤 Candidate Lifecycle (The Journey)
The candidate moves through a linear state machine defined by the `JobCandidate` status.

1.  **Entry:** Receives an automated email with login credentials upon registration by Recruiter.
2.  **Profile Setup:** Updates LinkedIn/GitHub links and Resume.
3.  **Screening Phase:** Status = `Applied`. Waits for Reviewer action.
4.  **Interview Loop:**
    * Receives meeting link via email.
    * Status updates to `ScheduledInterview`.
    * Status updates to `WaitForInterview` or `Passed` based on outcome.
5.  **Offer Phase:**
    * Receives Offer Letter.
    * **Action:** Accepts or Rejects. (If Rejected, `OfferRejectionReason` is mandatory).
6.  **Documentation:**
    * If Accepted, uploads required proofs (Degree, ID).
    * Waits for HR Verification.

---

## 3. 📝 Reviewer Lifecycle (The Filter)
The gatekeeper who ensures only quality candidates reach the interview pool.

1.  **Queue Management:** Accesses "Pending Reviews" filtered by assigned Job Openings.
2.  **Assessment:** Reviews the Resume/CV and Skills.
3.  **Decision:**
    * Logs `ReviewerComment`.
    * Updates status: `Reviewed` (Pass) or `Rejected` (Fail).
    * *Result:* Passed candidates automatically appear in the **Interviewer's Technical Pool**.

---

## 4. 🗣️ Interviewer Lifecycle (The Assessor)
This role manages the dynamic interview loop.

1.  **Scheduling:**
    * Picks a candidate from the **Technical Pool** or **HR Pool**.
    * Generates a Meeting Link and sets `ScheduledAt` time.
2.  **Execution:** Conducts the interview.
3.  **Feedback Submission:**
    * Fills the Feedback Form: `Marks`, `Feedback`, `IsPassed`.
4.  **Routing Logic (The Decision Engine):**
    The interviewer determines the candidate's next step using boolean flags:
    * `IsNextTechnicalRound = true`: Sends candidate back to the Technical Pool for another round.
    * `IsNextHrRound = true`: Promotes candidate to the HR Pool.
    * **Both False:** The interview loop ends (Candidate is ready for Final Selection or Rejection).
5. **onBoarding(for hr interviewer):**
    * Verifies uploaded documents (`IsDocumentVerified` = true).
---

## 5. 🛠 Admin Lifecycle
1.  **System Setup:** Creates and manages Recruiter accounts.
2.  **Master Data:** Manages the global list of Skills and Document types.

---

## 📊 Logic Flow Summary

| Stage | Actor | Input | Output / State Change |
| :--- | :--- | :--- | :--- |
| **Creation** | Recruiter | Job Details, Team | `JobOpening` Created |
| **Application** | Recruiter | Excel File | `JobCandidate` Created (Status: Applied) |
| **Screening** | Reviewer | Resume | Status: `Reviewed` or `Rejected` |
| **Interview** | Interviewer | Meeting Link | Status: `ScheduledInterview` |
| **Feedback** | Interviewer | Marks, Flags | Status: `Passed` / Next Round Triggered |
| **Selection** | Recruiter | History View | Status: `Selected` |
| **Offer** | Recruiter | Offer Letter | Status: `OfferSent` |
| **Hiring** | Recruiter | Document Verification | Candidate -> Employee |
