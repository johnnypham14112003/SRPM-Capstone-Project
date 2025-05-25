using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Repositories.Models
{
    public class UserRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        public Guid? ProjectId { get; set; }
        public Guid? CouncilId { get; set; }

        // Navigation properties
        public virtual Account Account { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
        public virtual Project? Project { get; set; }
        public virtual AppraisalCouncil? Council { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
