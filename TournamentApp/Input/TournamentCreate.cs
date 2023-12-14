using TournamentApp.Models;

namespace TournamentApp.Input
{
    public class TournamentCreate
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string City { get; set; }
        public int TeamCount { get; set; }
        public EliminationTypes EliminationAlgorithm { get; set; }
        public int OrganizerId { get; set; }
    }
}
