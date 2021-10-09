using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.API.Models;
using App.API.Models.Security;
using App.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OfficeController> _logger;

        public UserController(UserManager<ApplicationUser> userManager, ILogger<OfficeController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("getUsers")]
        public async Task<IActionResult> GetUsers(string officeIds)
        {

            try
            {
                var ids = officeIds
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => Guid.Parse(o))
                    .ToList();

                var users = this._userManager.Users
                    .Where(o => ids.Contains((Guid)o.officeId))
                    .ToList();


                users.ForEach(t => _userManager.GetRolesAsync(t));

                return Ok(users.ToArray());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
