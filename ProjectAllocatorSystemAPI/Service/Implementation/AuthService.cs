using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;

namespace ProjectAllocatorSystemAPI.Service.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordService _passwordService;

        public AuthService(IAuthRepository authRepository, IPasswordService passwordService)
        {
            _authRepository = authRepository;
            _passwordService = passwordService;
        }
        public ServiceResponse<string> LoginUserService(LoginDto login)
        {
            var response = new ServiceResponse<string>();
            try
            {
                if (login != null)
                {
                    var user = _authRepository.ValidateUser(login.Username);
                    if (user == null)
                    {
                        response.Success = false;
                        response.Message = "Invalid user login id or password";
                        return response;
                    }
                    else if (!_passwordService.VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        response.Success = false;
                        response.Message = "Invalid user login id or password";
                        return response;
                    }

                    string token = _passwordService.CreateToken(user);
                    response.Success = true;
                    response.Data = token;
                    response.Message = "Success";
                    return response;
                }

                response.Success = false;
                response.Message = "Something went wrong, please try after some time";
            }
            catch (Exception ex)
            {
                throw new Exception();
                // Log the exception if needed
            }
            return response;
        }

        public ServiceResponse<string> RegisterUserService(RegisterUserDto register)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var message = string.Empty;
                if (register != null)
                {
                    message = _passwordService.CheckPasswordStrength(register.Password);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        response.Success = false;
                        response.Message = message;
                        return response;
                    }
                    else if (_authRepository.UserExists(register.LoginId, register.Email))
                    {
                        response.Success = false;
                        response.Message = "User already exist";
                        return response;
                    }
                    else if (register.Gender.ToLower() != "m" && register.Gender.ToLower() != "f")
                    {
                        response.Success = false;
                        response.Message = "Gender can be either M or F";
                        return response;
                    }
                    else if (register.Phone.Length > 12 || register.Phone.Length < 10)
                    {
                        response.Success = false;
                        response.Message = "Enter valid phone number";
                        return response;
                    }
                    else
                    {
                        User user = new User()
                        {
                            Name = register.Name,
                            LoginId = register.LoginId,
                            Email = register.Email,
                            Phone = register.Phone,
                            Gender = register.Gender,
                            SecurityQuestionId = register.SecurityQuestionId,
                            Answer = register.Answer,
                            UserRoleId = register.UserRoleId,
                        };

                        _passwordService.CreatePasswordHash(register.Password, out byte[] passwordHash, out byte[] passwordSalt);
                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                        var result = _authRepository.RegisterUser(user);
                        response.Success = result;
                        response.Message = result ? string.Empty : "Something went wrong, please try after sometime";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;

        }

        public ServiceResponse<IEnumerable<SecurityQuestion>> GetAllQuestions()
        {
            var response = new ServiceResponse<IEnumerable<SecurityQuestion>>();
            try
            {
                var questions = _authRepository.GetAllSecurityQuestions();
                if (questions != null && questions.Any())
                {

                    response.Data = questions;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Data does not exists.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return response;
        }

        public ServiceResponse<IEnumerable<UserRole>> GetUserRole()
        {
            var response = new ServiceResponse<IEnumerable<UserRole>>();
            try
            {
                var questions = _authRepository.GetUserRoles().Where(c => c.UserRoleId != 3);
                if (questions != null && questions.Any())
                {

                    response.Data = questions;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Data does not exists.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return response;
        }

        public ServiceResponse<string> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var response = new ServiceResponse<string>();
            try
            {
                if (changePasswordDto != null)
                {
                    var user = _authRepository.ValidateUser(changePasswordDto.LoginId);
                    if (user == null)
                    {
                        response.Success = false;
                        response.Message = "Something went wrong, please try after some time.";
                        return response;
                    }

                    if (changePasswordDto.OldPassword == changePasswordDto.NewPassword)
                    {
                        response.Success = false;
                        response.Message = "New password cannot be same as old password.";
                        return response;
                    }

                    if (!_passwordService.VerifyPasswordHash(changePasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                    {
                        response.Success = false;
                        response.Message = "Old password is incorrect.";
                        return response;
                    }

                    _passwordService.CreatePasswordHash(changePasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    var result = _authRepository.UpdateUser(user);
                    response.Success = result;
                    response.Message = result ? "Successfully updated password. Signin again!" : "Something went wrong, please try after some time.";

                    var message = _passwordService.CheckPasswordStrength(changePasswordDto.NewPassword);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Something went wrong, please try after some time.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
        }

        public ServiceResponse<string> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = new ServiceResponse<string>();
            try
            {
                if (resetPasswordDto != null)
                {
                    var user = _authRepository.ValidateUser(resetPasswordDto.LoginId);
                    if (user == null)
                    {
                        response.Success = false;
                        response.Message = "Invalid loginId!";
                        return response;
                    }

                    if (resetPasswordDto.SecurityQuestionId != user.SecurityQuestionId)
                    {
                        response.Success = false;
                        response.Message = "User verification failed!";
                        return response;
                    }

                    // Trimming and converting to lowercase
                    resetPasswordDto.Answer = resetPasswordDto.Answer.Trim();

                    if (user.Answer != resetPasswordDto.Answer)
                    {
                        response.Success = false;
                        response.Message = "User verification failed!";
                        return response;
                    }

                    var message = _passwordService.CheckPasswordStrength(resetPasswordDto.NewPassword);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        response.Success = false;
                        response.Message = message;
                        return response;
                    }

                    _passwordService.CreatePasswordHash(resetPasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    var result = _authRepository.UpdateUser(user);
                    response.Success = result;
                    response.Message = result ? "Successfully updated password." : "Something went wrong, please try after some time.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Something went wrong, please try after some time.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
        }
    }
}
