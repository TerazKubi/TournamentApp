using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentApp.Controllers;
using TournamentApp.Dto;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Tests.Controllers
{
    public class GameControllerTests
    {
        private readonly IGameRepository _gameRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IGameCommentRepository _gameCommentRepository;
        private readonly ISwissEliminationRepository _swissEliminationRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GameControllerTests()
        {
            _mapper = A.Fake<IMapper>();
            _gameRepository = A.Fake<IGameRepository>();
            _tournamentRepository = A.Fake<ITournamentRepository>();
            _teamRepository = A.Fake<ITeamRepository>();
            _gameCommentRepository = A.Fake<IGameCommentRepository>();
            _userManager = A.Fake<UserManager<User>>();
            _swissEliminationRepository = A.Fake<ISwissEliminationRepository>();
        }

        [Fact]
        public void GameController_GetGames_returnsOk()
        {
            //Arange
            var fakeGameList = A.Fake<List<Game>>();
            var fakeGameDtoList = A.Fake<List<GameNoDetailsDto>>();


            A.CallTo(() => _mapper.Map<List<GameNoDetailsDto>>(fakeGameList)).Returns(fakeGameDtoList);

            var controller = new GameController(_mapper, _gameRepository,
                _tournamentRepository, _teamRepository, _gameCommentRepository,       
                _userManager, _swissEliminationRepository);
            
            //Act
            var result = controller.GetGames();


            //Assert
            result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
        }


        [Fact]
        public void GameController_GetGameById_returnsOk()
        {
            //Arange
            var gameId = 1;
            var fakeGame = A.Fake<Game>();
            var fakeGameDto = A.Fake<GameDto>();


            A.CallTo(() => _gameRepository.GameExists(gameId)).Returns(true);
            A.CallTo(() => _mapper.Map<GameDto>(fakeGame)).Returns(fakeGameDto);

            var controller = new GameController(_mapper, _gameRepository,
                _tournamentRepository, _teamRepository, _gameCommentRepository,
                _userManager, _swissEliminationRepository);

            //Act
            var result = controller.GetGameById(gameId);


            //Assert
            result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
        }
    }
}
