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
        #region Variables
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTTicketService(ApplicationDbContext context, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
        }
        #endregion

        #region AddNewTicketsAsync
        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Add(ticket); // Adds ticket to database
                await _context.SaveChangesAsync(); // Saves data asynchronously
            }

            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region ArchiveTicketAsync
        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true; // Sets archived value as true
                _context.Add(ticket); // Adds ticket to database
                await _context.SaveChangesAsync(); // Saves data asynchronously
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region AssignTicketAsync
        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(u => u.Id == ticketId); // Creates Ticket object ticket by calling Tickets database and getting ticketId
            try
            {
                if (ticket != null) // if ticket is not null
                {
                    try
                    {
                        ticket.DeveloperUserId = userId; // Sets developerUserId to userId passed
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value; // Gets ticket statudId by calling LookupTicketStatusIdAsync
                        await _context.SaveChangesAsync(); // Saves data asynchronously
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
        #endregion

        #region GetAllTickets Tasks
        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects.Where(u => u.CompanyId == companyId) // Creates list of tickets of type Ticket where company ids match
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
            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value; // Gets priorityId by calling LookupTicketPriorityIdAsync

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(p => p.CompanyId == companyId) // Creates list of tickets of type Ticket where company ids and priority ids match
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

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value; // Gets status id by calling LookupTicketStatusIdAsync

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(p => p.CompanyId == companyId) // Creates list of tickets of type Ticket where company ids and status ids match
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
            int typeId = (await LookupTicketTypeIdAsync(typeName)).Value; // Gets type id by calling LookupTicketTypeIdAsync

            try
            {
                List<Ticket> tickets = await _context.Projects.Where(p => p.CompanyId == companyId) // Creates list of tickets of type Ticket where company ids and type ids match
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
        #endregion

        #region GetArchivedTicketsAsync
        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(u => u.Archived == true).ToList(); // Gets List of tickets of type Ticket where archived is true by calling GetAllTicketsByCompanyAsync 
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region GetProjectTickets Tasks
        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            List<Ticket> tickets = new(); // Instantiates List tickets of type Ticket

            try
            {
                tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(u => u.ProjectId == projectId).ToList(); // Adds all tickets where project ids match by calling  GetAllTicketsByPriorityAsync to list
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            List<Ticket> tickets = new(); // Instantiates List tickets of type Ticket

            try
            {
                tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(userId => userId.ProjectId == projectId).ToList(); // Adds all tickets where project ids match by calling  GetTicketsByRoleAsync to list
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            List<Ticket> tickets = new(); // Instantiates List tickets of type Ticket

            try
            {
                tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(u => u.ProjectId == projectId).ToList(); // Adds all tickets where project ids match by calling GetAllTicketsByStatusAsync to list
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            List<Ticket> tickets = new(); // Instantiates List tickets of type Ticket

            try
            {
                tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(u => u.ProjectId == projectId).ToList(); // Adds all tickets where project ids match by calling GetAllTicketsByTypeAsync to list
                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region GetTickets Tasks
        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets.FirstOrDefaultAsync(u => u.Id == ticketId); // Gets all tickets from Tickets table from database by comparing ticket ids
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            BTUser developer = new(); // Instantiates BTUser developer

            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(u => u.Id == ticketId); // Gets ticket by calling GetAllTicketsByCompanyAsync where ticket ids match

                if (ticket?.DeveloperUser != null) // If ticket developer user is not null, BTUser developer = tickets developer
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
            List<Ticket> tickets = new(); // Instantiates Ticket list tickets

            try
            {
                if (role == Roles.Admin.ToString()) // If role = admin, gets all tickets by calling GetAllTicketsByCompanyAsync
                {
                    tickets = await GetAllTicketsByCompanyAsync(companyId);
                }

                else if (role == Roles.Developer.ToString()) // If role = developer, gets all tickets where developer user id = user id by calling GetAllTicketsByCompanyAsync
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(u => u.DeveloperUserId == userId).ToList();
                }

                else if (role == Roles.Submitter.ToString()) // If role = submitters, gets all tickets where owner user id = user id by calling GetAllTicketsByCompanyAsync
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(u => u.OwnerUserId == userId).ToList();
                }

                else if (role == Roles.ProjectManager.ToString()) // If role = Project Manager, gets all tickets where PM id = user id by calling GetAllTicketsByUserIdAsync
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
            BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // Gets a new btUser of type BTUser from Users table in database where user ids match
            List<Ticket> tickets = new(); // Instantiates new List tickets of type Ticket

            try
            {
                if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Admin.ToString())) // Checks if user is an admin by calling IsUserInRoleAsync from roles service
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId)) // Adds tickets to list by calling GetAllProjectsByCompany from project service
                        .SelectMany(u => u.Tickets).ToList();
                }

                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Developer.ToString())) // Checks if user is a developer by calling IsUserInRoleAsync from roles service
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId)) // Adds tickets to list by calling GetAllProjectsByCompany from project service where developer user id = user id
                        .SelectMany(u => u.Tickets).Where(u => u.DeveloperUserId == userId).ToList();
                }

                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Submitter.ToString())) // Checks if user is a submitter by calling IsUserInRoleAsync from roles service
                {
                    tickets = (await _projectService.GetAllProjectsByCompany(companyId)) // Adds tickets to list by calling GetAllProjectsByCompany from project service where owner user id = user id
                        .SelectMany(u => u.Tickets).Where(u => u.OwnerUserId == userId).ToList();
                }

                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.ProjectManager.ToString())) // Checks if user is a project manager by calling IsUserInRoleAsync from roles service
                {
                    tickets = (await _projectService.GetUserProjectsAsync(userId)) // Adds tickets to list by calling GetAllProjectsByCompany from project service
                        .SelectMany(u => u.Tickets).ToList();
                }

                return tickets;
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region LookupTicket Tasks
        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(u => u.Name == priorityName); // Gets priority of type TicketPriority by looking through the TicketPriorities table where Name = priority name.
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
                TicketStatus status = await _context.TicketStatuses.FirstOrDefaultAsync(u => u.Name == statusName); // Gets status of type TicketStatus by looking through the TicketStatuses table where Name = status name.
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
                TicketType type = await _context.TicketTypes.FirstOrDefaultAsync(u => u.Name == typeName); // Gets type of type TicketType by looking through the TicketTypes table where Name = type name.
                return type?.Id;
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region UpdateTicketAsync
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket); // Updates ticket in database
                await _context.SaveChangesAsync(); // Saves data asynchronously
            }

            catch (Exception)
            {
                throw;
            }
        } 
        #endregion
    }
}
