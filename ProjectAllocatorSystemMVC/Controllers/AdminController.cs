using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectAllocatorSystemMVC.Infrastructure;
using ProjectAllocatorSystemMVC.ViewModels;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace ProjectAllocatorSystemMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IConfiguration _configuration;

        private string endPoint;

        public AdminController(IHttpClientService httpClientService, IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }


        public IActionResult Index(string? search, string? sortBy, string sortOrder = "asc", int page = 1, int pageSize = 2)
        {
            try
            {
                var getBooksUrl = "";
                var getBooksCountUrl = "";
                ViewBag.Search = search;
                if (search != null && sortBy != null && search.Length > 2)
                {
                    getBooksUrl = endPoint + "Admin/GetAllEmployeesByPagination?search=" + search + "&sortBy=" + sortBy + "&page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                    getBooksCountUrl = $"{endPoint}Admin/GetEmployeesCount?search={search}";
                }
                else if (search == null && sortBy != null)
                {
                    getBooksUrl = endPoint + "Admin/GetAllEmployeesByPagination?sortBy=" + sortBy + "&page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                    getBooksCountUrl = endPoint + "Admin/GetEmployeesCount";
                }
                else if (search != null && search.Length > 2 && sortBy == null)
                {
                    getBooksUrl = endPoint + "Admin/GetAllEmployeesByPagination?search=" + search + "&page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                    getBooksCountUrl = endPoint + "Admin/GetEmployeesCount?search=" + search;
                }
                else
                {
                    getBooksUrl = endPoint + "Admin/GetAllEmployeesByPagination?page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                    getBooksCountUrl = endPoint + "Admin/GetEmployeesCount";
                }

                ServiceResponse<int> countOfBooks = new ServiceResponse<int>();

                countOfBooks = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                    (getBooksCountUrl, HttpMethod.Get, HttpContext.Request);

                int totalCount = countOfBooks.Data;

                if (totalCount == 0)
                {
                    return View(new List<EmployeeViewModel>());
                }
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


                if (page > totalPages)
                {
                    return RedirectToAction("Index", new { search, sortBy, sortOrder, page = 1, pageSize });
                }
                ViewBag.SortBy = sortBy;
                ViewBag.SortOrder = sortOrder;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = totalPages;
                ServiceResponse<IEnumerable<EmployeeViewModel>> response = new ServiceResponse<IEnumerable<EmployeeViewModel>>();
                response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>
                    (getBooksUrl, HttpMethod.Get, HttpContext.Request);
                if (response.Success)
                {
                    return View(response.Data);
                }
                return View(new List<EmployeeViewModel>());
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Details(int employeeId)
        {
            try
            {
                var apiUrl = $"{endPoint}Admin/GetEmployeeById?id=" + employeeId;
                var response = _httpClientService.GetHttpResponseMessage<EmployeeViewModel>(apiUrl, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<EmployeeViewModel>>(data);
                    if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                    {
                        return View(serviceResponse.Data);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = serviceResponse?.Message;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    string errorData = response.Content.ReadAsStringAsync().Result;
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<EmployeeViewModel>>(errorData);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = errorResponse?.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong.Please try after sometime.";
                    }
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }


        }
        public IActionResult Delete(int id)
        {
            try
            {
                var apiUrl = $"{endPoint}Admin/RemoveEmployee?id=" + id;
                var response = _httpClientService.ExecuteApiRequest<ViewModels.ServiceResponse<string>>
                    ($"{apiUrl}", HttpMethod.Delete, HttpContext.Request);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = response.Message;
                    return RedirectToAction("Index");

                }
                else
                {

                    TempData["ErrorMessage"] = response.Message;
                    return RedirectToAction("Index");

                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var apiUrl = $"{endPoint}Admin/GetEmployeeById?id=" + id;
                ViewBag.SkillsSuggestions = new List<string>
            {
                "Web Forms", "MVC", ".Net Core", "Blazor", "Angular", "PHP", "Java", "React", "TypeScript", "Power BI", "MS SQL Server", "Oracle"
            };
                var response = _httpClientService.GetHttpResponseMessage<UpdateEmployeeViewModel>(apiUrl, HttpContext.Request);

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<UpdateEmployeeViewModel>>(data);
                    if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                    {
                        UpdateEmployeeViewModel viewModel = serviceResponse.Data;
                        return View(viewModel);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = serviceResponse.Message;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    string errorData = response.Content.ReadAsStringAsync().Result;
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<UpdateEmployeeViewModel>>(errorData);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = errorResponse.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                    }
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }
        //[Authorize]
        [HttpPost]

        public IActionResult Edit(UpdateEmployeeViewModel contact)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var apiUrl = $"{endPoint}Admin/UpdateEmployee";
                    HttpResponseMessage response = _httpClientService.PutHttpResponseMessage(apiUrl, contact, HttpContext.Request);
                    if (response.IsSuccessStatusCode)
                    {
                        string successResponse = response.Content.ReadAsStringAsync().Result;
                        var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                        TempData["SuccessMessage"] = serviceResponse.Message;
                        return RedirectToAction("Index");
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
                            TempData["ErrorMessage"] = "Something went wrong. Please try after sometime.";
                        }
                    }
                }


                return View(contact);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult GetEmployeesByJobRoleAndType(int? jobRoleId)
        {

            ViewBag.JobRole = GetJobRole();
            ViewBag.JobRoleId = jobRoleId;
            try
            {
                if (jobRoleId.HasValue)
                {
                    ViewBag.JobRoleId = jobRoleId.Value;
                    var apiUrl = $"{endPoint}Admin/GetEmployeesByJobRoleAndType?jobRoleId={jobRoleId}&typeId=1";
                    var response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);


                    if (response.Success)
                    {
                        return View(response.Data);
                    }
                    return View(new List<EmployeeViewModel>());


                }
                else
                {
                    ViewBag.JobRoleId = null;
                    return View();

                }

            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }


        }

        public IActionResult GetEmployeeData(string startDate, string? enddate = null)
        {

            ViewBag.JobRole = GetJobRole();
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = enddate;
            string todayDate = DateTime.Today.ToString("yyyy-MM-dd");

            try
            {
                if (startDate != null)
                {
                    ViewBag.StartDate = startDate;
                    var apiUrl = $"{endPoint}Admin/GetEmployeeData?startDate={startDate}&enddate={enddate}";
                    var response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);


                    if (response.Success)
                    {
                        return View(response.Data);
                    }
                    return View(new List<spViewModel>());


                }
                else
                {
                    ViewBag.StartDate = startDate;
                    var apiUrl = $"{endPoint}Admin/GetEmployeeData?startDate={todayDate}&enddate={enddate}";
                    var response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);


                    if (response.Success)
                    {
                        return View(response.Data);
                    }
                    return View(new List<spViewModel>());

                    ViewBag.JobRoleId = null;
                    return View();

                }

            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }


        }
        [ExcludeFromCodeCoverage]

        private List<JobRoleViewModel> GetJobRole()
        {

            ServiceResponse<IEnumerable<JobRoleViewModel>> response = new ServiceResponse<IEnumerable<JobRoleViewModel>>();
            string endPoint = _configuration["EndPoint:CivicaApi"];
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>
                ($"{endPoint}Admin/GetAllJobRoles", HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                return response.Data.ToList();
            }
            return new List<JobRoleViewModel>();



        }
        [HttpGet]
        public IActionResult AddEmployee()
        {
                var viewModel = new AddEmployeeViewModel();

                ViewBag.SkillsSuggestions = new List<string>
            {
                "Web Forms", "MVC", ".Net Core", "Blazor", "Angular", "PHP", "Java", "React", "TypeScript", "Power BI", "MS SQL Server", "Oracle"
            };

                return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddEmployee(AddEmployeeViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var apiUrl = $"{endPoint}Admin/AddEmployee";
                    var response = _httpClientService.PostHttpResponseMessage<AddEmployeeViewModel>(apiUrl, viewModel, HttpContext.Request);

                    if (response.IsSuccessStatusCode)
                    {
                        string data = response.Content.ReadAsStringAsync().Result;
                        var serviceResponse = JsonConvert.DeserializeObject<ViewModels.ServiceResponse<AddEmployeeViewModel>>(data);

                        if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                        {
                            return View(serviceResponse.Data);
                        }
                        else
                        {
                            TempData["SuccessMessage"] = serviceResponse?.Message;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        string errorData = response.Content.ReadAsStringAsync().Result;
                        var errorResponse = JsonConvert.DeserializeObject<ViewModels.ServiceResponse<AddEmployeeViewModel>>(errorData);

                        if (errorResponse != null)
                        {
                            TempData["ErrorMessage"] = errorResponse.Message;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                        }
                        return View(viewModel);
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
    }
}
