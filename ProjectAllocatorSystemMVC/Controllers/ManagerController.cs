using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectAllocatorSystemMVC.Infrastructure;
using ProjectAllocatorSystemMVC.ViewModels;

namespace ProjectAllocatorSystemMVC.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;


        public ManagerController(IHttpClientService _httpClientService, IConfiguration _configuration)
        {
            this._httpClientService = _httpClientService;
            this._configuration = _configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }
        public IActionResult Index(string? search, string? sortBy, string sortOrder = "asc", int page = 1, int pageSize = 2)
        {
            var getBooksUrl = "";
            var getBooksCountUrl = "";
            try { 
            ViewBag.Search = search;
            if (search != null && sortBy != null && search.Length > 2)
            {
                getBooksUrl = endPoint + "Manager/GetAllEmployeesByPagination?search=" + search + "&sortBy=" + sortBy + "&page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                getBooksCountUrl = $"{endPoint}Manager/GetEmployeesCount?search={search}";
            }
            else if (search == null && sortBy != null)
            {
                getBooksUrl = endPoint + "Manager/GetAllEmployeesByPagination?sortBy=" + sortBy + "&page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                getBooksCountUrl = endPoint + "Manager/GetEmployeesCount";
            }
            else if (search != null && search.Length > 2 && sortBy == null)
            {
                getBooksUrl = endPoint + "Manager/GetAllEmployeesByPagination?search=" + search + "&page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                getBooksCountUrl = endPoint + "Manager/GetEmployeesCount?search=" + search;
            }
            else
            {
                getBooksUrl = endPoint + "Manager/GetAllEmployeesByPagination?page=" + page + "&pageSize=" + pageSize + "&sortOrder=" + sortOrder;
                getBooksCountUrl = endPoint + "Manager/GetEmployeesCount";
            }

            ServiceResponse<int> countOfBooks = new ServiceResponse<int>();

            countOfBooks = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (getBooksCountUrl, HttpMethod.Get, HttpContext.Request);

            int totalCount = countOfBooks.Data;

            if (totalCount == 0)
            {
                return View(new List<ManagerListViewModel>());
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
            ServiceResponse<IEnumerable<ManagerListViewModel>> response = new ServiceResponse<IEnumerable<ManagerListViewModel>>();
            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<ManagerListViewModel>>>
                (getBooksUrl, HttpMethod.Get, HttpContext.Request);
            if (response.Success)
            {
                return View(response.Data);
            }
            return View(new List<ManagerListViewModel>());
            }
            catch
            {
                TempData["ErrorMessage"] = "An unexpected error occured, Please try again later !!";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult AllocateTask(int id)
        {
            try { 
            var apiUrl = $"{endPoint}Admin/GetEmployeeById?id=" + id;
            var response = _httpClientService.GetHttpResponseMessage<ManagerAllocationViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<ManagerAllocationViewModel>>(data);

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
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<ManagerAllocationViewModel>>(errorData);

                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse?.Message;
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
        [HttpPost]
        public IActionResult AllocateTask(ManagerAllocationViewModel viewModel)
        {
            try { 
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Allocator/Create";
                var response = _httpClientService.PostHttpResponseMessage<ManagerAllocationViewModel>(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successMessage = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<ManagerAllocationViewModel>>(successMessage);

                    TempData["SuccessMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorMessage = response.Content.ReadAsStringAsync().Result;
                    var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<ManagerAllocationViewModel>>(errorMessage);
                    if (errorResponse != null)
                    {
                        TempData["ErrorMessage"] = errorResponse.Message;

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong. Please try after sometime";
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
        [HttpGet]
        public IActionResult EmployeeDetails(int id)
        {
            try { 
            var apiUrl = $"{endPoint}Manager/GetEmployeeById/" + id;
            var response = _httpClientService.GetHttpResponseMessage<EmployeeManagerDetails>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<EmployeeManagerDetails>>(data);

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
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<EmployeeManagerDetails>>(errorData);

                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse?.Message;
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
    }
}
