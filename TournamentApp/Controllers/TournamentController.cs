using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TournamentApp.Dto;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Route("api/Tournaments")]
    [ApiController]
    public class TournamentController : Controller
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IMapper _mapper;

        public TournamentController(IMapper mapper, ITournamentRepository tournamentRepository)
        {
            _mapper = mapper;
            _tournamentRepository = tournamentRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TournamentDto>))]
        public IActionResult GetTournaments()
        {
            var tournaments = _mapper.Map<List<TournamentDto>>(_tournamentRepository.GetTournaments());


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tournaments);
        }

        [HttpGet("{tournamentId}")]
        [ProducesResponseType(200, Type = typeof(TournamentDto))]
        [ProducesResponseType(400)]
        public IActionResult GetTeamById(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
                return NotFound();

            var tournament = _mapper.Map<TournamentDto>(_tournamentRepository.GetTournament(tournamentId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(tournament);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTeam([FromBody] TournamentDto tournamentCreate)
        {

            if (tournamentCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //if (!_tournamentRepository.TournamentExists(tournamentCreate.Id))
            //    return BadRequest(ModelState);

            


            var tournamentMap = _mapper.Map<Tournament>(tournamentCreate);

            if (!_tournamentRepository.CreateTournament(tournamentMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating tournament");
                return StatusCode(500, ModelState);
            }

            //var user = _userRepository.GetUser(teamCreate.UserId);
            //user.Team = teamMap;
            //user.TeamId = teamMap.Id;

            //if (!_userRepository.UpdateUser(user))
            //{
            //    ModelState.AddModelError("", "Something went wrong while updating User while creating Team");
            //    return StatusCode(500, ModelState);
            //}

            return Ok("Successfully created");
        }
    }
}
