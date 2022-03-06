using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class BTUser : IdentityUser
    {
        // Required get set values required for any BTUser
        #region Required Fields
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } 
        #endregion

        // Not mapped value in BTUser, and gets the first and last name values of user using return and string interpolation
        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        // Avatar File Names, Data, and ContentType associated with given user
        #region Avatar
        [DisplayName("Avatar")]
        public string AvatarFileName { get; set; }
        public byte[] AvatarFileData { get; set; }

        [Display(Name = "File Extension")]
        public string AvatarContentType { get; set; } 
        #endregion

        // Gets Company Id associated with each instance of BTUser
        public int CompanyId { get; set; }

        // Connects to other Models and database tables
        #region Navigational Properties
        public virtual Company Company { get; set; }
        public virtual ICollection<Project> Projects { get; set; } 
        #endregion
    }
}
