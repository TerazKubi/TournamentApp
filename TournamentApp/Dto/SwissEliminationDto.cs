using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class SwissEliminationDto
    {
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
        public TeamNoDetailsDto Team { get; set; }
        public int Points { get; set; }
        public bool HasPause { get; set; }
    }
}
