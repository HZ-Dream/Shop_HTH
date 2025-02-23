using Microsoft.EntityFrameworkCore;
using Shop_HTH.Repository;

var builder = WebApplication.CreateBuilder(args);

//Connected sql
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

//seeding data
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);

app.Run();
