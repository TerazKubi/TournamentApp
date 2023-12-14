namespace TournamentApp.Models
{
    public enum EliminationTypes
    {
        SingleElimination,
        DoubleElimination,
        SwissElimination
    }
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string City { get; set; }
        public int TeamCount { get; set; }
        public EliminationTypes EliminationAlgorithm { get; set; }
        public string State { get; set; }

        //=========================================================================
        public Organizer Organizer { get; set; }
        public int OrganizerId { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Game> Games { get; set; } = new List<Game>();

        public List<SwissElimination> SwissElimination { get; set; } = new List<SwissElimination>();

    }
}
