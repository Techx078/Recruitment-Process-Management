namespace WebApis.Dtos.MailDtos
{
    public class EmployeeCreatedMailDto
    {
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }

        public string Department { get; set; }
        public string Designation { get; set; }
        public DateTime JoiningDate { get; set; }
    }
}
