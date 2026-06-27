using System.Collections.Generic;
using lockin.core.Models;

namespace lockin.core.Interfaces
{
    public interface    IUserRepository
    {
        // Find a user by their unique ID
        UserInfo GetUserById(int id);

        // Fast-filter query to find players in the exact same city/country
        List<UserInfo> GetPlayersInSameLocation(int locationId, int currentUserId);

        // Add a new user profile to the database
        void AddUser(UserInfo user);
    }
}