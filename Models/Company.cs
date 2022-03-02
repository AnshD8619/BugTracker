using System.Collections.Generic;
using System.ComponentModel;

namespace BugTracker.Models
{
    public class Company
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign keys compare to different table columns
        #region Foreign Keys
        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }
        #endregion

        // Connects to other Models and database tables
        #region Navigational Properties

        public virtual ICollection<BTUser> Members { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Invite> Invites { get; set; }  
        #endregion
    }
}
