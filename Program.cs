using AttendanceTracker.AttendanceTrackerStateMachine;
using AttendanceTracker.DataAccess.Data;
using AttendanceTracker.DataAccess.Repository.IRepository;
using AttendanceTracker.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(
    options=> options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<AttendanceTrackerStateContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
});

// adding session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// adding redis
builder.Services.AddDistributedMemoryCache();

// Uncomment the following lines to use Redis cache in production
// builder.Services.AddStackExchangeRedisCache(options =>
//  {
//      options.Configuration = builder.Configuration.GetConnectionString("MyRedisConStr");
//      options.InstanceName = "SampleInstance";
//  });

// add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// register the session middleware (redis)
app.Lifetime.ApplicationStarted.Register(() =>
{
    var currentTimeUTC = DateTime.UtcNow.ToString();
    byte[] encodedCurrentTimeUTC = System.Text.Encoding.UTF8.GetBytes(currentTimeUTC);
    var options = new DistributedCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(60));
    app.Services.GetService<IDistributedCache>()
                              .Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
});

// Map SignalR RefreshHub to endpoint "/refreshHub"
app.MapHub<RefreshHub>("/refreshHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=QR}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
