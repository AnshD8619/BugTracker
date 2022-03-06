using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        #region Variables
        private readonly ApplicationDbContext _context;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region AddHistoryAsync
        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            if (oldTicket == null && newTicket != null) // If there is no old ticket and there is a new ticket
            {
                TicketHistory history = new() // Instantiates history of type TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Ticket Created"
                };

                try
                {
                    await _context.TicketHistories.AddAsync(history); // Adds ticket history to TicketHistories table in database
                    await _context.SaveChangesAsync(); // Saves data asynchronously
                }

                catch (Exception)
                {
                    throw;
                }
            }

            else
            {
                if (oldTicket.Title != newTicket.Title) // If the titles of the old and new tickets dont match
                {
                    TicketHistory history = new() // Instantiates history of type TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket title: {newTicket.Title}"
                    };

                    await _context.TicketHistories.AddAsync(history); // Adds ticket history to TicketHistories table in database
                }

                if (oldTicket.Description != newTicket.Description) // If the description of the old and new tickets dont match
                {
                    TicketHistory history = new() // Instantiates history of type TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.Description}"
                    };
                    await _context.TicketHistories.AddAsync(history); // Adds ticket history to TicketHistories table in database
                }

                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId) // If the ticket priority id of the old and new tickets dont match
                {
                    TicketHistory history = new() // Instantiates history of type TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketPriority",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket priority: {newTicket.TicketPriority.Name}"
                    };
                    await _context.TicketHistories.AddAsync(history); // Adds ticket history to TicketHistories table in database
                }

                if (oldTicket.TicketStatusId != newTicket.TicketStatusId) // If the ticket status id of the old and new tickets dont match
                { 
                    TicketHistory history = new() // Instantiates history of type TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketStatus",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket priority: {newTicket.TicketStatus.Name}"
                    };
                    await _context.TicketHistories.AddAsync(history); // Adds ticket history to TicketHistories table in database
                }

                if (oldTicket.TicketTypeId != newTicket.TicketTypeId) // If the ticket type id of the old and new tickets dont match
                {
                    TicketHistory history = new() // Instantiates history of type TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketTypeId",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket priority: {newTicket.TicketType.Name}"
                    };
                    await _context.TicketHistories.AddAsync(history); // Adds ticket history to TicketHistories table in database
                }

                if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId) // If the developer user id of the old and new tickets dont match
                {
                    TicketHistory history = new() // Instantiates history of type TicketHistory
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket priority: {newTicket.DeveloperUser.FullName}"
                    };
                    await _context.TicketHistories.AddAsync(history); // Adds ticket history to TicketHistories table in database
                }

                try
                {
                    await _context.SaveChangesAsync(); // Saves data asynchronously
                }

                catch (Exception)
                {
                    throw;
                }
            }
        }

        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.FindAsync(ticketId);
                string description = model.ToLower().Replace("ticket", "");
                description = $"New {description} added to ticket: {ticket.Title}";

                TicketHistory history = new()
                {
                    TicketId = ticket.Id,
                    Property = model,
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = description

                };

                await _context.TicketHistories.AddAsync(history);
                await _context.SaveChangesAsync();

            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetCompanyTicketsHistoriesAsync
        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> projects = (await _context.Companies // Gets list of projects of type Project where id = companyId
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.History)
                                .ThenInclude(h => h.User)
                    .FirstOrDefaultAsync(u => u.Id == companyId)).Projects.ToList();

                List<Ticket> tickets = projects.SelectMany(u => u.Tickets).ToList(); // Gets list of tickets of type Ticket
                List<TicketHistory> ticketHistories = tickets.SelectMany(u => u.History).ToList(); // Gets ticketHistories list of type TicketHistory
                return ticketHistories;
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region GetProjectTicketsHistoriesAsync
        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Where(u => u.CompanyId == companyId) // Gets project of type Project from Projects table in database where company ids match
                    .Include(u => u.Tickets)
                        .ThenInclude(t => t.History)
                            .ThenInclude(h => h.User)
                    .FirstOrDefaultAsync(u => u.Id == projectId);

                List<TicketHistory> ticketHistory = project.Tickets.SelectMany(u => u.History).ToList(); // Gets list ticketHistory of type TicketHistory from Tickets in project
                return ticketHistory;

            }

            catch (Exception)
            {
                throw;
            }
        } 
        #endregion
    }
}
