using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Data.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IAppDbContext _appDbContext;

        public AuthRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public User ValidateUser(string username)
        {
            try
            {
                User? user = _appDbContext.Users.FirstOrDefault(c => c.LoginId.ToLower() == username.ToLower() || c.Email == username.ToLower());
                return user;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool RegisterUser(User user)

        {
            try
            {
                var result = false;
                if (user != null)
                {
                    _appDbContext.Users.Add(user);
                    _appDbContext.SaveChanges();

                    return true;
                }
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool UserExists(string loginId, string email)
        {
            try
            {
                if (_appDbContext.Users.Any(c => c.LoginId.ToLower() == loginId.ToLower() || c.Email.ToLower() == email.ToLower()))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEnumerable<SecurityQuestion> GetAllSecurityQuestions()
        {
            try
            {
                var allQuestions = _appDbContext.SecurityQuestions.ToList();
                return allQuestions;
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEnumerable<UserRole> GetUserRoles()
        {
            try
            {
                var allRoles = _appDbContext.UserRoles.ToList();
                return allRoles;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                var result = false;
                if (user != null)
                {
                    _appDbContext.Users.Update(user);
                    _appDbContext.SaveChanges();
                    result = true;
                }
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
