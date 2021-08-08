using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moonpig.PostOffice.Api.Services;

namespace Moonpig.PostOffice.Api.Controllers
{
    [Route("api/[controller]")]
    public class DespatchDateController : Controller
    {
        private readonly ILogger<DespatchDateController> _logger;
        private readonly IDespatchService _despatchService;

        #region Constructor
        public DespatchDateController(ILogger<DespatchDateController> logger, IDespatchService despatchService)
        {
            _logger = logger;
            _despatchService = despatchService;
        } 
        #endregion

        [HttpGet]
        public async Task<IActionResult> Get(List<int> productIds, DateTime orderDate)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                    return BadRequest("Invalid Product Id.");

                if (orderDate == DateTime.MinValue || orderDate == DateTime.MaxValue)
                    return BadRequest("Invalid Order Date.");

                var despatchDate = await _despatchService.GetDespatchDateAsync(productIds, orderDate);
                return Ok(despatchDate);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get despatch date");

                // Thrown when product/supplier not found
                if (e is KeyNotFoundException)
                    return NotFound();

                // Throw a 500 error if we don't know what's wrong.
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
