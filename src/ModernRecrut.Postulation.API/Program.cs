using Microsoft.EntityFrameworkCore;
using ModernRecrut.Postulation.API.Data;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<PostulationsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));

builder.Services.AddScoped<IPostulationsService, PostulationsService>();
builder.Services.AddScoped<INotesService, NotesService>();
builder.Services.AddScoped<IGenererEvaluationService, GenererEvaluationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
