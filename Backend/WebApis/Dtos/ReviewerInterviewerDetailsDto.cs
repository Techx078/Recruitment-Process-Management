namespace WebApis.Dtos
{
    public class ReviewerInterviewerDetailsDto
    {
        public int Id { get; set; }
        public string Department { get; set; }

        public UserDto User { get; set; }

        public List<AssignedJobOpeningDto> AssignedJobOpenings { get; set; }
    }

}
