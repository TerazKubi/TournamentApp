﻿using TournamentApp.Models;

namespace TournamentApp.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public DateTime LastLogin { get; set; }

        public int TeamId { get; set; }
        public int PlayerId { get; set; }
    }
}
