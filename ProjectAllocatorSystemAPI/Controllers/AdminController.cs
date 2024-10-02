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
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            try
            {
                var response = _adminService.GetAllEmployees();
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


        [HttpGet("GetEmployeeById")]
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
                    var response = _adminService.GetEmployeeById(id);
                    return response.Success ? Ok(response) : NotFound(response);
                }

            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           

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
                var response = _adminService.TotalEmployees(search);
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
        [HttpPost("AddEmployee")]
        public IActionResult AddEmployee(AddEmployeeDto addEmployee)
        {
            try
            {
                var response = _adminService.AddEmployee(addEmployee);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           
        }
        [HttpPut("UpdateEmployee")]
        public IActionResult UpdateEmployee(UpdateEmployeeDto updateEmployee)
        {
            try
            {
                var response = _adminService.ModifyEmployee(updateEmployee);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);

            }
            catch (Exception ex) 
            {
                throw new Exception();
            }
           
        }
        [HttpDelete("RemoveEmployee")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                var response = _adminService.RemoveEmployee(id);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           
        }

        [HttpGet("GetEmployeesByJobRoleAndType")]
        public IActionResult GetEmployeesByJobRoleAndType(int jobRoleId, int typeId)
        {
            try
            {
                var response = _adminService.GetEmployeesByJobRoleAndType(jobRoleId, typeId);

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

        [HttpGet("GetAllJobRoles")]
        public IActionResult GetAllJobRoles()
        {
            try
            {
                var response = _adminService.GetAllJobRoles();
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

        [HttpGet("GetEmployeeData")]
        public IActionResult GetEmployeeData(string startDate, string? enddate = null)
        {
            try
            {
                var response = new ServiceResponse<IEnumerable<SPDto>>();

                response = _adminService.GetEmployeeData(startDate, enddate);
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

        [HttpPut("UpdateEmployees")]
        public IActionResult UpdateEmployees(UpdateAllocationDto updateEmployee)
        {
            try
            {
                var response = _adminService.UpdateEmployee(updateEmployee);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
           
        }
    }
}
