using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class TournamentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string City { get; set; }
        public int TeamCount { get; set; }
        public string EliminationAlgorithm { get; set; }
        public string State { get; set; }

        //=========================================================================
        public int OrganizerId { get; set; }
        public List<TeamNoDetailsDto> Teams { get; set; } = new List<TeamNoDetailsDto>();
        public List<GameNoDetailsDto> Games { get; set; } = new List<GameNoDetailsDto>();
    }


    public class TournamentNoDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string City { get; set; }
        public int TeamCount { get; set; }

        public int OrganizerId { get; set; }
        public OrganizerDto Organizer { get; set; }
    }
}
