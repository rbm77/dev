using Buslogix.Interfaces;
using Buslogix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buslogix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController(ICompanyService companyService) : ControllerBase
    {
        private readonly ICompanyService _companyService = companyService;

        [Authorize(Policy = "Company.Read")]
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            List<Company> companies = await _companyService.GetCompanies();
            return Ok(companies);
        }
    }
}
