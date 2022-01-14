using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Repository.IRepository;
using AutoMapper;
using System.Collections.Generic;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Models;
using Microsoft.AspNetCore.Authorization;
namespace ParkyAPI.Controllers
{

    [Route("api/v{version:apiVersion}/Trails")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepository, IMapper mapper)
        {
            _trailRepository = trailRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of all trails
        /// </summary>
        /// <returns></returns>

        [HttpGet(Name = "GetTrails")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrailDtos>))]
        public IActionResult GetTrails()
        {
            var trails = _trailRepository.GetTrails();

            var trailsDto = _mapper.Map<List<Trail>>(trails);

            return Ok(trailsDto);
        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="TrailId">The Id of the trail</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("{TrailId:int}", Name = "GetTrail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Trail))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTrail(int TrailId)
        {
            var trail = _trailRepository.GetTrail(TrailId);

            if (trail == null)
                return NotFound();

            var trilDto = _mapper.Map<TrailDtos>(trail);

            return Ok(trilDto);

        }

        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDtos))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrilGetTrailsInNationalPark(int nationalParkId)
        {
            var trail = _trailRepository.GetTrailsInNationalPark(nationalParkId);
            if (trail == null)
            {
                return NotFound();
            }

            var trailDtos = _mapper.Map<List<TrailDtos>>(trail);

            return Ok(trailDtos);

        }

        [HttpPost(Name = "CreateTrail")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailInsertDto trailInsertDto)
        {

            if (trailInsertDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepository.TrailExists(trailInsertDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            var trail = _mapper.Map<Trail>(trailInsertDto);

            if (!_trailRepository.CreateTrail(trail))
            {
                ModelState.AddModelError("", $"Something went worng when saving the record{trailInsertDto.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { TrailId = trail.Id }, trail);
        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailUpdateDto)
        {
            if (trailUpdateDto == null || trailId != trailUpdateDto.Id)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailUpdateDto);

            if (!_trailRepository.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating record {trailUpdateDto.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepository.TrailExists(trailId))
            {
                return NotFound();
            }

            var trilObj = _trailRepository.GetTrail(trailId);
            if (!_trailRepository.DeleteTrail(trilObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting record{trilObj.Name}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
