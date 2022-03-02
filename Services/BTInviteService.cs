using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTInviteService : IBTInviteService
    {
        #region Variables
        private readonly ApplicationDbContext _context;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTInviteService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region AcceptInviteAsync
        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite invite = await _context.Invites.FirstOrDefaultAsync(u => u.CompanyToken == token); // Gets invite of type Invite by going through Invites table in database where company tokens match

            if (invite == null)
            {
                return false;
            }

            try
            {
                invite.IsValid = false; // Invite is not valid
                invite.InviteeId = userId; // Invitee id = user id
                await _context.SaveChangesAsync(); // Saves data asynchronously
                return true;
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region AddNewInviteAsync
        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                await _context.Invites.AddAsync(invite); // Adds invite to Invites table in database
                await _context.SaveChangesAsync(); // Saves changes asynchronously
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region AnyInviteAsync
        public async Task<bool> AnyInviteAsync(Guid? token, string email, int companyId)
        {
            try
            {
                bool result = await _context.Invites.Where(u => u.CompanyId == companyId) // Gets result and goes through Invites table in database where company id, company token, and invitee emails match
                    .AnyAsync(u => u.CompanyToken == token && u.InviteeEmail == email);

                return result;
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetInvite Tasks
        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(u => u.CompanyId == companyId) // Gets invite of type Invite by going through Invites table in database where company and invite ids match
                    .Include(u => u.Company)
                    .Include(u => u.Project)
                    .Include(u => u.Invitor)
                    .FirstOrDefaultAsync(u => u.Id == inviteId);

                return invite;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(u => u.CompanyId == companyId) // Gets invite of type Invite by going through Invites table in database where company ids, company tokens, and invitee emails match
                    .Include(u => u.Company)
                    .Include(u => u.Project)
                    .Include(u => u.Invitor)
                    .FirstOrDefaultAsync(u => u.CompanyToken == token && u.InviteeEmail == email);

                return invite;
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ValidateInviteCodeAsync
        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            if (token == null)
            {
                return false;
            }

            bool result = false;

            Invite invite = await _context.Invites.FirstOrDefaultAsync(u => u.CompanyToken == token); // Gets invite of type Invite by going through Invites table in database where company tokens match

            if (invite != null)
            {
                DateTime inviteDate = invite.InviteDate.DateTime; // Gets invite DateTime
                bool validDate = (DateTime.Now - inviteDate).TotalDays <= 7; // Gets time invite is valid until

                if (validDate)
                {
                    result = invite.IsValid; // Checks if invite is valid
                }
            }

            return result;
        } 
        #endregion
    }
}
