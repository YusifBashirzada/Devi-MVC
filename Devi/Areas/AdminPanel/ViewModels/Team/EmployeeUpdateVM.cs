using Devi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Devi.Areas.AdminPanel.ViewModels
{
    public class EmployeeUpdateVM
    {
        public string Name { get; set; }
        public int PositionId { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }

        public string? TwitterUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedinUrl { get; set; }

        public List<Position>? Positions { get; set; }
    }
}
