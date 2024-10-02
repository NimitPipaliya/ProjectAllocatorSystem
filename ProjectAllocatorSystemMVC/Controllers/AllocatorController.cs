using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProjectAllocatorSystemMVC.Attributes;
using ProjectAllocatorSystemMVC.Infrastructure;
using ProjectAllocatorSystemMVC.ViewModels;
using System.Diagnostics.Metrics;

namespace ProjectAllocatorSystemMVC.Controllers
{
    public class AllocatorController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;
        public AllocatorController(IHttpClientService httpClientService, IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];

        }
        public IActionResult Index(string? search, string? sortBy, string sortOrder = "asc", int page = 1, int pageSize = 2)
        {
            var getBooksUrl = "";
            var getBooksCountUrl = "";
            try
            {
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
                return View(new List<AllocatorListViewModel>());
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
            ServiceResponse<IEnumerable<AllocatorListViewModel>> response = new ServiceResponse<IEnumerable<AllocatorListViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<AllocatorListViewModel>>>
                (getBooksUrl, HttpMethod.Get, HttpContext.Request);
            if (response.Success)
            {
                return View(response.Data);
            }
            return View(new List<AllocatorListViewModel>());
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllocatorAuthorize]
        [HttpGet]
        public IActionResult AddAllocation(int id)
        {
            try
            {
                var apiUrl = $"{endPoint}Admin/GetEmployeeById?id=" + id;
                var response = _httpClientService.GetHttpResponseMessage<AddAllocationViewModel>(apiUrl, HttpContext.Request);

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<AddAllocationViewModel>>(data);

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
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<AddAllocationViewModel>>(errorData);

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

        [AllocatorAuthorize]
        [HttpPost]
        public IActionResult AddAllocation(AddAllocationViewModel addAllocationViewModel)
        {
            try 
            { 
            //string endPoint = _configuration["EndPoint:CivicaApi"];
            var apiUrl = $"{endPoint}Allocator/Create";

            HttpResponseMessage responseMessage = _httpClientService.PostHttpResponseMessage(apiUrl, addAllocationViewModel, HttpContext.Request);

            if (responseMessage.IsSuccessStatusCode)
            {
                string successMessage = responseMessage.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<AddAllocationViewModel>>(successMessage);
                UpdateEmployee(addAllocationViewModel);
                TempData["SuccessMessage"] = serviceResponse.Message;
                return RedirectToAction("Index");
            }
            else
            {
                string errorMessage = responseMessage.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorMessage);
                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse?.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong, please try after sometime.";
                }
            }

            return View(addAllocationViewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllocatorAuthorize]
        [HttpGet]
        public IActionResult SetBenchForm(int id)
        {
            try
            {
            var apiUrl = $"{endPoint}Admin/GetEmployeeById?id=" + id;
            var response = _httpClientService.GetHttpResponseMessage<AddAllocationViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<AddAllocationViewModel>>(data);

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
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<AddAllocationViewModel>>(errorData);

                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong please try after some time";
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

        [HttpPost]
        private IActionResult UpdateEmployee(AddAllocationViewModel addAllocationViewModel)
        {
            try 
            { 
            //string endPoint = _configuration["EndPoint:CivicaApi"];
            var apiUrl = $"{endPoint}Admin/UpdateEmployees";

            HttpResponseMessage responseMessage = _httpClientService.PutHttpResponseMessage(apiUrl, addAllocationViewModel, HttpContext.Request);

            if (responseMessage.IsSuccessStatusCode)
            {
                string successMessage = responseMessage.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<AddAllocationViewModel>>(successMessage);

                TempData["SuccessMessage"] = serviceResponse.Message;
                return RedirectToAction("Index");
            }
            else
            {
                string errorMessage = responseMessage.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorMessage);
                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse?.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong, please try after sometime.";
                }
            }

            return View(addAllocationViewModel);
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
