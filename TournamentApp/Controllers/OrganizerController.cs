using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/Organizers")]
    [ApiController]
    public class OrganizerController : Controller
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public OrganizerController(IOrganizerRepository organizerRepository, IMapper mapper, ITournamentRepository tournamentRepository, IUserRepository userRepository)
        {
            _organizerRepository = organizerRepository;
            _mapper = mapper;
            _tournamentRepository = tournamentRepository;
            _userRepository = userRepository;
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
        [HttpGet("{organizerId}/tournaments")]
        [ProducesResponseType(200, Type = typeof(List<TournamentDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetOrganizerTournaments(int organizerId)
        {
            if (!_organizerRepository.OrganizerExists(organizerId))
                return NotFound();


            var tournaments = _mapper.Map<List<TournamentDto>>(_tournamentRepository.GetTournamentsByOrganizerId(organizerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tournaments);
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

            if(!_userRepository.UserExists(organizerCreate.UserId)) 
                return BadRequest(ModelState);



            var organizerMap = _mapper.Map<Organizer>(organizerCreate);

            var user = _userRepository.GetUser(organizerCreate.UserId);

            organizerMap.User = user;


            if (!_organizerRepository.CreateOrganizer(organizerMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating organizer");
                return StatusCode(500, ModelState);
            }

            user.Organizer = organizerMap;
            user.OrganizerId = organizerMap.Id;

            if (!_userRepository.UpdateUser(user))
            {
                ModelState.AddModelError("", "Something went wrong while updating User while creating Organizer");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

    }
}
