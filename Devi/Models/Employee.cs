namespace Devi.Models
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
        public List<SocialMedia> SocialMedias { get; set; }
    }
}
