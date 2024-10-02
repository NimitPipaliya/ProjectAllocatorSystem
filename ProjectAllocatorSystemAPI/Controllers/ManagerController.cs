using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Service.Contract;
using ProjectAllocatorSystemAPI.Service.Implementation;

namespace ProjectAllocatorSystemAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }


        [HttpGet("GetAllEmployeesByPagination")]
        public IActionResult GetPaginatedEmployees(string? search, string? sortBy, int page = 1, int pageSize = 4, string sortOrder = "asc")
        {
            try
            {
                var response = new ServiceResponse<IEnumerable<EmployeeDto>>();
                response = _managerService.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);
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

        [HttpGet("GetEmployeeById/{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Please enter valid data.");
                }
                else
                {
                    var response = _managerService.GetAllocationByEmpId(id);
                    return response.Success ? Ok(response) : NotFound(response);
                }
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           

        }
        [HttpGet("GetEmployeesCount")]
        public IActionResult GetTotalCountOfContacts(string? search)
        {
            try
            {
                var response = _managerService.TotalEmployees(search);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch
            {
                throw new Exception();
            }
           
           
        }
    }
}
