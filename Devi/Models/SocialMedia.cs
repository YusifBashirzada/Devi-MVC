namespace Devi.Models
{
    public class SocialMedia : BaseEntity
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
