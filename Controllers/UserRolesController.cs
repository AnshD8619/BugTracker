using BugTracker.Extensions;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        // Private variables of specific types
        #region Variables
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;
        #endregion

        // Constructs services by applying names to the values passed through the parameters to use in the controller
        #region Constructor
        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }
        #endregion

        // Gets User Roles
        #region ManageUserRoles Get Method
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new(); // An instance of the ViewModel as a list model
            int companyId = User.Identity.GetCompanyId().Value;
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);
                model.Add(viewModel);
            }

            return View(model); 
            
        }
        #endregion

        // POST's roles to View
        #region ManageUserRoles POST Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                if (await _rolesService.RemoveUserFromRoleAsync(btUser, userRole))
                {
                    await _rolesService.AddUserToRoleAsync(btUser, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        } 
        #endregion
    }
}
