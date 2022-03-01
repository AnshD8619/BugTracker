﻿using BugTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Services.Interfaces
{
    public interface IBTNotificationService
    {
        public Task AddNotificationAsync(Notification notification);
        public Task<List<Notification>> GetRecievedNotificationsAsync(string userId);
        public Task<List<Notification>> GetSentNotificationsAsync(string userId);
        public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);
        public Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members);
        public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);
    }
}