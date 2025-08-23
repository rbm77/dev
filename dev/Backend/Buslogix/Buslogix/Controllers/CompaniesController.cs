using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompaniesController(ICompanyService companyService) : ControllerBase
    {

        private readonly ICompanyService _companyService = companyService;

        [Authorize(Policy = $"{Resources.COMPANY}.{PermissionMode.READ}")]
        [HttpGet]
        public async Task<IActionResult> GetCompany()
        {
            Company? company = await _companyService.GetCompany();
            return company == null ? NotFound() : Ok(company);
        }
    }
}
