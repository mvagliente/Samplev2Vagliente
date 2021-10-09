using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.API.Data;
using App.API.Models;
using App.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OfficeController : ControllerBase
    {
        private readonly ICrudService<Office> _officeService;
        private readonly ILogger<OfficeController> _logger;
        public OfficeController(ICrudService<Office> officeService, ILogger<OfficeController> logger)
        {
            _officeService = officeService;
            _logger = logger;
        }

        [HttpGet]
        [Route("getOffices")]
        public async Task<IActionResult> GetOffices(string searchPattern)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchPattern))
                    return Ok(_officeService.Where(x => x.Address.Contains(searchPattern.ToUpper())).ToArray());
                else
                    return Ok(_officeService.GetAll().ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
