using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Enregistrement du service de mise en cache mémoire
builder.Services.AddMemoryCache(options =>
{
	options.SizeLimit = 5000000;
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<ICacheTools, CacheTools>();

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
