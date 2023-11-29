using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class GameDto
    {
        public int Id { get; set; }
        public string KeyCode { get; set; }
        public string State { get; set; }
        public DateTime StartDate { get; set; }

        public int Round { get; set; }
        public int Set1Team1Points { get; set; }
        public int Set1Team2Points { get; set; }
        public int Set2Team1Points { get; set; }
        public int Set2Team2Points { get; set; }
        public int Set3Team1Points { get; set; }
        public int Set3Team2Points { get; set; }
        public int Set4Team1Points { get; set; }
        public int Set4Team2Points { get; set; }
        public int Set5Team1Points { get; set; }
        public int Set5Team2Points { get; set; }
        public int Team1Sets { get; set; }
        public int Team2Sets { get; set; }

        public int CurrentSet { get; set; }

        //=========================================================================
        //public List<Team> Teams { get; set; } = new List<Team>();
        public List<GameCommentDto> GameComments { get; set; } = new List<GameCommentDto>();

        public int TournamentId { get; set; }

        
        public int? Team1Id { get; set; }
        
        public int? Team2Id { get; set; }


        public int? WinnerId { get; set; }

        public int? ParentId { get; set; }
        public List<GameDto> Children { get; set; } = new List<GameDto>();
    }
}
