export default function DataInTable({ finalJson = [] }) {
  return (
    <div className="mt-4 overflow-x-auto">
      <table className="min-w-full border border-gray-300 rounded">
        <thead>
          <tr>
            <th className="p-2 border">#</th>
            <th className="p-2 border">Full Name</th>
            <th className="p-2 border">Email</th>
            <th className="p-2 border">Phone</th>
            <th className="p-2 border">ResumePath</th>
            <th className="p-2 border">Domain</th>
            <th className="p-2 border">DomainExperienceYears</th>
            <th className="p-2 border">LinkedIn</th>
            <th className="p-2 border">GitHub</th>
            <th className="p-2 border">Educations</th>
            <th className="p-2 border">Skills</th>
          </tr>
        </thead>

        <tbody>
          {finalJson?.map((candidate, index) => (
            <tr
              key={index}
              className={`hover:bg-gray-100 ${
                candidate?.Succeeded === 0 ? "bg-yellow-200 text-yellow-900" : ""
              }`}
            >
              <td className={`p-2 border text-center `}>{index + 1}</td>

              <td className="p-2 border">{candidate.FullName}</td>
              <td className="p-2 border">{candidate.Email}</td>
              <td className="p-2 border">{candidate.PhoneNumber}</td>
              <td className="p-2 border">{candidate.ResumePath}</td>
              <td className="p-2 border">{candidate.Domain}</td>
              <td className="p-2 border">{candidate.DomainExperienceYears}</td>
              <td className="p-2 border">
                {candidate.LinkedInProfile ? (
                  <a
                    href={`https://${candidate.LinkedInProfile}`}
                    target="_blank"
                    rel="noreferrer"
                    className="text-blue-600 underline"
                  >
                    Profile
                  </a>
                ) : (
                  "—"
                )}
              </td>

              <td className="p-2 border">
                {candidate.GitHubProfile ? (
                  <a
                    href={`https://${candidate.GitHubProfile}`}
                    target="_blank"
                    rel="noreferrer"
                    className="text-blue-600 underline"
                  >
                    Repo
                  </a>
                ) : (
                  "—"
                )}
              </td>

              {/* EDUCATION */}
              <td className="p-2 border">
                {candidate.Educations?.map((edu, i) => (
                  <div key={i} className="mb-2">
                    <div className="font-semibold">{edu.Degree}</div>
                    <div className="text-xs text-gray-600">
                      {edu.College} – {edu.University}
                    </div>
                    <div className="text-xs">
                      {edu.PassingYear} | {edu.Percentage}%
                    </div>
                  </div>
                ))}
              </td>

              {/* SKILLS */}
              <td className="p-2 border">
                {candidate.Skills?.map((skill, i) => (
                  <div key={i} className="text-sm">
                    • {skill.Name} ({skill.Experience} yrs)
                  </div>
                ))}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
