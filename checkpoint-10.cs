//https://app.pluralsight.com/code-labs/lessons/c491cc0e-d5e2-469c-8201-6d91fb561db2?access_token=690A613EBA92FFF13DC22E0D163A279D&step_id=d916649a-52e7-4bb4-8fab-5097f090d0a4

global using Globoticket.WebApp;
global using Globoticket.WebApp.Services;

global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Data.Sqlite;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
	options.ValidateOnBuild = true;
	options.ValidateScopes = builder.Environment.IsDevelopment();
});

var services = builder.Services;

services.TryAddSingleton<IShuffleImages, RandomImages>();

services.Configure<RandomImageConfiguration>(builder.Configuration.GetSection("testing"));
services.Configure<BookingLengthConfiguration>(builder.Configuration.GetSection("EventBookings"));
services.Configure<EventSiteConfiguration>(builder.Configuration.GetSection("EventSettings"));

services.AddSetupRegistrations();

services.TryAddSingleton<IBookingLengthConfiguration>(sp =>
	sp.GetRequiredService<IOptions<BookingLengthConfiguration>>().Value);

services.AddBookingRules();

services.AddWelcome();

/*
// Add services to the container.
services.AddControllersWithViews();
services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/FindAvailableEvents");
    options.Conventions.AuthorizePage("/BookEvent");
    options.Conventions.AuthorizePage("/CustomerBookings");
});
services.AddHttpClient();


using var connection = new SqliteConnection("Filename=:memory:");
connection.Open();

//Add services to the container.

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connection));
services.AddDatabaseDeveloperPageExceptionFilter();

services.AddDefaultIdentity<GloboticketUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

services.AddHostedService<InitializeDbService>();
 
services.ConfigureApplicationCookie(options =>
{
	options.AccessDeniedPath = "/identity/account/access-denied";
});

*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program { }
