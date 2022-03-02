using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTNotificationService : IBTNotificationService
    {
        #region Variables
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBTRolesService _rolesService;
        #endregion

        // Assigns variables values passed to constructor through the parameters
        #region Constructor
        public BTNotificationService(ApplicationDbContext context, IEmailSender emailSender, IBTRolesService rolesService)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }
        #endregion

        #region AddNotificationAsync
        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification); // Adds notification to database
                await _context.SaveChangesAsync(); // Saves data asynchronously
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetRecievedNotificationsAsync
        public async Task<List<Notification>> GetRecievedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications // Gets list notifications of type Notification by going through Notifications table in database where recipient ids match
                    .Include(u => u.Recipient)
                    .Include(u => u.Sender)
                    .Include(u => u.Ticket)
                        .ThenInclude(p => p.Project)
                    .Where(u => u.RecipientId == userId).ToListAsync();

                return notifications;
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetSentNotificationsAsync
        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications // Gets list notifications of type Notification by going through table Notifications in database where sender ids match
                    .Include(u => u.Recipient)
                    .Include(u => u.Sender)
                    .Include(u => u.Ticket)
                        .ThenInclude(p => p.Project)
                    .Where(u => u.SenderId == userId).ToListAsync();

                return notifications;
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region SendEmails Task
        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId); // Gets btUser of type BTUser by going through Users table in database where recipient ids match

            if (btUser != null) // If btUser is there
            {
                string btUserEmail = btUser.Email; 
                string message = notification.Message;

                try
                {
                    await _emailSender.SendEmailAsync(btUserEmail, emailSubject, message); // Calls SendEmailAsync
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

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<BTUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId); // Gets list members of type BTUser by calling GetUsersInRoleAsync in Roles Service

                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id; // Recipient id = user id
                    await SendEmailNotificationAsync(notification, notification.Title); // Calls SendEmailNotificationAsync
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region SendMembersEmailNotificationsAsync
        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
        {
            try
            {
                foreach (BTUser btUser in members)
                {
                    notification.RecipientId = btUser.Id; // Recipient id = user id
                    await SendEmailNotificationAsync(notification, notification.Title); // Calls SendEmailNotificationAsync
                }
            }

            catch (Exception)
            {
                throw;
            }
        } 
        #endregion
    }
}
