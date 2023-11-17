using Microsoft.EntityFrameworkCore;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;

namespace TournamentApp.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly DataContext _context;
        public TeamRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            return Save();
        }

        public bool DelteTeam(Team team)
        {
            _context.Remove(team);
            return Save();
        }

        public Team GetById(int id)
        {
            return _context.Teams.Where(t => t.Id == id).FirstOrDefault();
        }

        public List<Team> GetTeams()
        {
            return _context.Teams.Include(t => t.Players).ToList();
        }

        public bool TeamExists(int teamId)
        {
            return _context.Teams.Any(t => t.Id == teamId);
        }

        public bool UpdateTeam(Team team)
        {
            _context.Update(team);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool AllTeamsExists(List<int> teamIdList)
        {
            var existingTeamList = _context.Teams.Where(t => teamIdList.Contains(t.Id)).ToList();
            return teamIdList.Count == existingTeamList.Count;
        }

        public List<Team> GetTeamsFromList(List<int> teamIdList)
        {
            return _context.Teams.Where(t => teamIdList.Contains(t.Id)).ToList();
        }
    }
}
