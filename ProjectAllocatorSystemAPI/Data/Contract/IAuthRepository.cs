using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Data.Contract
{
    public interface IAuthRepository
    {
        User ValidateUser(string username);
        bool RegisterUser(User user);
        bool UserExists(string loginId, string email);
        IEnumerable<SecurityQuestion> GetAllSecurityQuestions();
        IEnumerable<UserRole> GetUserRoles();
        bool UpdateUser(User user);
    }
}
