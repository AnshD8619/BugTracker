using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        #region Variables
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }
        #endregion

        #region AddNewProjectAsync
        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project); // Adds project to database asynchronously
            await _context.SaveChangesAsync(); // Saves data asynchronously
        }
        #endregion

        #region AddUser Tasks
        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentPM = await GetProjectManagerAsync(projectId); // Gets currentPM of type BTUser by calling GetProjectManagerAsync

            if (currentPM != null) // If a value if recieved that is not null
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId); // Removes current PM by calling RemoveProjectManagerAsync

                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error removing current PM. - Error: {ex.Message}");
                    return false;
                }
            }

            try
            {
                await AddProjectManagerAsync(userId, projectId); // Adds PM by calling AddProjectManagerAsync
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new PM. - Error: {ex.Message}");
                return false;

            }
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Gets user of type BTUser by going through the Users table in the database and getting the values where ids match

            if (user != null) // If user is not null
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(u => u.Id == projectId); // Gets project of type Project by going through the Projects table in the database where project ids match
                if (!await IsUserOnProjectAsync(userId, projectId)) // if user is not on project
                {
                    try
                    {
                        project.Members.Add(user); // Adds user to project tables members column
                        await _context.SaveChangesAsync(); // Saves data asynchronously
                        return true;
                    }

                    catch (Exception)
                    {
                        throw;
                    }
                }

                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }
        #endregion

        #region ArchiveProjectAsync
        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _context.Update(project); // Adds project to database
            await _context.SaveChangesAsync(); // Saves data asynchronously
        }
        #endregion

        #region GetAllProjectMembersExceptPMAsync
        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString()); // Gets list developers of type BTUsers by calling GetProjectMembersByRoleAsync
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString()); // Gets list submitters of type BTUsers by calling GetProjectMembersByRoleAsync
            List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString()); // Gets list admins of type BTUsers by calling GetProjectMembersByRoleAsync

            List<BTUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList(); // Gets list teamMembers of type BTUsers by putting all lists together

            return teamMembers;
        }
        #endregion

        #region GetProjects Tasks
        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = new(); // Instantiates list projects of type Project
            projects = await _context.Projects.Where(u => u.CompanyId == companyId) // Gets all projects from Projects table in database where company ids match
                            .Include(u => u.Members)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.Comments)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.Attachments)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.History)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.Notifications)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.DeveloperUser)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.OwnerUser)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.TicketStatus)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.TicketPriority)
                            .Include(u => u.Tickets)
                                .ThenInclude(t => t.TicketType)
                            .Include(u => u.ProjectPriority)
                            .ToListAsync();

            return projects;

        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId); // Gets list projects of type Project by calling GetAllProjectsByCompany
            int priorityId = await LookupProjectPriorityId(priorityName); // Gets priority id by calling LookupProjectPriorityId
            return projects.Where(u => u.ProjectPriorityId == priorityId).ToList(); // Returns projects list where the priority ids match
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId); // Gets list projects of type Project by calling GetAllProjectsByCompany

            return projects.Where(u => u.Archived == true).ToList(); // Returns projects where Archived = true
        }
        #endregion

        #region GetDevelopersOnProjectAsync
        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region GetProjectByIdAsync
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects // Gets project of type Project by going through Projects table in database where the project and company ids match
                .Include(u => u.Tickets)
                .Include(u => u.Members)
                .Include(u => u.ProjectPriority)
                .FirstOrDefaultAsync(u => u.Id == projectId && u.CompanyId == companyId);

            return project;
        }
        #endregion

        #region GetProjectManagerAsync
        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects // Gets project of type Project by going throhg Projects table in database where project ids match
                .Include(u => u.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (BTUser member in project?.Members) // Goes through each member of type BTUser in project.members
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString())) // if user is in role by calling IsUserInRoleAsync
                {
                    return member;
                }
            }
            return null;
        }
        #endregion

        #region GetProjectMembersByRoleAsync
        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects // Gets project fo type Project by going through Projects table in database where project ids match
                .Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            List<BTUser> members = new(); // Instantiates list members of type BTUser
            foreach (var user in project.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(user, role)) // If user is in role by calling IsUserInRoleAsync
                {
                    members.Add(user); // Adds user to members
                }
            }

            return members;
        }
        #endregion

        #region GetSubmittersOnProjectAsync
        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region GetUserProjectsAsync
        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users // Gets userProjects list of type Project by going through Users table in database where user ids match
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error Getting user projects list. --> {ex.Message}");
                throw;
            }
        }
        #endregion

        #region GetUsersNotOnProjectAsync
        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<BTUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync(); // Gets list users of type BTUser by going through Users table in database where project ids dont match

            return users.Where(u => u.CompanyId == companyId).ToList(); // Returns users where company ids match
        }
        #endregion

        #region IsUserOnProjectAsync
        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects // Gets project of type Project by going through Projects table in database where project ids match
                .Include(u => u.Members)
                .FirstOrDefaultAsync(u => u.Id == projectId);

            bool result = false;

            if (project != null) // If a project is returned
            {
                result = project.Members.Any(u => u.Id == userId); // Gets any Members where user ids match
            }

            return result;
        }
        #endregion

        #region LookupProjectPriorityId
        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(u => u.Name == priorityName)).Id; // gets priorityId by going through ProjectPriorities table in database where priority names match
            return priorityId;
        }

        #endregion

        #region RemoveProjectManagerAsync
        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects // Gets project of type Project by going through Projects table in database where project ids match
                .Include(u => u.Members)
                .FirstOrDefaultAsync(u => u.Id == projectId);

            try
            {
                foreach (BTUser member in project.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString())) // Checks if user is in role by calling IsUserInRoleAsync
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId); // Calls RemoveUserFromProjectAsync to remove the user from the project
                    }
                }
            }

            catch
            {
                throw;
            }
        }
        #endregion

        #region RemoveUserFromProjectAsync
        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Gets user of type BTUser by going through Users table in database where user ids match
                Project project = await _context.Projects.FirstOrDefaultAsync(u => u.Id == projectId); // Gets project of type Project by going through Projects table in database where project ids match
                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId)) // Checks if user is in project by calling IsUserOnProjectAsync
                    {
                        project.Members.Remove(user); // Removes user from members
                        await _context.SaveChangesAsync(); // Saves data asynchronously
                    }
                }

                catch (Exception)
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Error Removing User from project. ---> {ex.Message}");
            }
        }
        #endregion

        #region RemoveUsersFromProjectByRoleAsync
        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role); // Gets members list of type BTUsers by calling GetProjectMembersByRoleAsync
                Project project = await _context.Projects.FirstOrDefaultAsync(u => u.Id == projectId); // Gets project of type Project by going through Projects table in database where project ids match

                foreach (BTUser btUser in members)
                {
                    try
                    {
                        project.Members.Remove(btUser); // Removes user from Members
                        await _context.SaveChangesAsync(); // Saves data asynchronously
                    }

                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error removing users from project. --> {ex.Message}");
                throw;
            }
        }
        #endregion

        #region UpdateProjectAsync
        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project); // Updates project in database
            await _context.SaveChangesAsync(); // Saves data asynchronously
        } 
        #endregion
    }
}
