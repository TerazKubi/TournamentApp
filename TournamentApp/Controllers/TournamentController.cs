using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TournamentApp.Dto;
using TournamentApp.Input;
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
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public TournamentController(IMapper mapper, ITournamentRepository tournamentRepository, ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _tournamentRepository = tournamentRepository;
            _teamRepository = teamRepository;
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
        public IActionResult CreateTournament([FromBody] TournamentCreateWrapper tournamentCreate)
        {

            if (tournamentCreate.Tournament == null || tournamentCreate.teamsIdList == null)
                return BadRequest(ModelState);

            if(tournamentCreate.teamsIdList.Count == 0)
                return BadRequest(ModelState);

            if(tournamentCreate.teamsIdList.Count != tournamentCreate.Tournament.TeamCount) 
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //check if all teams exists
            if(!_teamRepository.AllTeamsExists(tournamentCreate.teamsIdList)) 
                return BadRequest(ModelState);

            


            var tournamentMap = _mapper.Map<Tournament>(tournamentCreate.Tournament);
            tournamentMap.Teams = _teamRepository.GetTeamsFromList(tournamentCreate.teamsIdList);

            

            if (!_tournamentRepository.CreateTournament(tournamentMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating tournament");
                return StatusCode(500, ModelState);
            }

            tournamentMap.Games.Add(new Game()
            {
                KeyCode = "asdasdasdasd",
                TournamentId = tournamentMap.Id,
                Team1Id = tournamentCreate.teamsIdList[0],
                Team2Id = tournamentCreate.teamsIdList[1]
            });
            tournamentMap.Games.Add(new Game()
            {
                KeyCode = "dsaasdadadasdadasd",
                TournamentId = tournamentMap.Id,
            });

            if (!_tournamentRepository.UpdateTournament(tournamentMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating tournament");
                return StatusCode(500, ModelState);
            }


            return Ok("Successfully created");
        }

        [HttpDelete("{tournamentId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTournament(int tournamentId)
        {
            if (!_tournamentRepository.TournamentExists(tournamentId))
            {
                return NotFound();
            }

            var tournamentToDelete = _tournamentRepository.GetTournament(tournamentId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_tournamentRepository.DeleteTournament(tournamentToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting tournament");
            }

            return NoContent();
        }
    }
}
