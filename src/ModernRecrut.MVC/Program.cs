using Microsoft.Extensions.Logging.Configuration;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModernRecrut.MVC.Data;
using ModernRecrut.MVC.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UtilisateursContextConnection") ?? throw new InvalidOperationException("Connection string 'UtilisateursContextConnection' not found.");

builder.Services.AddDbContext<UtilisateursContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<Utilisateur>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UtilisateursContext>();


// Journalisation
//builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddEventLog();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IOffreEmploisService, OffreEmploisServiceProxy>(client => 
	client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlOffreEmploiAPI")));

builder.Services.AddHttpClient<IFavorisService, FavorisServiceProxy>(client => 
	client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlFavoriAPI")));

builder.Services.AddHttpClient<IDocumentsService, DocumentsServiceProxy>(client =>
	client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlDocumentAPI")));

builder.Services.AddHttpClient<IPostulationsService, PostulationsServiceProxy>(client =>
	client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlPostulationAPI")));

builder.Services.AddHttpClient<INotesService, NotesServiceProxy>(client =>
	client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("UrlNoteAPI")));
//builder.Services.AddScoped<IDocumentsService, DocumentsServiceProxy>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Home/CodeStatus?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
