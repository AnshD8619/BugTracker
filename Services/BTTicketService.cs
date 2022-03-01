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
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                _context.Add(ticket);
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(u => u.Id == ticketId);
            try
            {
                if(ticket != null)
                {
                    try
                    {
                        ticket.DeveloperUserId = userId;
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                        await _context.SaveChangesAsync();
                    }

                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects.Where(u => u.CompanyId == companyId)
                    .SelectMany(u => u.Tickets)
                        .Include(u => u.Attachments)
                        .Include(u => u.Comments)
                        .Include(u => u.DeveloperUser)
                        .Include(u => u.OwnerUser)
                        .Include(u => u.TicketPriority)
                        .Include(u => u.TicketStatus)
                        .Include(u => u.History)
                        .Include(u => u.TicketType)
                    .ToListAsync();

                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(p => p.CompanyId == companyId)
                    .SelectMany(u => u.Tickets)
                        .Include(u => u.Attachments)
                        .Include(u => u.Comments)
                        .Include(u => u.DeveloperUser)
                        .Include(u => u.OwnerUser)
                        .Include(u => u.TicketPriority)
                        .Include(u => u.TicketStatus)
                        .Include(u => u.TicketType)
                        .Include(u => u.Project)
                    .Where(u => u.TicketPriorityId == priorityId).ToListAsync();

                return tickets;
            }

            catch(Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(p => p.CompanyId == companyId)
                    .SelectMany(u => u.Tickets)
                        .Include(u => u.Attachments)
                        .Include(u => u.Comments)
                        .Include(u => u.DeveloperUser)
                        .Include(u => u.OwnerUser)
                        .Include(u => u.TicketPriority)
                        .Include(u => u.TicketStatus)
                        .Include(u => u.TicketType)
                        .Include(u => u.Project)
                    .Where(u => u.TicketStatusId == statusId)
                    .ToListAsync();
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(p => p.CompanyId == companyId)
                    .SelectMany(u => u.Tickets)
                        .Include(u => u.Attachments)
                        .Include(u => u.Comments)
                        .Include(u => u.DeveloperUser)
                        .Include(u => u.OwnerUser)
                        .Include(u => u.TicketPriority)
                        .Include(u => u.TicketStatus)
                        .Include(u => u.TicketType)
                        .Include(u => u.Project)
                    .Where(u => u.TicketTypeId == typeId).ToListAsync();

                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(u => u.Archived == true).ToList();
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(u => u.ProjectId == projectId).ToList();
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(userId => userId.ProjectId == projectId).ToList();
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(u => u.ProjectId == projectId).ToList();
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(u => u.ProjectId == projectId).ToList();
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets.FirstOrDefaultAsync(u => u.Id == ticketId);
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            BTUser developer = new();

            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(u => u.Id == ticketId);

                if(ticket?.DeveloperUser != null)
                {
                    developer = ticket.DeveloperUser;
                }

                return developer;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                if(role == Roles.Admin.ToString())
                {
                    tickets = await GetAllTicketsByCompanyAsync(companyId);
                }

                else if(role == Roles.Developer.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(u => u.DeveloperUserId == userId).ToList();
                }

                else if(role == Roles.Submitter.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(u => u.OwnerUserId == userId).ToList();
                }

                else if(role == Roles.ProjectManager.ToString())
                {
                    tickets = await GetTicketsByUserIdAsync(userId, companyId);
                }

                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<Ticket> tickets = new();

            try
            {
                if(await _rolesService.IsUserInRoleAsync(btUser, Roles.Admin.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                        .SelectMany(u => u.Tickets).ToList();
                }

                else if(await _rolesService.IsUserInRoleAsync(btUser, Roles.Developer.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                        .SelectMany(u => u.Tickets).Where(u => u.DeveloperUserId == userId).ToList();
                }

                else if(await _rolesService.IsUserInRoleAsync(btUser, Roles.Submitter.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId))
                        .SelectMany(u => u.Tickets).Where(u => u.OwnerUserId == userId).ToList();
                }

                else if(await _rolesService.IsUserInRoleAsync(btUser, Roles.ProjectManager.ToString()))
                {
                    tickets = (await _projectService.GetUserProjectsAsync(userId))
                        .SelectMany(u => u.Tickets).ToList();
                }

                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(u => u.Name == priorityName);
                return priority?.Id;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                TicketStatus status = await _context.TicketStatuses.FirstOrDefaultAsync(u => u.Name == statusName);
                return status?.Id;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                TicketType type = await _context.TicketTypes.FirstOrDefaultAsync(u => u.Name == typeName);
                return type?.Id;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
