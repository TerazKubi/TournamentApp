using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Controllers
{
    [Authorize]
    [Route("api/Games")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IGameCommentRepository _gameCommentRepository;
        private readonly ISwissEliminationRepository _swissEliminationRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GameController(IMapper mapper, IGameRepository gameRepository,
            ITournamentRepository tournamentRepository, ITeamRepository teamRepository, IGameCommentRepository gameCommentRepository, UserManager<User> userManager, ISwissEliminationRepository swissEliminationRepository)
        {
            _mapper = mapper;
            _gameRepository = gameRepository;
            _tournamentRepository = tournamentRepository;
            _teamRepository = teamRepository;
            _gameCommentRepository = gameCommentRepository;
            _userManager = userManager;
            _swissEliminationRepository = swissEliminationRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<GameNoDetailsDto>))]
        public IActionResult GetGames()
        {
            var games = _mapper.Map<List<GameNoDetailsDto>>(_gameRepository.GetGames());


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(games);
        }

        [HttpGet("{gameId}")]
        [ProducesResponseType(200, Type = typeof(GameDto))]
        [ProducesResponseType(400)]
        public IActionResult GetGameById(int gameId)
        {
            if (!_gameRepository.GameExists(gameId))
                return NotFound();

            var game = _mapper.Map<GameDto>(_gameRepository.GetGame(gameId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(game);
        }

        [HttpGet("{gameId}/gameComments")]
        [ProducesResponseType(200, Type = typeof(List<GameCommentDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetGameCommentByGameId(int gameId)
        {
            if (!_gameRepository.GameExists(gameId))
                return NotFound();

            var gameComments = _mapper.Map<List<GameCommentDto>>(_gameCommentRepository.GetGameCommentsByGameId(gameId));


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(gameComments);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateGame([FromBody] GameCreate gameCreate)
        {

            if (gameCreate == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_tournamentRepository.TournamentExists(gameCreate.TournamentId))
                return BadRequest(ModelState);


            // validate here like in updategame
            if (gameCreate.Team1Id != null && !_teamRepository.TeamExists((int)gameCreate.Team1Id))
            {
                ModelState.AddModelError("errorMessage", "team 1 doesnt exists");
                return BadRequest(ModelState);
            }

            if (gameCreate.Team2Id != null && !_teamRepository.TeamExists((int)gameCreate.Team2Id))
            {
                ModelState.AddModelError("errorMessage", "team 2 doesnt exists");
                return BadRequest(ModelState);
            }




            var gameMap = _mapper.Map<Game>(gameCreate);

            if (!_gameRepository.CreateGame(gameMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating game");
                return StatusCode(500, ModelState);
            }


            return Ok("Successfully created");
        }

        [Authorize(Roles = UserRoles.Referee)]
        [HttpPut("{gameId}/Addpoint/{teamId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult AddPoint(int gameId, int teamId)
        {
            if(!_gameRepository.GameExists(gameId) || !_teamRepository.TeamExists(teamId)) 
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest();

            var game = _gameRepository.GetGame(gameId);

            if (game.State == "finished")
                return BadRequest();

            if (game.Team1Id != teamId && game.Team2Id != teamId)
                return BadRequest();

            if(game.Team1Id == null || game.Team2Id == null)
                return BadRequest();

            if (game.State == "awaited") game.State = "ongoing";

            var tournament = _tournamentRepository.GetTournament(game.TournamentId);
            if(tournament != null)
            {
                if (tournament.State == "awaited") tournament.State = "ongoing";
                if(!_tournamentRepository.UpdateTournament(tournament))
                {
                    ModelState.AddModelError("", "Something went wrong updating tournament");
                    return StatusCode(500, ModelState);
                }
            }

            //add point
            AddPoint(game, teamId);

            //increment current set
            if (IsSetComplete(game)) IncrementCurrentSet(game, teamId);



            //check if game complete
            if (IsGameComplete(game)) 
            {       
                var result = HandleGameWinEvent(game, teamId, tournament.EliminationAlgorithm);
                if (result.StatusCode != 204) return result;
            }
            

            


            

            

            if (!_gameRepository.UpdateGame(game))
            {
                ModelState.AddModelError("", "Something went wrong updating game");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }



        [Authorize(Roles = UserRoles.Organizer)]
        [HttpPut("{gameId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateGame(int gameId, [FromBody] GameUpdate updateGame)
        {
            if (updateGame == null)
                return BadRequest(ModelState);

            if (gameId != updateGame.Id)
                return BadRequest(ModelState);

            if (!_gameRepository.GameExists(gameId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var gameToUpdate = _gameRepository.GetGame(gameId);
         
            gameToUpdate.StartDate = updateGame.StartDate;

            var currentUserId = _userManager.GetUserId(User);
            bool isAdmin = User.IsInRole(UserRoles.Admin);

            if (!(gameToUpdate.Tournament.Organizer.UserId.Equals(currentUserId) || isAdmin)) return Forbid();

            //var gameMap = _mapper.Map<Game>(updateGame);

            if (!_gameRepository.UpdateGame(gameToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong updating game");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private bool IsSetComplete(Game game)
        {
            int team1Points, team2Points;
            switch (game.CurrentSet)
            {
                case 1:
                    team1Points = game.Set1Team1Points;
                    team2Points = game.Set1Team2Points;
                    break;
                case 2:
                    team1Points = game.Set2Team1Points;
                    team2Points = game.Set2Team2Points;
                    break;
                case 3:
                    team1Points = game.Set3Team1Points;
                    team2Points = game.Set3Team2Points;
                    break;
                case 4:
                    team1Points = game.Set4Team1Points;
                    team2Points = game.Set4Team2Points;
                    break;
                case 5:
                    team1Points = game.Set5Team1Points;
                    team2Points = game.Set5Team2Points;
                    break;
                default:
                    return false;
            }

            return (team1Points >= 5 || team2Points >= 5) && Math.Abs(team1Points - team2Points) >= 2;
        }

        private bool IsGameComplete(Game game)
        {
            return game.Team1Sets == 3 || game.Team2Sets == 3;
        }

        private void AdvanceInBracket(Game game)
        {
            if (game.ParentId != null) return;

            if (!_gameRepository.GameExists((int)game.ParentId)) return;

            var parentGame = _gameRepository.GetGame((int)game.ParentId);

            if (parentGame.Team1Id == null)
            {
                parentGame.Team1Id = game.WinnerId;              
            }
            else if (parentGame.Team2Id == null)
            {
                parentGame.Team2Id = game.WinnerId;              
            }

            if(!_gameRepository.UpdateGame(parentGame)) return;

        }



        private void AddPoint(Game game, int teamId)
        {
            switch (game.CurrentSet)
            {
                case 1:
                    game.Set1Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set1Team2Points += (game.Team2Id == teamId) ? 1 : 0;

                    break;
                case 2:
                    game.Set2Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set2Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                case 3:
                    game.Set3Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set3Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                case 4:
                    game.Set4Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set4Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                case 5:
                    game.Set5Team1Points += (game.Team1Id == teamId) ? 1 : 0;
                    game.Set5Team2Points += (game.Team2Id == teamId) ? 1 : 0;
                    break;
                default:
                    return;
            }
        }

        private void IncrementCurrentSet(Game game, int teamId)
        {
            game.CurrentSet++;
            game.Team1Sets += (game.Team1Id == teamId) ? 1 : 0;
            game.Team2Sets += (game.Team2Id == teamId) ? 1 : 0;
        }

        private ObjectResult HandleGameWinEvent(Game game, int teamId, EliminationTypes eliminationAlgo)
        {
            if (eliminationAlgo == EliminationTypes.SingleElimination) return HandleSingleEliminationWin(game, teamId);
            else if (eliminationAlgo == EliminationTypes.DoubleElimination) return HandleDoubleEliminationWin(game, teamId);
            else if (eliminationAlgo == EliminationTypes.SwissElimination) return HandleSwissEliminationWin(game, teamId);

            return StatusCode(204, "");
        }

        private ObjectResult HandleSingleEliminationWin(Game game, int teamId)
        {
            game.State = "finished";
            game.WinnerId = teamId;

            if (game.ParentId != null)
            {
                if (!_gameRepository.GameExists((int)game.ParentId))
                {
                    ModelState.AddModelError("", "No parent game with id: " + (int)game.ParentId);
                    return StatusCode(400,ModelState);
                }

                var parentGame = _gameRepository.GetGame((int)game.ParentId);

                if (parentGame.Team1Id == null)
                {
                    parentGame.Team1Id = game.WinnerId;
                }
                else if (parentGame.Team2Id == null)
                {
                    parentGame.Team2Id = game.WinnerId;
                }

                //update parent game
                if (!_gameRepository.UpdateGame(parentGame))
                {
                    ModelState.AddModelError("", "Something went wrong updating parent game");
                    return StatusCode(500, ModelState);
                }
            }
            else
            {
                var tournament = _tournamentRepository.GetTournament(game.TournamentId);
                tournament.State = "finished";
                if (!_tournamentRepository.UpdateTournament(tournament))
                {
                    ModelState.AddModelError("", "Something went wrong updating tournament");
                    return StatusCode(500, ModelState);
                }
            }


            


            return StatusCode(204, "");
        }

        private ObjectResult HandleDoubleEliminationWin(Game game, int teamId)
        {
            game.State = "finished";
            game.WinnerId = teamId;

            int gameId = game.Id;

            //if not final
            if (game.ParentId != null)
            {
                if (!_gameRepository.GameExists((int)game.ParentId))
                {
                    ModelState.AddModelError("", "No parent game with id: " + (int)game.ParentId);
                    return StatusCode(400, ModelState);
                }

                var parentGame = _gameRepository.GetGame((int)game.ParentId);

                if (parentGame.Team1Id == null)
                {
                    parentGame.Team1Id = game.WinnerId;
                }
                else if (parentGame.Team2Id == null)
                {
                    parentGame.Team2Id = game.WinnerId;
                }

                //update parent game
                if (!_gameRepository.UpdateGame(parentGame))
                {
                    ModelState.AddModelError("", "Something went wrong updating parent game");
                    return StatusCode(500, ModelState);
                }

                //add loser to looser tree

                // id losera
                var looserId = teamId == game.Team1Id? game.Team2Id : game.Team1Id;

                var mainRoot = _tournamentRepository.GetTournamentRootGame(game.TournamentId);




                var rootLooserTree = mainRoot.Children[1];





                // jesli jestesmy w winner tree to loosera trzeba dac do looser tree
                if (game.IsWinnerTree == 1)
                {
                    AddTeamToLooserTree(rootLooserTree, (int)looserId);
                }

            }
            else
            {
                //if its FINAL REMATCH end tournament
                if(game.Team1Id == game.Children[0].Team1Id && game.Team2Id == game.Children[0].Team2Id) 
                {
                    var tournament = _tournamentRepository.GetTournament(game.TournamentId);
                    tournament.State = "finished";
                    if (!_tournamentRepository.UpdateTournament(tournament))
                    {
                        ModelState.AddModelError("", "Something went wrong updating tournament");
                        return StatusCode(500, ModelState);
                    }

                    return StatusCode(204, "");
                }
                //handle final game WIN if we need to create a rematch
                if(game.Team2Id == teamId)
                {
                    var oldRootGame = _tournamentRepository.GetTournamentRootGame(game.TournamentId);

                    var newRoot = new Game
                    {
                        TournamentId = game.TournamentId,
                        KeyCode = $"{Guid.NewGuid().ToString("N")}_{DateTime.Now.Ticks}",
                        Round = game.Round + 1,
                        Team1Id = game.Team1Id,
                        Team2Id = teamId
                    };

                    if (!_gameRepository.CreateGame(newRoot))
                    {
                        ModelState.AddModelError("", "Something went wrong updating tournament");
                        return StatusCode(500, ModelState);
                    }


                    oldRootGame.ParentId = newRoot.Id;
                    newRoot.Children.Add(oldRootGame);

                    _gameRepository.Save2();
                    
                    
                } 
                else //if no need for final rematch end tournament
                {
                    var tournament = _tournamentRepository.GetTournament(game.TournamentId);
                    tournament.State = "finished";
                    if (!_tournamentRepository.UpdateTournament(tournament))
                    {
                        ModelState.AddModelError("", "Something went wrong updating tournament");
                        return StatusCode(500, ModelState);
                    }
                }



                
            }

            return StatusCode(204, "");
        }

        private ObjectResult HandleSwissEliminationWin(Game game, int teamId)
        {
            game.State = "finished";
            game.WinnerId = teamId;

            var swissTableTeam1 = _swissEliminationRepository.GetSwissElimination(game.TournamentId, (int)game.Team1Id);
            var swissTableTeam2 = _swissEliminationRepository.GetSwissElimination(game.TournamentId, (int)game.Team2Id);

            var team1Sets = game.Team1Sets;
            var team2Sets = game.Team2Sets;
            Console.WriteLine($"\n\nt1 sets: {team1Sets}; t2sets: {team2Sets}\n\n");

            if ((team1Sets == 3 && team2Sets == 0) || (team1Sets == 0 && team2Sets == 3) ||
                (team1Sets == 3 && team2Sets == 1) || (team1Sets == 1 && team2Sets == 3))
            {
                if(game.WinnerId == game.Team1Id)
                {
                    swissTableTeam1.Points += 3;
                }
                if (game.WinnerId == game.Team2Id)
                {
                    swissTableTeam2.Points += 3;
                }
            }
            else if ((team1Sets == 3 && team2Sets == 2) || (team1Sets == 2 && team2Sets == 3))
            {
                if (game.WinnerId == game.Team1Id)
                {
                    swissTableTeam1.Points += 2;
                    swissTableTeam2.Points += 1;
                } 

                if (game.WinnerId == game.Team2Id)
                {
                    swissTableTeam2.Points += 2;
                    swissTableTeam1.Points += 1;
                }
                
            }

            if (!_swissEliminationRepository.UpdateSwissTable(swissTableTeam1))
            {
                ModelState.AddModelError("", "Something went wrong updating swiss table");
                return StatusCode(500, ModelState);
            }
            if (!_swissEliminationRepository.UpdateSwissTable(swissTableTeam2))
            {
                ModelState.AddModelError("", "Something went wrong updating swiss table");
                return StatusCode(500, ModelState);
            }

            //check if round has finished
            if (IsSwissRoundComplete(game.TournamentId, game.Round))
            {
                
                if (!HandleSwissRoundComplete(game.TournamentId, game.Round))
                    return StatusCode(500, "Something when wrong while handling completed round");
                
            }




            return StatusCode(204, "");
        }

        private bool AddTeamToLooserTree(Game game, int teamId)
        {
            if (game.Children.Count >= 1)
            {
                if (AddTeamToLooserTree(game.Children[0], teamId))
                    return true;
            }
            

            if (game.Children.Count == 0)
            {
                if(game.Team1Id == null)
                {
                    game.Team1Id = teamId;
                    _gameRepository.UpdateGame(game);
                    return true;
                }       
            }


            if (game.Children.Count == 2)
            {
                if (AddTeamToLooserTree(game.Children[1], teamId))
                    return true;
            }
            


            if (game.Children.Count <= 1)
            {
                if(game.Team2Id == null)
                {
                    game.Team2Id = teamId;
                    _gameRepository.UpdateGame(game);
                    return true;
                }
            }

            return false;
            

            
        }

        private bool IsSwissRoundComplete(int tournamentId, int currentRound)
        {
            var tournamentGames = _tournamentRepository.GetTournament(tournamentId).Games.ToList();
            var roundGames = tournamentGames.Where(g => g.Round == currentRound).ToList();

            return roundGames.All(g => g.WinnerId.HasValue);
        }

        private bool HandleSwissRoundComplete(int tournamentId, int currentRound)
        {
            var tournament = _tournamentRepository.GetTournament(tournamentId);
            var games = tournament.Games.ToList();

            int maxRound = games.Max(g => g.Round);

            if (currentRound == maxRound)
            {
                tournament.State = "finished";
                if (!_tournamentRepository.UpdateTournament(tournament)) return false;
                return true;
            }

            int newCurrentRound = currentRound + 1;

            var roundGames = games.Where(g => g.Round == newCurrentRound).ToList();

            var sortedTeams = GetTeamsSortedByPointsDesc(tournamentId);

            
            if(sortedTeams.Count % 2 != 0)
            {
                var swissTableSorted = GetSwissTableSortedAsc(tournamentId);
                Team TeamToPause = null;
                foreach( var swiss in swissTableSorted)
                {

                    if (swiss.HasPause == true) continue;
                    else
                    {
                        TeamToPause = swiss.Team;
                        break;
                    }
                }

                var swissTableRow = _swissEliminationRepository.GetSwissElimination(tournamentId, TeamToPause.Id);
                swissTableRow.HasPause = true;
                swissTableRow.Points += 3;

                if(!_swissEliminationRepository.UpdateSwissTable(swissTableRow)) return false;
                sortedTeams.Remove(TeamToPause);
            }

            foreach (var game in roundGames)
            {
                if (!(sortedTeams.Count >= 2)) break;

                game.Team1 = sortedTeams[0];
                game.Team1.Id = sortedTeams[0].Id;
                sortedTeams.RemoveAt(0);

                game.Team2 = sortedTeams[0];
                game.Team2.Id = sortedTeams[0].Id;
                sortedTeams.RemoveAt(0);
            }

            if (!_gameRepository.UpdateGamesFromList(roundGames)) return false;


            

            return true;
        }

        private List<Team> GetTeamsSortedByPointsDesc(int tournamentId)
        {
            var swissTeams = _swissEliminationRepository.GetSwissEliminationList(tournamentId);
            var swissTeamsSorted = swissTeams.OrderByDescending(s => s.Points).ToList();

            List<Team> teams = new List<Team>();
            foreach (var sts in swissTeamsSorted) 
            {
                teams.Add(sts.Team);
            }

            return teams;
        }

        private List<SwissElimination> GetSwissTableSortedAsc(int tournamentId)
        {
            var swissTeams = _swissEliminationRepository.GetSwissEliminationList(tournamentId);
            var swissTeamsSorted = swissTeams.OrderBy(s => s.Points).ToList();

            return swissTeamsSorted;
        }

    }
}
