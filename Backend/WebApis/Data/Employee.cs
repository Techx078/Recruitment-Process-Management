namespace WebApis.Data
{
    public class Employee
    {
        public int Id {  get; set; }
        public int UserId { get; set; }
        public User User {  get; set; }

        public string Department { get; set; }

        public string Designation { get; set; }

        public string salaryRange { get; set; }
        public String JobType { get; set; }
        public String Location { get; set; }

        public string EmploymentStatus { get; set; }   

        public DateTime? JoiningDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
