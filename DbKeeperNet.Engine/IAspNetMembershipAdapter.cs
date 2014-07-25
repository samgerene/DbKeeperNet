namespace DbKeeperNet.Engine
{
    /// <summary>
    /// An interface for adapter to ASP.NET membership providers
    /// </summary>
    public interface IAspNetMembershipAdapter
    {
        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userName">Seeded user's name</param>
        /// <param name="password">Seeded password</param>
        /// <param name="email">Associated email address</param>
        void CreateUser(string userName, string password, string email);

        /// <summary>
        /// Assign user to roles
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <param name="roles">Collection of roles to which user should belong</param>
        void AddUserToRoles(string userName, string[] roles);

        /// <summary>
        /// Delete an existing user
        /// </summary>
        /// <param name="userName">Existing user's name</param>
        /// <returns><c>true</c> if user was deleted, <c>false</c> otherwise</returns>
        bool DeleteUser(string userName);

        /// <summary>
        /// Checks whether the <paramref name="userName"/> exists.
        /// </summary>
        /// <param name="userName">Login name of the user to be checked</param>
        /// <returns><c>true</c> if user exists, <c>false</c> otherwise</returns>
        bool UserExists(string userName);

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="roleName">Role name to be created</param>
        void CreateRole(string roleName);
    }
}