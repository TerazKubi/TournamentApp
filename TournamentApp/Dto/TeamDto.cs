using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string ShortTeamName { get; set; }
        public string City { get; set; }
        public string CoachFullName { get; set; }

        //=========================================================================
        
        public List<PlayerDto> Players { get; set; } = new List<PlayerDto>();
        public List<Game> Team1Games { get; set; } = new List<Game>();
        public List<Game> Team2Games { get; set; } = new List<Game>();

        public int UserId { get; set; }
    }
}
