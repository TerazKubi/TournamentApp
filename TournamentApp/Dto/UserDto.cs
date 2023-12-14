using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        //public DateTime LastLogin { get; set; }
        public string ImageURL { get; set; }


        public int? TeamId { get; set; }
        public int? PlayerId { get; set; }
        public int? OrganizerId { get; set; }
    }

    public class UserNoDetailsDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string ImageURL { get; set; }

    }

    public class UserAsAuthorDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string ImageURL { get; set; }

        public OrganizerAuthorDto Organizer { get; set; }
        public TeamAsAuthorDto Team {  get; set; }
        public PlayerAsAuthor Player { get; set; }

    }

    public class UserImageUrlDto
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
    }
}
