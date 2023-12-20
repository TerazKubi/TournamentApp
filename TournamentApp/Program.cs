
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TournamentApp.Data;
using TournamentApp.Interfaces;
using TournamentApp.Models;
using TournamentApp.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TournamentApp;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

// Etity framework ===============================================================================================================================================================================
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


// Identity ========================================================================================================================================================================================
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


// Adding Authentication ===========================================================================================================================================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Jwt Bearer ========================================================================================================================================================================================
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});


builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
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
builder.Services.AddScoped<IGameCommentRepository, GameCommentRepository>();
builder.Services.AddScoped<ISwissEliminationRepository, SwissEliminationRepository>();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
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

    
}

static async Task SeedDataAsync(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using var scope = scopedFactory.CreateScope();
    var service = scope.ServiceProvider.GetService<Seed>();
    await service.SeedDataContextAsync();
}

await SeedDataAsync(app);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//STATIC IMAGE

app.UseStaticFiles();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
