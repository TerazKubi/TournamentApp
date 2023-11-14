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
        
        public List<Player> Players { get; set; } = new List<Player>();
        //public List<Score > Scores { get; set; } = new List<Score>();
        
        public int UserId { get; set; }
    }
}
