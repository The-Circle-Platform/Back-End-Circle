using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TheCircleBackend.DBInfra;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.DomainServices.IRepo;
using TheCircleBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DomainContext>(options =>
{
    options.UseSqlServer(@"Data Source=.;Initial Catalog=TheCircleDomainDB;Integrated Security=True; TrustServerCertificate=True");
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IWebsiteUserRepo, EFWebsiteUserRepo>();
builder.Services.AddScoped<ILogItemRepo, EFLogItemRepo>();
builder.Services.AddScoped<IChatMessageRepository, EFChatMessageRepo>();

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



app.UseHttpsRedirection();

app.UseCors(builder =>
{
    //Localhost
    builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowCredentials()
        .Build();

    /*//Test server vervang http://localhost:4200 met test server of netlify website
    builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowCredentials()
        .Build();

    //Production server
    builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowCredentials()
        .Build();*/

});

//Misschien is een Proxy toepassen een goed idee om de gecommented 
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/ChatHub");
app.Run();
