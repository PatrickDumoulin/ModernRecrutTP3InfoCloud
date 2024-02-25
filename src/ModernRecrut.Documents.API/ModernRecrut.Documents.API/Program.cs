
using ModernRecrut.Documents.API.Services;
using ModernRecrut.Documents.API.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureClients
    (
        configure =>
        {
            configure.AddBlobServiceClient(builder.Configuration.GetConnectionString("StorageConnectionString"));
        }
    );

builder.Services.AddScoped<IDocumentsService, DocumentService>();

builder.Services.AddScoped<IStorageServiceHelper, StorageServiceHelper>();


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

app.UseStaticFiles();

app.Run();
