namespace WebApis.Data
{
    public class Education
    {
            public int Id { get; set; }
            public string Degree { get; set; }                 // e.g. B.Tech, M.Tech
            public string University { get; set; }             // University name
            public string College { get; set; }                // College / Institute name
            public int PassingYear { get; set; }               // e.g. 2022
            public decimal Percentage { get; set; }            // e.g. 78.5

            public int CandidateId { get; set; }               // FK
            public Candidate Candidate { get; set; }
        }
    }



