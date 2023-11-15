using Microsoft.EntityFrameworkCore;
using TournamentApp.Models;

namespace TournamentApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        //public DbSet<Score> Scores { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySQL("server=localhost;database=tournamentapptest;user=root");
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");

                // user 1-1 team
                entity.HasOne(u => u.Team)
                    .WithOne(t => t.User)
                    .HasForeignKey<Team>(t => t.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                //user 1-1 player
                entity.HasOne(u => u.Player)
                    .WithOne(p => p.User)
                    .HasForeignKey<Player>(p => p.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                //user 1-* posts
                entity.HasMany(u => u.Posts)
                    .WithOne(p => p.Author)
                    .HasForeignKey(p => p.AuthorId);
                //user 1-* comments
                entity.HasMany(u => u.Comments)
                    .WithOne(c => c.Author)
                    .HasForeignKey(c => c.AuthorId);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Text).IsRequired();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

                //post 1-* comments
                entity.HasMany(p => p.Comments)
                    .WithOne(c => c.Post)
                    .HasForeignKey(c => c.PostId);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Text).IsRequired();
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Organizer>(entity => 
            {
                entity.HasKey(o => o.Id);

                //organizer 1-* tournaments
                entity.HasMany(o => o.Tournaments)
                    .WithOne(tr => tr.Organizer)
                    .HasForeignKey(tr => tr.OrganizerId);
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.TeamName).IsRequired();
                entity.Property(t => t.ShortTeamName).IsRequired();
                entity.Property(t => t.City).IsRequired();

                
            });

            modelBuilder.Entity<Player>(entity => 
            {
                entity.HasKey(pl => pl.Id);

            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(tr => tr.Id);
                entity.Property(tr => tr.StartDate).IsRequired();
                entity.Property(tr => tr.TeamCount).IsRequired();
                entity.Property(tr => tr.EliminationAlgorithm).IsRequired();
                entity.Property(tr => tr.State).HasDefaultValue("awaited");

                //tournament 1-* games
                entity.HasMany(tr => tr.Games)
                    .WithOne(g => g.Tournament)
                    .HasForeignKey(g => g.TournamentId);

                // tournaments *-* teams
                entity.HasMany(tr => tr.Teams)
                    .WithMany(t => t.Tournaments);
            });

            modelBuilder.Entity<Game>(entity => 
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.KeyCode).IsRequired();
                entity.Property(g => g.State).HasDefaultValue("awaited");

                //games *-1 team
                entity.HasOne(g => g.Team1)
                    .WithMany(t => t.Team1Games)
                    .HasForeignKey(g => g.Team1Id)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(g => g.Team2)
                    .WithMany(t => t.Team2Games)
                    .HasForeignKey(g => g.Team2Id)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                //game 1-*scores
                //entity.HasMany(g => g.Scores)
                //    .WithOne(s => s.Game)
                //    .HasForeignKey(s => s.GameId);

            });

            //modelBuilder.Entity<Score>(entity =>
            //{
            //    entity.HasKey(s => s.Id);
            //    entity.Property(s => s.SetPoints).HasDefaultValue(0);
            //    entity.Property(s => s.Points).HasDefaultValue(0);
            //});
        }
    }
}
