using System;
using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign keys compare to different table columns
        #region Foreign Keys
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }

        [DisplayName("Previous")]
        public string OldValue { get; set; }

        [DisplayName("Current")]
        public string NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Description of Change")]
        public string Description { get; set; }

        [DisplayName("Team Member")]
        public string UserId { get; set; }
        #endregion

        // Connects to other Models and database tables
        #region Navigational Properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; } 
        #endregion
    }
}
