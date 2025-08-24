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
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            if (id != HttpContext.GetCompanyId())
            {
                return Forbid();
            }

            Company? company = await _companyService.GetCompany(id);
            return company == null ? NotFound() : Ok(company);
        }

        [Authorize(Policy = $"{Resources.COMPANY}.{PermissionMode.WRITE}")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] Company company)
        {
            if (id != HttpContext.GetCompanyId())
            {
                return Forbid();
            }

            bool updated = await _companyService.UpdateCompany(id, company);
            return updated ? NoContent() : NotFound();
        }
    }
}
