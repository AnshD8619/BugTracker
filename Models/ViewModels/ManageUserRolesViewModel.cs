using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BugTracker.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        // Gets and sets a BTUser of type BTUser
        public BTUser BTUser { get; set; }

        // Multi list of roles
        public MultiSelectList Roles { get; set; }

        // List of Selected Roles of type string for each entry
        public List<string> SelectedRoles { get; set; }
    }
}
