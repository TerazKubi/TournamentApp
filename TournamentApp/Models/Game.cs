namespace TournamentApp.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string KeyCode { get; set; }
        public string State { get; set; }
        public DateTime? StartDate { get; set; }

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

        public bool IsWinnerTree { get; set; }

        //=========================================================================
        //public List<Team> Teams { get; set; } = new List<Team>();
        //public List<Score> Scores { get; set; } = new List<Score>();
        public Tournament Tournament { get; set; }
        public int TournamentId { get; set; }

        public List<GameComment> GameComments { get; set; } = new List<GameComment>();

        public Team Team1 { get; set; }
        public int? Team1Id { get; set;}
        public Team Team2 { get; set; }
        public int? Team2Id { get; set;}

        public Team Winner { get; set; }
        public int? WinnerId { get; set; }

        public int? ParentId { get; set; }
        public Game Parent { get; set; }

        public List<Game> Children { get; set; } = new List<Game>();
        

    }

    //public class TreeNode
    //{
    //    public Game Game { get; set; }
    //    public TreeNode Left { get; set; }
    //    public TreeNode Right { get; set; }
    //    public TreeNode Parent { get; set; }
    //}
}
