using TournamentApp.Dto;

namespace TournamentApp.Input
{
    public class TournamentCreateWrapper
    {
        public TournamentCreate Tournament { get; set; }
        public List<int> teamsIdList { get; set; } = new List<int>();
    }
}
