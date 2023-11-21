
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IOrganizerRepository, OrganizerRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    var server = builder.Configuration["DatabaseServer"] ?? "localhost";
    var port = builder.Configuration["DatabasePort"] ?? "1433";
    var user = builder.Configuration["DatabaseUser"] ?? "SA";
    var password = builder.Configuration["DatabasePassword"] ?? "";
    var dbName = builder.Configuration["DatabaseName"] ?? "TournamentAppDB";
   


    var connectionString = $"Server={server},{port}; Initial Catalog={dbName}; User ID={user}; Password={password}; Encrypt=False;Trust Server Certificate=False;";
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Checking for database......");
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DataContext>();

    // Ensure the database is created
    Console.WriteLine("ENSURING DB CREATED......");
    dbContext.Database.EnsureCreated();

    // Apply pending migrations
    //Console.WriteLine("APPLYING MIGRATIONS...........");
    //dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
