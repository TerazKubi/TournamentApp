﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers

{
    [Route("api/Teams")]
    [ApiController]
    public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        public TeamController(ITeamRepository teamRepository, IMapper mapper, IUserRepository userRepository, IPostRepository postRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _postRepository = postRepository;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TeamNoDetailsDto>))]
        public IActionResult GetTeams()
        {
            var teams = _mapper.Map<List<TeamNoDetailsDto>>(_teamRepository.GetTeams());


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

        [HttpGet("{teamId}/players")]
        [ProducesResponseType(200, Type = typeof(List<PlayerDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetTeamPlayers(int teamId)
        {
            if (!_teamRepository.TeamExists(teamId))
                return NotFound();

            var players = _mapper.Map<List<PlayerDto>>(_teamRepository.GetPlayers(teamId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(players);
        }

        [HttpGet("{teamId}/posts")]
        [ProducesResponseType(200, Type = typeof(List<PostDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetTeamPostss(int teamId)
        {
            if (!_teamRepository.TeamExists(teamId))
                return NotFound();

            var posts = _mapper.Map<List<PostDto>>(_postRepository.GetTeamPosts(teamId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(posts);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTeam([FromBody] TeamCreate teamCreate)
        {
            
            if (teamCreate == null || teamCreate.UserId == 0)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.UserExists(teamCreate.UserId))
                return BadRequest(ModelState);

            // check if team already exists TODO

            // check if given user is player if yes return bad request
            


            var teamMap = _mapper.Map<Team>(teamCreate);

            if (!_teamRepository.CreateTeam(teamMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating team");
                return StatusCode(500, ModelState);
            }

            var user = _userRepository.GetUser(teamCreate.UserId);
            user.Team = teamMap;
            user.TeamId = teamMap.Id;

            if (!_userRepository.UpdateUser(user))
            {
                ModelState.AddModelError("", "Something went wrong while updating User while creating Team");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
    }
}