using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiVersion("2.0")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksV2Controller : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksV2Controller(INationalParkRepository nationalParkRepository, IMapper mapper)
        {
            this._npRepo = nationalParkRepository;
            this._mapper = mapper;
        }

        /// <summary>
        /// Get list of all national parks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDtos>))]
        public IActionResult GetNationalParks()
        {
            var nationalParks = _npRepo.GetNationalParks().FirstOrDefault();

            var nationalParksDto = _mapper.Map<NationalParkDtos>(nationalParks);

            return Ok(nationalParksDto);

        }

    }
}
