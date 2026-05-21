using System.ComponentModel.DataAnnotations.Schema;

namespace Devi.Areas.AdminPanel.ViewModels.Team
{
    public class TeamUpdateVM
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
