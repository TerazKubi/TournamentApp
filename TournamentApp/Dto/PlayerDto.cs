using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class PlayerDto
    {
        public int Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }

        //=========================================================================
        
        public int TeamId { get; set; }
        public UserDto User { get; set; }
        public int UserId { get; set; }

    }
}
