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
        private readonly ApplicationDbContext _context;

        public BTInviteService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite invite = await _context.Invites.FirstOrDefaultAsync(u => u.CompanyToken == token);
            
            if (invite == null)
            {
                return false;
            }

            try
            {
                invite.IsValid = false;
                invite.InviteeId = userId;
                await _context.SaveChangesAsync();
                return true;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                await _context.Invites.AddAsync(invite);
                await _context.SaveChangesAsync();
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AnyInviteAsync(Guid? token, string email, int companyId)
        {
            try
            {
                bool result = await _context.Invites.Where(u => u.CompanyId == companyId)
                    .AnyAsync(u => u.CompanyToken == token && u.InviteeEmail == email);

                return result;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(u => u.CompanyId == companyId)
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
                Invite invite = await _context.Invites.Where(u => u.CompanyId == companyId)
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

        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            if(token == null)
            {
                return false;
            }

            bool result = false;

            Invite invite = await _context.Invites.FirstOrDefaultAsync(u => u.CompanyToken == token);

            if(invite != null)
            {
                DateTime inviteDate = invite.InviteDate.DateTime;
                bool validDate = (DateTime.Now - inviteDate).TotalDays <= 7;

                if (validDate)
                {
                    result = invite.IsValid;
                }
            }

            return result;
        }
    }
}
