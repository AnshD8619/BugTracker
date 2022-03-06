using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTLookupService : IBTLookupService
    {
        #region Variables
        private readonly ApplicationDbContext _context;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTLookupService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region GetProjectPrioritiesAsync
        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                return await _context.ProjectPriorities.ToListAsync();
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetTicketPrioritiesAsync
        public async Task<List<TicketPriority>> GetTicketPrioritiesAsync()
        {
            try
            {
                return await _context.TicketPriorities.ToListAsync();
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetTicketStatusesAsync
        public async Task<List<TicketStatus>> GetTicketStatusesAsync()
        {
            try
            {
                return await _context.TicketStatuses.ToListAsync();
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetTicketTypesAsync
        public async Task<List<TicketType>> GetTicketTypesAsync()
        {
            try
            {
                return await _context.TicketTypes.ToListAsync();
            }

            catch (Exception)
            {
                throw;
            }
        } 
        #endregion
    }
}
