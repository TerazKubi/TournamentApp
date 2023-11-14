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
    public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public TeamController(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TeamDto>))]
        public IActionResult GetTeams()
        {
            var teams = _mapper.Map<List<TeamDto>>(_teamRepository.GetTeams());


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(teams);
        }

        [HttpGet("{teamId}")]
        [ProducesResponseType(200, Type = typeof(TeamDto))]
        [ProducesResponseType(400)]
        public IActionResult GetTeamById(int teamId)
        {
            if (!_teamRepository.TeamExists(teamId))
                return NotFound();

            var team = _mapper.Map<TeamDto>(_teamRepository.GetById(teamId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(team);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTeam([FromBody] TeamDto teamCreate)
        {
            
            if (teamCreate == null || teamCreate.UserId == 0)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // check if team already exists
            //if (!_userRepository.UserExists(postCreate.AuthorId))
            //    return BadRequest(ModelState);


            var teamMap = _mapper.Map<Team>(teamCreate);



            if (!_teamRepository.CreateTeam(teamMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating team");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
    }
}
