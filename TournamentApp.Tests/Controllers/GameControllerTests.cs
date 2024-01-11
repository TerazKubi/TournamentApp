using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Tests.Controllers
{
    internal class GameControllerTests
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
    }
}
