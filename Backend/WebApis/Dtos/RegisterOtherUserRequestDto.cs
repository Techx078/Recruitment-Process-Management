namespace WebApis.Dtos
{
    public class RegisterOtherUserRequestDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }

        public DepartmentEnum Department { get; set; }

        public List<SkillsDto>? Skills { get; set; }
  
    }
        public enum DepartmentEnum
        {
            IT,
            HR,
            Finance,
            Marketing,
            Sales,
            Operations,
            Administration,
            Support
        }
}
