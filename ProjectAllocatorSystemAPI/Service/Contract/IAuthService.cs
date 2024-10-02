using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Service.Contract
{
    public interface IAuthService
    {
        ServiceResponse<string> LoginUserService(LoginDto login);
        ServiceResponse<string> RegisterUserService(RegisterUserDto register);
        ServiceResponse<IEnumerable<SecurityQuestion>> GetAllQuestions();
        ServiceResponse<IEnumerable<UserRole>> GetUserRole();
        ServiceResponse<string> ChangePassword(ChangePasswordDto changePasswordDto);
        ServiceResponse<string> ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
