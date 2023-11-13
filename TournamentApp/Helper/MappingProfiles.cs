using AutoMapper;
using TournamentApp.Dto;
using TournamentApp.Models;

namespace TournamentApp.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<Post, PostDto>();
            CreateMap<Comment, CommentDto>();
            CreateMap<Game, GameDto>();
            CreateMap<Tournament, TournamentDto>();
            CreateMap<Player, PlayerDto>();
            CreateMap<Team, TeamDto>();
            CreateMap<Organizer, OrganizerDto>();
        }
    }
}
