using System.Data;

namespace WebApis.Data
{
    public class Document
    {
        public int id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }

        public ICollection<JobDocument> JobDocuments { get; set; } = new List<JobDocument>();

    }
}
