using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
        #region Variables
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTRolesService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #endregion

        #region AddUserToRoleAsync
        public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded; // checks if adding the user succeeded
            return result;
        }
        #endregion

        #region GetUserRole Tasks
        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId); // Gets role of type IdentityRole by checking the Roles table in the database where role id matches
            string result = await _roleManager.GetRoleNameAsync(role); 
            return result;
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new(); // Instantiates new list result of type IdentityRole
                result = await _context.Roles.ToListAsync(); // Adds all role names to result list by going through Roles table in database
                return result;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user); // Gets all roles associated with given user by calling GetRolesAsync
            return result;
        }

        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<BTUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList(); // gets list users of type BTUser by calling GetUsersInRoleAsync
            List<BTUser> result = users.Where(u => u.CompanyId == companyId).ToList(); // gets list result of type BTUser where company ids match
            return result;
        }
        #endregion

        #region GetUsersNotInRoleAsync
        public async Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList(); // Gets list userIds of type string by calling GetUsersInRoleAsync
            List<BTUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList(); // Gets list roleUsers of type BTUser by going through the Users table in the database where userIds doesnt have the id
            List<BTUser> result = roleUsers.Where(u => u.CompanyId == companyId).ToList(); // Gets list result of type BTUser where company ids match
            return result;
        }
        #endregion

        #region IsUserInRoleAsync
        public async Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName); // Gets result by calling IsInRoleAsync
            return result;
        }
        #endregion

        #region RemoveUserFromRole Tasks
        public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded; // Gets result by calling RemoveFromRoleAsync
            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded; // Gets result by calling RemoveFromRolesAsync
            return result;
        } 
        #endregion
    }
}
