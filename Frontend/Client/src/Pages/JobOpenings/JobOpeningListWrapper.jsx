// JobOpeningsListWrapper.jsx
import JobOpeningContextProvider from "../../Context/JobOpeningContext";
import JobOpeningsList from "./JobOpeningsList";

export default function JobOpeningsListWrapper() {
  return (
    <JobOpeningContextProvider>
      <JobOpeningsList />
    </JobOpeningContextProvider>
  );
}
