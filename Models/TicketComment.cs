using System;
using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketComment
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign keys compare to different table columns
        #region Foreign Keys
        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

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
