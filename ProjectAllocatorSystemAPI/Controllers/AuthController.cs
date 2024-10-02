using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Service.Contract;

namespace ProjectAllocatorSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            try
            {
                var response = _authService.LoginUserService(loginDto);
                return !response.Success ? BadRequest(response) : Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
           
        }

        [HttpPost("Register")]
        public IActionResult AddUser(RegisterUserDto registerDto)
        {
            try
            {
                var response = _authService.RegisterUserService(registerDto);
                return !response.Success ? BadRequest(response) : Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }

           


        }

        [HttpGet("GetAllSecurityQuestions")]
        public IActionResult GetAllSecurityQuestions()
        {
            try
            {
                var response = _authService.GetAllQuestions();
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
            
        }

        [HttpGet("GetUserRoles")]
        public IActionResult GetUserRoles()
        {
            try
            {
                var response = _authService.GetUserRole();
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var response = _authService.ChangePassword(changePasswordDto);
                return !response.Success ? BadRequest(response) : Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           
        }

        [AllowAnonymous]
        [HttpPut("ForgetPassword")]
        public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var response = _authService.ResetPassword(resetPasswordDto);
                return !response.Success ? BadRequest(response) : Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           
        }
    }
}
