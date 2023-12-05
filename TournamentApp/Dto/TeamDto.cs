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
        public List<GameNoDetailsDto> Team1Games { get; set; } = new List<GameNoDetailsDto>();
        public List<GameNoDetailsDto> Team2Games { get; set; } = new List<GameNoDetailsDto>();

        public string UserId { get; set; }
    }


    public class TeamNoDetailsDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string ShortTeamName { get; set; }
        public string City { get; set; }
        public string CoachFullName { get; set; }
        //public int UserId { get; set; }
    }
}
