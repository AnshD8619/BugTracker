using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTCompanyInfoService : IBTCompanyInfoService
    {
        #region Variables
        private readonly ApplicationDbContext _context;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAllMembersAsync
        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            List<BTUser> result = new(); // Initializes new list result of type BTUser
            result = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync(); // Gets result by going through Users table in database where company ids match
            return result;
        }
        #endregion

        #region GetAllProjectsAsync
        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> result = new(); // Creates new list result of type Project
            result = await _context.Projects.Where(u => u.CompanyId == companyId) // Gets result by going through Projects table in database where company ids match
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

            return result;
        }
        #endregion

        #region GetAllTicketsAsync
        public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
        {
            List<Ticket> result = new(); // Initializes new list result of type Ticket
            List<Project> projects = new(); // Initializes new list projects of type Project
            projects = await GetAllProjectsAsync(companyId); // Gets all projects by calling GetAllProjectsAsync
            result = projects.SelectMany(u => u.Tickets).ToList(); // Gets list of tickets from projects
            return result;
        }
        #endregion

        #region GetCompanyInfoByIdAsync
        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new(); // Initializes new result of type Company

            if (companyId != null) // If company id exists
            {
                result = await _context.Companies // Gets result by going through Companies table in database where company ids match
                    .Include(u => u.Members)
                    .Include(u => u.Projects)
                    .Include(u => u.Invites)
                    .FirstOrDefaultAsync(u => u.Id == companyId);
            }
            return result;
        } 
        #endregion
    }
}
