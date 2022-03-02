using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign keys compare to different table columns
        #region Foreign Keys
        // Integer CompanyId associated with each instance of Project if it exists
        [DisplayName("Company")]
        public int? CompanyId { get; set; }

        // Name of project Required
        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string Name { get; set; }

        // Description of project
        [DisplayName("Description")]
        public string Description { get; set; }

        // Start DateTime of project
        [DisplayName("Start Date")]
        public DateTimeOffset StartDate { get; set; }

        // End DateTime of project
        [DisplayName("End Date")]
        public DateTimeOffset EndDate { get; set; }

        // Priority Id integer associated with each instance of project if it exists
        [DisplayName("Priority")]
        public int? ProjectPriorityId { get; set; }

        // File sent in http format
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile ImageFormFile { get; set; }

        // File name
        [DisplayName("File Name")]
        public string ImageFileName { get; set; }

        // File data
        public byte[] ImageFileData { get; set; }

        // File Content
        [DisplayName("File Extension")]
        public string ImageContentType { get; set; } 
        #endregion

        // Archived status of project
        [DisplayName("Archived")]
        public bool Archived { get; set; }

        // Connects to other Models and database tables
        #region Navigational Properties
        public virtual Company Company { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }
        public virtual ICollection<BTUser> Members { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; } 
        #endregion

    }
}
