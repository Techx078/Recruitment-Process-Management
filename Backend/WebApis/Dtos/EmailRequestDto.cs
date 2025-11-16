public class EmailRequestDto
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> Cc { get; set; } = new();
    public List<string> Bcc { get; set; } = new();
}
