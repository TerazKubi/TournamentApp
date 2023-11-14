using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizerController : Controller
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IMapper _mapper;

        public OrganizerController(IOrganizerRepository organizerRepository, IMapper mapper)
        {
            _organizerRepository = organizerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<OrganizerDto>))]
        public IActionResult GetOrganizers()
        {
            var organizers = _mapper.Map<List<OrganizerDto>>(_organizerRepository.GetOrganizers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(organizers);
        }

        [HttpGet("{organizerId}")]
        [ProducesResponseType(200, Type = typeof(List<OrganizerDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetOrganizer(int organizerId)
        {
            if (!_organizerRepository.OrganizerExists(organizerId))
                return NotFound();


            var organizer = _mapper.Map<OrganizerDto>(_organizerRepository.GetOrganizerById(organizerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(organizer);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOrganizer([FromBody] OrganizerDto organizerCreate)
        {
            if (organizerCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (!_organizerRepository.OrganizerExists(organizerCreate.Id))
            //    return BadRequest(ModelState);



            var organizerMap = _mapper.Map<Organizer>(organizerCreate);


            if (!_organizerRepository.CreateOrganizer(organizerMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating organizer");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

    }
}
