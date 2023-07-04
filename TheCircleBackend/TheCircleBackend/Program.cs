using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TheCircleBackend.DBInfra;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Hubs;
using TheCircleBackend.DomainServices.IHelpers;
using TheCircleBackend.Helper;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<DomainContext>(options =>
{

    options.UseSqlServer(@"Data Source=.;Initial Catalog=TheCircleDomainDB;Integrated Security=True; TrustServerCertificate=True");
});

builder.Services.AddDbContext<IdentityDBContext>(options =>
{
    options.UseSqlServer(
        @"Data Source=.;Initial Catalog=TheCircleIdentityDB;Integrated Security=True; TrustServerCertificate=True");
});


builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<IdentityDBContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
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





// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IWebsiteUserRepo, EFWebsiteUserRepo>();
builder.Services.AddScoped<IChatMessageRepository, EFChatMessageRepo>();
builder.Services.AddScoped<ILogItemRepo, EFLogItemRepo>();
builder.Services.AddScoped<IViewerRepository, EFViewerRepo>();
builder.Services.AddScoped<ISecurityHelper, SecurityHelper>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IKeyRepo, EFKeyRepo>();
builder.Services.AddScoped<IVidStreamRepo, EFVidStream>();
builder.Services.AddScoped<IStreamChunkRepo, EFStreamChunkRepo>();
builder.Services.AddScoped<IVodRepo, EFVodRepo>();
builder.Services.AddScoped<IEntityCheckerService, EntityCheckersService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();


builder.Services.AddLogging(loggingBuilder => {
    loggingBuilder.AddFile("thecircle.log", append: true);
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



//app.UseHttpsRedirection();

app.UseCors(builder =>
{
    //Localhost
    builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowCredentials()
        .Build();


});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<LivestreamHub>("/hubs/Livestream");
app.MapHub<ChatHub>("/hubs/ChatHub");
app.MapHub<ViewerHub>("/hubs/ViewHub");

app.Run();
