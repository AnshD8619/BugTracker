using System;
using System.ComponentModel;

namespace BugTracker.Models
{
    public class Invite
    {
        // Primary Key
        public int Id { get; set; }

        // Date invite was sent
        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; }

        // Date that invitee joined
        [DisplayName("Join Date")]
        public DateTimeOffset JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        // Foreign keys compare to different table columns
        #region Foreign Keys

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string InviteeId { get; set; }

        [DisplayName("Invitee Email")]
        public string InviteeEmail { get; set; }

        [DisplayName("Invitee First Name")]
        public string InviteeFirstName { get; set; }

        [DisplayName("Invitee Last Name")]
        public string InviteeLastName { get; set; } 
        #endregion

        public bool IsValid { get; set; }

        // Connects to other Models and database tables
        #region Navigational Properties

        public virtual Company Company { get; set; }
        public virtual BTUser Invitor { get; set; }
        public virtual BTUser Invitee { get; set; }
        public virtual Project Project { get; set; } 
        #endregion
    }
}
