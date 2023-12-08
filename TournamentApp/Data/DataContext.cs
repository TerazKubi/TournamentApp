using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TournamentApp.Models;

namespace TournamentApp.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions <DataContext> options) : base(options)
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
        public DbSet<GameComment> GameComments { get; set; }
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
                //entity.Property(u => u.PasswordHash).IsRequired();
                //entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.ImageURL).HasDefaultValue("Upload/UserImages/default.png");
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

                //user 1-1 organizer
                entity.HasOne(u => u.Organizer)
                .WithOne(o => o.User)
                .HasForeignKey<Organizer>(o => o.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                //user 1-* posts
                entity.HasMany(u => u.Posts)
                    .WithOne(p => p.Author)
                    .HasForeignKey(p => p.AuthorId);
                //user 1-* comments
                entity.HasMany(u => u.Comments)
                    .WithOne(c => c.Author)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                //user 1-* game comments
                entity.HasMany(u => u.GameComments)
                    .WithOne(gc => gc.Author)
                    .HasForeignKey(gc => gc.AuthorId);
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

            modelBuilder.Entity<GameComment>(entity => {
                entity.HasKey(gc => gc.Id);
                entity.Property(gc => gc.Text).IsRequired();
                entity.Property(gc => gc.CreatedAt).HasDefaultValueSql("GETDATE()");
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
                entity.Property(g => g.Set1Team1Points).HasDefaultValue(0);
                entity.Property(g => g.Set1Team2Points).HasDefaultValue(0);
                entity.Property(g => g.Set2Team1Points).HasDefaultValue(0);
                entity.Property(g => g.Set2Team2Points).HasDefaultValue(0);
                entity.Property(g => g.Set3Team1Points).HasDefaultValue(0);
                entity.Property(g => g.Set3Team2Points).HasDefaultValue(0);
                entity.Property(g => g.Set4Team1Points).HasDefaultValue(0);
                entity.Property(g => g.Set4Team2Points).HasDefaultValue(0);
                entity.Property(g => g.Set5Team1Points).HasDefaultValue(0);
                entity.Property(g => g.Set5Team2Points).HasDefaultValue(0);
                entity.Property(g => g.Team1Sets).HasDefaultValue(0);
                entity.Property(g => g.Team2Sets).HasDefaultValue(0);
                entity.Property(g => g.CurrentSet).HasDefaultValue(1);

                //games *-1 team
                entity.HasOne(g => g.Team1)
                    .WithMany(t => t.Team1Games)
                    .HasForeignKey(g => g.Team1Id)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(g => g.Team2)
                    .WithMany(t => t.Team2Games)
                    .HasForeignKey(g => g.Team2Id)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                //games 1-* gamecomment
                entity.HasMany(g => g.GameComments)
                    .WithOne(gc => gc.Game)
                    .HasForeignKey(gc => gc.GameId);


                // ======================================================
                entity.HasOne(g => g.Winner).WithMany().HasForeignKey(g => g.WinnerId);

                //entity.HasOne(g => g.Parent)
                //    .WithOne().HasForeignKey<Game>(g => g.ParentId);

                entity.HasMany(g => g.Children).WithOne(g => g.Parent).HasForeignKey(g => g.ParentId).OnDelete(DeleteBehavior.Restrict);


                //entity.HasOne(g => g.LeftChild).WithOne().HasForeignKey<Game>(g => g.LeftChildId);
                //entity.HasOne(g => g.RightChild).WithOne().HasForeignKey<Game>(g => g.RightChildId);


                //entity.HasOne(g => g.LeftChild).WithOne().HasForeignKey(g => g.Left)

            //    modelBuilder.Entity<BinaryTreeNode>()
            //.HasOne(node => node.Parent)
            //.WithMany()
            //.HasForeignKey(node => node.ParentId);

            //    modelBuilder.Entity<BinaryTreeNode>()
            //        .HasOne(node => node.LeftChild)
            //        .WithOne()
            //        .HasForeignKey<BinaryTreeNode>(node => node.LeftChildId);

            //    modelBuilder.Entity<BinaryTreeNode>()
            //        .HasOne(node => node.RightChild)
            //        .WithOne()
            //        .HasForeignKey<BinaryTreeNode>(node => node.RightChildId);
            });

            
        }
    }
}
