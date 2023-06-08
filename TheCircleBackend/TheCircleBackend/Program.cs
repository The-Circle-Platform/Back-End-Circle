using Microsoft.EntityFrameworkCore;
using TheCircleBackend.DBInfra;
using TheCircleBackend.DBInfra.Repo;
using TheCircleBackend.DomainServices.IRepo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DomainContext>(options =>
{
    options.UseSqlServer(@"Data Source=.;Initial Catalog=TheCircleDomainDB;Integrated Security=True; TrustServerCertificate=True");
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IWebsiteUserRepo, EFWebsiteUserRepo>();
builder.Services.AddScoped<ILogItemRepo, EFLogItemRepo>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
