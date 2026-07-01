using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using lockin.core.Interfaces;
using lockin.core.Models;
using lockin.business.Data;


namespace lockin.business.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly lockindbcontext _context;

        public UserRepository(lockindbcontext context)
        {
            _context = context;
        }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            return await _context.UserInfo.FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(UserInfo user)
        {
            _context.UserInfo.Update(user);
            await _context.SaveChangesAsync();
        }

        public UserInfo GetUserById(int id)
        {
            return _context.UserInfo
                           .Include(u => u.Location)
                           .FirstOrDefault(u => u.UserId == id); // Matched to UserId
        }

        public List<UserInfo> GetPlayersInSameLocation(int locationId, int currentUserId)
        {
            // 1. Defensive Validation: Guard against invalid database IDs
            if (locationId <= 0) return new List<UserInfo>();

            // 2. Query the PLAYERS table, not the Locations table
            return _context.UserInfo
                           .Where(u => u.LocationId == locationId)   // Find players in this location
                           .Where(u => u.UserId != currentUserId)     // Exclude the active player
                           .ToList();                                 // Returns List<UserInfo> perfectly matching the interface
        }

        public void AddUser(UserInfo user)
        {
            _context.UserInfo.Add(user);
            _context.SaveChanges();
        }

        public void ProcessAnswerResult(int userId, bool isCorrect, int topicId)
        {
            var user = _context.UserInfo.FirstOrDefault(u => u.UserId == userId); // Matched to UserId
            if (user == null) return;

            if (isCorrect)
            {
                user.Xp += 25;
                user.CurrentStreak++;

                if (user.CurrentStreak > user.HighestStreak)
                {
                    user.HighestStreak = user.CurrentStreak;
                }
            }
            else
            {
                user.Xp -= 10;
                if (user.Xp < 0) user.Xp = 0;

                user.CurrentStreak = 0;
            }

            // Optional: Add logic here to determine LeastMistakesTopicId if tracking history logs later
            _context.SaveChanges();
        }
    }
}
