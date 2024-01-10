using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
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
            _swissRepository = A.Fake<ISwissEliminationRepository>(); ;
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
            

            var fakeTournamentId = 1; // Replace with an actual tournament ID for testing

            // Assume that _tournamentRepository.GetTournamentById returns a fake tournament
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

            // Add more specific assertions based on your actual implementation and requirements
            // For example, you might want to verify that the returned DTO matches the expected values.

            // Example assertion (adjust based on your actual implementation)
            tournamentDto.Id.Should().Be(fakeTournament.Id);
        }


        //[Fact]
        //public void TournamentController_GetTournamentById_ReturnNotFound()
        //{
        //    // Arrange
        //    var tournamentId = 1; // Provide a non-existing tournamentId for testing
        //    var tournamentRepository = A.Fake<ITournamentRepository>(); // Assuming you have an ITournamentRepository interface

        //    A.CallTo(() => tournamentRepository.TournamentExists(tournamentId))
        //        .Returns(false);

        //    var controller = new TournamentController(
        //        A.Fake<IMapper>(), tournamentRepository,
        //        A.Fake<ITeamRepository>(), A.Fake<IOrganizerRepository>(),
        //        A.Fake<IGameRepository>(), A.Fake<UserManager<ApplicationUser>>(),
        //        A.Fake<ISwissRepository>());

        //    // Act
        //    var result = controller.GetTournamnetById(tournamentId);

        //    // Assert
        //    result.Should().BeOfType<NotFoundResult>();
        //}
    }
}
