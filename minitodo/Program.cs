using Microsoft.EntityFrameworkCore;
using minitodo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

string connectionString = builder.Configuration.GetConnectionString("DefaultConntection")!;
builder.Services.AddDbContext<AppDbContext>(
    options=>options.UseSqlite(connectionString));
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
