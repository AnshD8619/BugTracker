using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Notification
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign keys compare to different table columns
        #region Foreign Keys
        // Required values for each instance of notification
        #region Required
        [Required]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Message { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string RecipientId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string SenderId { get; set; }
        #endregion

        // TicketId associated with each instance of notification
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        // DateTime that the Notification was created
        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; } 
        #endregion

        // Boolean value whether the instance of notification has been viewed or not
        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }

        // Connects to other Models and database tables
        #region Navigational Properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser Recipient { get; set; }
        public virtual BTUser Sender { get; set; } 
        #endregion
    }
}
