using System.ComponentModel.DataAnnotations.Schema;

namespace Devi.Areas.AdminPanel.ViewModels
{
    public class ServiceUpdateVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
