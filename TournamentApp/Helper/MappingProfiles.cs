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
            CreateMap<UserDto, User>();

            CreateMap<Post, PostDto>();
            CreateMap<PostDto, Post>();

            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();

            CreateMap<Game, GameDto>();
            CreateMap<GameDto, Game>();

            CreateMap<Tournament, TournamentDto>();
            CreateMap<TournamentDto, Tournament>();

            CreateMap<Player, PlayerDto>();
            CreateMap<PlayerDto, Player>();

            CreateMap<Team, TeamDto>();
            CreateMap<TeamDto, Team>();

            CreateMap<Organizer, OrganizerDto>();
            CreateMap<OrganizerDto, Organizer>();

            
            
            
        }
    }
}
