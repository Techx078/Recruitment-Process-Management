# Recruitment Process Management System (RPMS) - Frontend

The client-side application for the RPMS platform. Built with React, it manages the complex workflows between Recruiters, Candidates, Reviewers, and Interviewers using role-based routing and secure state management.

## 🛠 Tech Stack
* **Core:** React.js (Vite/CRA), React Router DOM v6
* **Styling:** Tailwind CSS
* **State Management:** Context API (`AuthUserContext`)
* **Notifications:** React Toastify
* **Security:** High-Order Components (HOCs) for Route Protection

---

## 📸 Application Tour

### 1. Authentication & Onboarding
The entry point for all users.
* **Login:** Universal login for all roles.
* **Candidate Register:** Public registration for job seekers.
* **Bulk Register:** A dedicated tool for Recruiters to upload Excel sheets for mass candidate onboarding.
* **Staff Registration:** Admin/Recruiter interface to onboard new Interviewers or Reviewers.

![Login and Registration Screens](./assets/auth-screens.png)

### 2. Dashboard & Job Management (Recruiter)
The command center for the hiring team.
* **Job Openings List:** View active, closed, and draft jobs.
* **Create Job:** A multi-step form to define requirements, salary, and team assignments.
* **Job Candidate Dashboard:** A visual Kanban-style board to track candidate progress for a specific job.

![Job Dashboard](./assets/recruiter-dashboard.png)

### 3. Job Configuration
Recruiters can granularly edit active jobs.
* **Edit Details:** Update description or salary.
* **Team Management:** Add/Remove Reviewers and Interviewers dynamically.
* **Requirements:** Configure mandatory Skills and Documents.

![Job Editing Interface](./assets/job-edit.png)

### 4. The Review Workflow
For users with the **Reviewer** role.
* **Pending Reviews:** A list of applicants waiting for initial screening.
* **Review Interface:** View Candidate Resume/CV and submit "Accept/Reject" decisions with comments.

![Reviewer Interface](./assets/reviewer-screen.png)

### 5. Interview Management
For users with the **Interviewer** role.
* **Technical Pool:** Candidates waiting for technical rounds.
* **Schedule Interview:** Send meeting invites directly to candidates.
* **Feedback Form:** Submit marks, feedback, and pass/fail status. This form controls the `IsNextTechnicalRound` flags.

![Interview Scheduling and Feedback](./assets/interview-process.png)

### 6. Selection & Offers
The final stage controlled by the **Recruiter**.
* **Interview History:** A detailed timeline of every round, mark, and comment a candidate has received.
* **Final Pool:** Candidates who have cleared all rounds.
* **Send Offer:** Generate and send offer letters.
* **Offer Pool:** Track Accepted, Rejected, and Pending offers.

![Offer Management](./assets/offer-management.png)

---

## 🛡️ Route Security (Protectors)

We use custom wrapper components to secure routes based on user roles. If a user tries to access a route they are not authorized for, they are redirected.

| Protector Component | Function |
| :--- | :--- |
| `RecruiterProtector` | Locks routes strictly for Recruiter roles (e.g., Creating Jobs). |
| `ReviewerProtector` | Ensures only Reviewers can access screening pages. |
| `InterviewerAssignedProtector` | **Dynamic Security:** Checks if the logged-in Interviewer is actually assigned to the specific candidate/job before allowing feedback entry. |
| `JobOpeningEditProtector` | Prevents editing of closed or restricted jobs. |
| `CandidateProtector` | Ensures candidates can only view/edit their own data. |

---

## 🚀 Getting Started

1.  **Install Dependencies**
    ```bash
    npm install
    ```

2.  **Run Development Server**
    ```bash
    npm run dev
    ```

3.  **Build for Production**
    ```bash
    npm run build
    ```

---

## 📂 Project Structure

```bash
/src
  /Components    # Reusable UI (Navbar, Footer, Inputs)
  /Context       # AuthUserContext (Global State)
  /Pages
    /Auth        # Login, Register, ForgotPassword
    /JobOpenings # CRUD operations for Jobs
    /JobCandidate
       /Pools    # Tables for different stages (Tech, HR, Final)
       /Feedback # Forms for Reviews and Interviews
    /Profile     # User Profile Views
  /Protectors    # Security Wrappers
