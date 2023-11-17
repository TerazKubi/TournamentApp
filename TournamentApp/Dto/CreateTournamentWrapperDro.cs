namespace TournamentApp.Dto
{
    public class CreateTournamentWrapperDro
    {
        public TournamentDto Tournament { get; set; }
        public List<int> teamsIdList { get; set; } = new List<int>();
    }
}
