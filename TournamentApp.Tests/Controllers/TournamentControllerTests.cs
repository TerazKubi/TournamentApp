using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TournamentApp.Controllers;
using TournamentApp.Dto;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;

namespace TournamentApp.Tests.Controllers
{
    public class TournamentControllerTests
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ISwissEliminationRepository _swissRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public TournamentControllerTests()
        {
            _tournamentRepository = A.Fake<ITournamentRepository>();
            _teamRepository = A.Fake<ITeamRepository>(); ;
            _organizerRepository = A.Fake<IOrganizerRepository>(); ;
            _gameRepository = A.Fake<IGameRepository>(); ;
            _swissRepository = A.Fake<ISwissEliminationRepository>();
            _mapper = A.Fake<IMapper>();
            _userManager = A.Fake<UserManager<User>>();
        }

        [Fact]
        public void TournamentController_GetTournaments_ReturnOk()
        {
            //Arrange
            var tournaments = A.Fake<ICollection<Tournament>>();
            var tournamentList = A.Fake<List<TournamentDto>>();
            A.CallTo(() => _mapper.Map<List<TournamentDto>>(tournaments))
                .Returns(tournamentList);

            var controller = new TournamentController(_mapper, _tournamentRepository,
               _teamRepository, _organizerRepository,
               _gameRepository, _userManager, _swissRepository);


            //Act
            var result = controller.GetTournaments();


            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }



        [Fact]
        public void TournamentController_GetTournamentById_ReturnsOk()
        {
            // Arrange
            var fakeTournamentId = 1;

            var fakeTournament = A.Fake<Tournament>();
            fakeTournament.Id = fakeTournamentId;
            var fakeTournamentDto = A.Fake<TournamentDto>();
            fakeTournamentDto.Id = fakeTournamentId;

            A.CallTo(() => _mapper.Map<TournamentDto>(fakeTournament))
                .Returns(fakeTournamentDto);

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(true);

            A.CallTo(() => _tournamentRepository.GetTournament(fakeTournamentId))
                .Returns(fakeTournament);


            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);

            // Act
            var result = controller.GetTournamentById(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var tournamentDto = okResult.Value as TournamentDto;
            tournamentDto.Should().NotBeNull();

            tournamentDto.Id.Should().Be(fakeTournament.Id);
        }



        [Fact]
        public void TournamentController_GetTournamentById_ReturnsNotFound()
        {
            // Arrange
            var fakeTournamentId = 0;

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(false);

            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);


            // Act
            var result = controller.GetTournamentById(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<NotFoundResult>();

            var okResult = result as NotFoundResult;
            okResult.Should().NotBeNull();
        }


        [Fact]
        public void TournamentController_GetTournamentRootGame_ReturnsOk()
        {
            //Arange
            var fakeTournamentId = 1;

            var fakeTournament = A.Fake<Tournament>();
            fakeTournament.Id = fakeTournamentId;
            fakeTournament.EliminationAlgorithm = EliminationTypes.SingleElimination;
            
            var fakeGame = A.Fake<Game>();
            fakeGame.TournamentId = fakeTournamentId;
            var fakeGameNode = A.Fake<GameNode>();
            fakeGameNode.TournamentId = fakeTournamentId;

            A.CallTo(() => _mapper.Map<GameNode>(fakeGame))
                .Returns(fakeGameNode);

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(true);

            A.CallTo(() => _tournamentRepository.GetTournament(fakeTournamentId))
                .Returns(fakeTournament);

            A.CallTo(() => _tournamentRepository.GetTournamentRootGame(fakeTournamentId))
                .Returns(fakeGame);


            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);

            // Act
            var result = controller.GetTournamentRootGame(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var gameNode = okResult.Value as GameNode;
            gameNode.Should().NotBeNull();

            gameNode.TournamentId.Should().Be(fakeTournament.Id);
        }

        [Fact]
        public void TournamentController_GetTournamentRootGame_ReturnsBadRequest()
        {
            //Arange
            var fakeTournamentId = 1;

            var fakeTournament = A.Fake<Tournament>();
            fakeTournament.Id = fakeTournamentId;
            fakeTournament.EliminationAlgorithm = EliminationTypes.SwissElimination; 

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(true);

            A.CallTo(() => _tournamentRepository.GetTournament(fakeTournamentId))
                .Returns(fakeTournament);

            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);


            // Act
            var result = controller.GetTournamentRootGame(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();

        }

        [Fact]
        public void TournamentController_GetTournamentRootGame_ReturnsNotFound()
        {
            //Arange
            var fakeTournamentId = 1;

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(false);

            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);


            // Act
            var result = controller.GetTournamentRootGame(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<NotFoundResult>();

            var okResult = result as NotFoundResult;
            okResult.Should().NotBeNull();

        }


        [Fact]
        public void TournamentController_GetTournamentSwissTable_ReturnsOk()
        {
            //Arange
            var fakeTournamentId = 1;

            var fakeTournament = A.Fake<Tournament>();
            fakeTournament.Id = fakeTournamentId;
            fakeTournament.EliminationAlgorithm = EliminationTypes.SwissElimination;

            var fakeSwissList = A.Fake<List<SwissElimination>>();
            var fakeSwissDtoList = A.Fake<List<SwissEliminationDto>>();

            A.CallTo(() => _mapper.Map<List<SwissEliminationDto>>(fakeSwissList))
                .Returns(fakeSwissDtoList);

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(true);

            A.CallTo(() => _tournamentRepository.GetTournament(fakeTournamentId))
                .Returns(fakeTournament);

            A.CallTo(() => _swissRepository.GetSwissEliminationList(fakeTournamentId))
                .Returns(fakeSwissList);


            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);

            // Act
            var result = controller.GetTournamentSwissTable(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var swissListResult = okResult.Value as List<SwissEliminationDto>;
            swissListResult.Should().NotBeNull();

        }


        [Fact]
        public void TournamentController_GetTournamentSwissTable_ReturnsBadRequest()
        {
            //Arange
            var fakeTournamentId = 1;

            var fakeTournament = A.Fake<Tournament>();
            fakeTournament.Id = fakeTournamentId;
            fakeTournament.EliminationAlgorithm = EliminationTypes.SingleElimination;



            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(true);

            A.CallTo(() => _tournamentRepository.GetTournament(fakeTournamentId))
                .Returns(fakeTournament);



            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);

            // Act
            var result = controller.GetTournamentSwissTable(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();


        }

        [Fact]
        public void TournamentController_GetTournamentSwissTable_ReturnsNotFound()
        {
            //Arange
            var fakeTournamentId = 0;

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(false);

            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);

            // Act
            var result = controller.GetTournamentSwissTable(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<NotFoundResult>();

            var okResult = result as NotFoundResult;
            okResult.Should().NotBeNull();


        }

        [Theory]
        [InlineData("userGUID", "organizerUserGUID", false, false)]
        [InlineData("userGUID", "organizerUserGUID", true, true)]
        [InlineData("sameUserGUID", "sameUserGUID", false, true)]
        [InlineData("sameUserGUID", "sameUserGUID", true, true)]
        public void TournamentController_DeleteTournament_IdentityTest_ReturnsOk(string curremtUserGUID, string organizerUserGUID, bool isAdmin, bool isOk)
        {
            //Arange
            var fakeTournamentId = 1;

            var fakeTournamentToDelete = A.Fake<Tournament>();
            fakeTournamentToDelete.Id = fakeTournamentId;

            //var currentUserId = "fake GUID";
            fakeTournamentToDelete.Organizer = A.Fake<Organizer>();
            fakeTournamentToDelete.Organizer.UserId = organizerUserGUID;
            //var isAdmin = true;

            var fakeCurrentUser = A.Fake<ClaimsPrincipal>();
            fakeCurrentUser.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, curremtUserGUID) } ));

               

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(true);

            A.CallTo(() => _tournamentRepository.GetTournament(fakeTournamentId))
                .Returns(fakeTournamentToDelete);

            A.CallTo(() => _tournamentRepository.DeleteTournament(fakeTournamentToDelete))
                .Returns(true);

            A.CallTo(() => _userManager.GetUserId(A<ClaimsPrincipal>.Ignored))
                .Returns(curremtUserGUID);

            A.CallTo(() => fakeCurrentUser.IsInRole(UserRoles.Admin))
                .Returns(isAdmin);

            

            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = fakeCurrentUser
                }
            };

            // Act
            var result = controller.DeleteTournament(fakeTournamentId);


            // Assert
            if (isOk)
            {
                result.Should().NotBeNull().And.BeOfType<NoContentResult>();

                var okResult = result as NoContentResult;
                okResult.Should().NotBeNull();
            }
            else
            {
                result.Should().NotBeNull().And.BeOfType<ForbidResult>();

                var okResult = result as ForbidResult;
                okResult.Should().NotBeNull();
            }
            
        }

        [Fact]
        public void TournamentController_DeleteTournament_ReturnsBadRequest()
        {
            //Arange
            var fakeTournamentId = 1;

            var fakeTournamentToDelete = A.Fake<Tournament>();
            fakeTournamentToDelete.Id = fakeTournamentId;

            //var currentUserId = "fake GUID";
            fakeTournamentToDelete.Organizer = A.Fake<Organizer>();
            fakeTournamentToDelete.Organizer.UserId = "GUID";
            var isAdmin = false;

            var curremtUserGUID = "GUID";

            var fakeCurrentUser = A.Fake<ClaimsPrincipal>();
            fakeCurrentUser.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, curremtUserGUID) }));



            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(true);

            A.CallTo(() => _tournamentRepository.GetTournament(fakeTournamentId))
                .Returns(fakeTournamentToDelete);

            A.CallTo(() => _tournamentRepository.DeleteTournament(fakeTournamentToDelete))
                .Returns(false);

            A.CallTo(() => _userManager.GetUserId(A<ClaimsPrincipal>.Ignored))
                .Returns(curremtUserGUID);

            A.CallTo(() => fakeCurrentUser.IsInRole(UserRoles.Admin))
                .Returns(isAdmin);



            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = fakeCurrentUser
                }
            };

            // Act
            var result = controller.DeleteTournament(fakeTournamentId);


            // Assert
            
            
            result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

            var okResult = result as BadRequestObjectResult;
            okResult.Should().NotBeNull();
            
            

        }

        [Fact]
        public void TournamentController_DeleteTournament_ReturnsNotFound()
        {
            //Arange
            var fakeTournamentId = 1;

            A.CallTo(() => _tournamentRepository.TournamentExists(fakeTournamentId))
                .Returns(false);

            var controller = new TournamentController(_mapper, _tournamentRepository,
                _teamRepository, _organizerRepository,
                _gameRepository, _userManager, _swissRepository);


            // Act
            var result = controller.DeleteTournament(fakeTournamentId);


            // Assert
            result.Should().NotBeNull().And.BeOfType<NotFoundResult>();

            var okResult = result as NotFoundResult;
            okResult.Should().NotBeNull();



        }


        [Fact]
        public void TournamentController_CreateTournamenet_ReturnsOk()
        {
            
        }
    }
}
