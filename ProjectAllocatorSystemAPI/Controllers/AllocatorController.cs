using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using System.Diagnostics.CodeAnalysis;

namespace ProjectAllocatorSystemAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AllocatorController : ControllerBase
    {
        private readonly IAllocatorService _adminService;

        public AllocatorController(IAllocatorService adminService)
        {
            _adminService = adminService;
        }
        [HttpGet("GetAllEmployeesByPagination")]
        public IActionResult GetPaginatedEmployees(string? search, string? sortBy, int page = 1, int pageSize = 4, string sortOrder = "asc")
        {
            try
            {
                var response = new ServiceResponse<IEnumerable<EmployeeDto>>();
                response = _adminService.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);
                if (!response.Success)
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
           
        }
        [HttpPost("Create")]
        public IActionResult AddEmployee(AddEmployeeAllocationDto addemployeeallocationDto)
        {
            try
            {
                var employee = new Allocation()

                {
                    EmployeeId = addemployeeallocationDto.EmployeeId,
                    TypeId = addemployeeallocationDto.TypeId,
                    StartDate = addemployeeallocationDto.StartDate,
                    EndDate = addemployeeallocationDto.EndDate,
                    Details = addemployeeallocationDto.Details,
                    TrainingId = addemployeeallocationDto.TrainingId,
                    InternalProjectId = addemployeeallocationDto.InternalProjectId

                };

                var result = _adminService.AddAllocation(employee);
                return !result.Success ? BadRequest(result) : Ok(result);

            }
            catch(Exception ex)
            {
                throw new Exception();
            }

           

        }
    }
}
