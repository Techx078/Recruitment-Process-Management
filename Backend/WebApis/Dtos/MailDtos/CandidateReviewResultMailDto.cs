public class CandidateReviewResultMailDto
{
    public string CandidateName { get; set; }
    public string CandidateEmail { get; set; }

    public string JobTitle { get; set; }
    public string ReviewerName { get; set; }

    public bool IsApproved { get; set; }
}
