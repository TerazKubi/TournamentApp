﻿using TournamentApp.Models;

namespace TournamentApp.Interfaces
{
    public interface IPlayerRepository
    {
        List<Player> GetPlayers();
        Player GetById(int id);
        bool CreatePlayer(Player player);
        bool DeletePlayer(Player player);
        bool UpdatePlayer(Player player);
        bool PlayerExists(int playerId);
    }
}
