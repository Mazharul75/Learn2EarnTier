using BLL.Services;
using DAL.EF;
using DAL.Repos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === Session storage requirements ===
builder.Services.AddDistributedMemoryCache(); // in-memory store for session data
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // expire after 30 min idle
    options.Cookie.HttpOnly = true;                  // JS can't read the session cookie
    options.Cookie.IsEssential = true;               // required even if user opts out of cookies (GDPR)
});

// === MVC controllers + views ===
builder.Services.AddControllersWithViews();

// === DAL repos (Scoped — one per HTTP request) ===
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<UserTypeRepo>();
builder.Services.AddScoped<CourseRepo>();
builder.Services.AddScoped<EnrollmentRepo>();
builder.Services.AddScoped<QuizRepo>();
builder.Services.AddScoped<QuestionRepo>();
builder.Services.AddScoped<QuizAttemptRepo>();
builder.Services.AddScoped<NotificationRepo>();

// === BLL services (Scoped) ===
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();
// More services added here in later phases:
// builder.Services.AddScoped<CourseService>();      (Phase D)
// builder.Services.AddScoped<EnrollmentService>();  (Phase F)
// etc.

// === EF Core DbContext (Scoped by default) ===
builder.Services.AddDbContext<Learn2EarnDBContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"));
});

var app = builder.Build();

// === HTTP pipeline (middleware order matters!) ===
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();          // serve files from wwwroot
app.UseRouting();
app.UseSession();              // ⬅️ session middleware (MUST be before Authorization & MapControllerRoute)
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();