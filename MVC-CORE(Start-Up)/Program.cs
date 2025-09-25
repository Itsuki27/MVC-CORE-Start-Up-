using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MVC_CORE_Start_Up_.Areas.Identity.Data;
using MVC_CORE_Start_Up_.BackendResources;
using MVC_CORE_Start_Up_.Data;
using MVC_CORE_Start_Up_.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StartUpDbContextConnection") ?? throw new InvalidOperationException("Connection string 'StartUpDbContextConnection' not found.");

builder.Services.AddDbContext<StartUpDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<StartUpDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

// For Razor Pages
builder.Services.AddRazorPages();

// Identity options configuration
builder.Services.Configure<IdentityOptions>(options => {     

    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

});

// Adding DBContext for Entity Framework
builder.Services.AddDbContext<MvcCoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StartUpDbContextConnection") ?? throw new InvalidOperationException("Connection string 'StartUpDbContextConnection' not found.")));

builder.Services.AddTransient<IEmailSender, EmailSender>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
