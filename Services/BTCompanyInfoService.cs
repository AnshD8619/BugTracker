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
        private readonly ApplicationDbContext _context;

        public BTCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            List<BTUser> result = new();
            result = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();
            return result;
        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> result = new();
            result = await _context.Projects.Where(u => u.CompanyId == companyId)
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

        public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
        {
            List<Ticket> result = new();
            List<Project> projects = new();
            projects = await GetAllProjectsAsync(companyId);
            result = projects.SelectMany(u => u.Tickets).ToList();
            return result;
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new();
            
            if (companyId != null)
            {
                result = await _context.Companies
                    .Include(u => u.Members)
                    .Include(u => u.Projects)
                    .Include(u => u.Invites)
                    .FirstOrDefaultAsync(u => u.Id == companyId);
            }
            return result;
        }
    }
}
