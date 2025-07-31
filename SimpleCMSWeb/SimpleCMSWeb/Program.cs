using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SimpleCMSWeb.Data;
using SimpleCMSWeb.Models.EmailView;
using SimpleCMSWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Add multiple cookie authentication schemes
builder.Services.AddAuthentication()
    .AddCookie("PatientScheme", options =>
    {
        options.LoginPath = "/PatientAccount/Login";
        options.LogoutPath = "/PatientAccount/Logout";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ✅ Mark cookie as secure
    })
    .AddCookie("DoctorScheme", options =>
    {
        options.LoginPath = "/DoctorAccount/Login";
        options.LogoutPath = "/DoctorAccount/Logout";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddCookie("AdminScheme", options =>
    {
        options.LoginPath = "/AdminAccount/Login";
        options.LogoutPath = "/AdminAccount/Logout";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

builder.Services.AddSession(); // For sessions

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();           // After routing is okay
app.UseAuthentication();    // ✅ Must be before Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // ✅ Now correctly at the end