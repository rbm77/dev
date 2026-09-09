using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Features.Companies
{
    [Route("[controller]")]
    [ApiController]
    public class CompaniesController(ICompanyRepository companyRepository) : ControllerBase
    {

        [Authorize(Policy = $"{Resources.COMPANY}.{PermissionMode.READ}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            if (id != HttpContext.GetCompanyId())
            {
                return Forbid();
            }

            Company? company = await companyRepository.GetCompany(id);
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

            bool updated = (await companyRepository.UpdateCompany(id, company)) > 0;
            return updated ? NoContent() : NotFound();
        }
    }
}
