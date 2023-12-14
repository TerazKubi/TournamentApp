using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }

        //=========================================================================
        
        public TeamNoDetailsDto Team { get; set; }
        public int TeamId { get; set; }
        public string UserId { get; set; }
        public UserNoDetailsDto User { get; set; }

    }

    public class PlayerNoDetailsDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }
        public TeamNoDetailsDto Team { get; set; }
        public int TeamId { get; set; }
        public string UserId { get; set; }
        public UserNoDetailsDto User { get; set; }
    }
    public class PlayerAsAuthor
    {
        public int Id { get; set; }
    }
}
