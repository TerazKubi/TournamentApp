using AutoMapper;
using TournamentApp.Dto;
using TournamentApp.Input;
using TournamentApp.Models;

namespace TournamentApp.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, UserNoDetailsDto>();
            CreateMap<User, UserAsAuthorDto>();
            CreateMap<User, UserImageUrlDto>();
            //CreateMap<UserImageUrlDto, User>();
            CreateMap<UserDto, User>();
            CreateMap<UserCreate, User>();

            CreateMap<Post, PostDto>();
            CreateMap<PostDto, Post>();
            CreateMap<PostCreate, Post>();

            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
            CreateMap<CommentCreate, Comment>();

            CreateMap<Game, GameDto>();
            CreateMap<Game, GameNoDetailsDto>();
            CreateMap<Game, GameNode>();
            CreateMap<GameDto, Game>();
            CreateMap<GameCreate, Game>();

            CreateMap<Tournament, TournamentDto>();
            CreateMap<Tournament, TournamentNoDetailsDto>();
            CreateMap<TournamentDto, Tournament>();
            CreateMap<TournamentCreate, Tournament>();

            CreateMap<Player, PlayerDto>();
            CreateMap<Player, PlayerNoDetailsDto>();
            CreateMap<Player, PlayerAsAuthor>();
            CreateMap<PlayerDto, Player>();
            CreateMap<PlayerCreate, Player>();

            CreateMap<Team, TeamDto>();
            CreateMap<Team, TeamNoDetailsDto>();
            CreateMap<Team, TeamAsAuthorDto>();
            CreateMap<TeamDto, Team>();
            CreateMap<TeamCreate, Team>();

            CreateMap<Organizer, OrganizerDto>();
            CreateMap<Organizer, OrganizerAuthorDto>();
            CreateMap<OrganizerDto, Organizer>();
            CreateMap<OrganizerCreate, Organizer>();

            CreateMap<GameComment, GameCommentDto>();
            CreateMap<GameCommentDto, GameComment>();
            CreateMap<GameCommentCreate, GameComment>();
            
            
        }
    }
}
