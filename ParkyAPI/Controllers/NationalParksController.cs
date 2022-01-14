using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;


namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    //[Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository nationalParkRepository, IMapper mapper)
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
            var nationalParks = _npRepo.GetNationalParks();

            var nationalParksDto = _mapper.Map<ICollection<NationalParkDtos>>(nationalParks);

            return Ok(nationalParksDto);

        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="NationalParkId"> The Id of the national park</param>
        /// <returns></returns>

        [Authorize(Roles = "Admin")]
        [HttpGet("{NationalParkId:int}", Name = "GetNationalPark")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalParkDtos))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int NationalParkId)
        {
            var Obj = _npRepo.GetNationalPark(NationalParkId);

            if (Obj == null)
                return NotFound();

            var ObjDto = _mapper.Map<NationalParkDtos>(Obj);
            return Ok(ObjDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NationalParkDtos))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CreateNationalPark([FromBody] NationalParkDtos nationalParkDtos)
        {
            if (nationalParkDtos == null)
            {
                return BadRequest(ModelState);
            }

            if (_npRepo.NationalParkExists(nationalParkDtos.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDtos);

            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went worng when saving the record{nationalParkDtos.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { version = HttpContext.GetRequestedApiVersion().ToString(), NationalParkId = nationalParkObj.Id }, nationalParkObj);
        }

        [HttpPatch("{NationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int NationalParkId, [FromBody] NationalParkDtos nationalParkDtos)
        {
            if (nationalParkDtos == null || NationalParkId != nationalParkDtos.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDtos);

            if (!_npRepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkDtos.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{NationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int NationalParkId)
        {
            if (!_npRepo.NationalParkExists(NationalParkId))
            {
                return NotFound();
            }

            var nationalParkObj = _npRepo.GetNationalPark(NationalParkId);

            if (!_npRepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went worng when deleting record {nationalParkObj.Name}");
                StatusCode(500, ModelState);
            }


            return NoContent();
        }
    }
}
