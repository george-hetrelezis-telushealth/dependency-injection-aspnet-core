// https://app.pluralsight.com/code-labs/lessons/298a30d2-553f-491c-b769-d4ced0c2665f?access_token=61112F8294F313380448041EBCC61E7D&step_id=ed7c818d-126b-4c3b-8a5f-92fc9b051b44

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

services.TryAddSingleton<ICurrentPromotion, CurrentPromotion>();


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

//Add Get Route Here
app.MapGet("/promotions/{eventname}", async (string input, ICurrentPromotion promotions) =>
  {
    var promotion = await promotions.GetCurrentPromotionAsync(input);
    return promotion.WebApp.Services;
  });
//

app.Run();

public partial class Program { }
