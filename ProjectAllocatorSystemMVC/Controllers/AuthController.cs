using ProjectAllocatorSystemMVC.Infrastructure;
using ProjectAllocatorSystemMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
namespace ProjectAllocatorSystemMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenHandler _tokenHandler;
        private string endPoint;
        public AuthController(IHttpClientService httpClientService, IConfiguration configuration, IJwtTokenHandler tokenHandler)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            _tokenHandler = tokenHandler;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUser(LoginViewModel viewModel)
        {
            try { 
            if (ModelState.IsValid)
            {
                string apiUrl = $"{endPoint}Auth/Login";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    string token = serviceResponse.Data;
                    Response.Cookies.Append("jwtToken", token, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddDays(1) //Set expiration time for cookie.
                    });
                    var jwtToken = _tokenHandler.ReadJwtToken(token);
                    //var userRoleId = jwtToken.Claims.First(claim => claim.Type == "UserRole").Value;
                    //Response.Cookies.Append("UserRole", userRoleId, new CookieOptions
                    //{
                    //    HttpOnly = false,
                    //    Secure = true,
                    //    SameSite = SameSiteMode.None,
                    //    Expires = DateTime.UtcNow.AddDays(1),
                    //});
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong, Please try after sometime.";
                    }
                }
            }
            return View(viewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            //Response.Cookies.Delete("UserRole");
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterUser()
        {
            IEnumerable<UserRoleViewModel> userRoles = GetUserRoles();
            ViewBag.UserRoleRoles = userRoles;
            IEnumerable<SecurityQuestionViewModel> securityQuestions = GetQuestions();
            ViewBag.SecurityQuestions = securityQuestions;
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(RegisterViewModel registerViewModel)
        {
            try { 
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Auth/Register";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, registerViewModel, HttpContext.Request);

                if (response.IsSuccessStatusCode)
                {

                    string data = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(data);
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("RegisterSuccess", "Auth");
                }
                else
                {
                    string errorData = response.Content.ReadAsStringAsync().Result;
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorData);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = errorResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                    }
                }
            }
            IEnumerable<UserRoleViewModel> userRoles = GetUserRoles();
            ViewBag.UserRoleRoles = userRoles;
            IEnumerable<SecurityQuestionViewModel> securityQuestion = GetQuestions();
            ViewBag.SecurityQuestions = securityQuestion;
            return View(registerViewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            try { 
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Auth/ChangePassword";
                var response = _httpClientService.PutHttpResponseMessage(apiUrl, changePasswordViewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    Response.Cookies.Delete("jwtToken");
                    Response.Cookies.Delete("UserRole");
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("LoginUser", "Auth");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                    }
                }
            }
            return View(changePasswordViewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            IEnumerable<SecurityQuestionViewModel> questions = GetQuestions();
            ViewBag.Questions = questions;
            return View();
        }

        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            try { 
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}User/ForgetPassword";
                var response = _httpClientService.PutHttpResponseMessage(apiUrl, forgetPasswordViewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["SuccessMessage"] = serviceResponse.Message;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                    }
                }
            }
            IEnumerable<SecurityQuestionViewModel> questions = GetQuestions();
            ViewBag.Questions = questions;
            return View(forgetPasswordViewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }


        private IEnumerable<SecurityQuestionViewModel> GetQuestions()
        {
            var apiUrl = $"{endPoint}Auth/GetAllSecurityQuestions";
            ServiceResponse<IEnumerable<SecurityQuestionViewModel>> response = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);
            return response.Data;
        }

        private IEnumerable<UserRoleViewModel> GetUserRoles()
        {
            
            var apiUrl = $"{endPoint}Auth/GetUserRoles";
            ServiceResponse<IEnumerable<UserRoleViewModel>> response = new ServiceResponse<IEnumerable<UserRoleViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);
            return response.Data;
        }

    }
}
