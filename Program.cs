using QuizMaker.Controllers;
using QuizMaker.DB;

var config = new ConfigurationBuilder()
            .AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
            .Build();

var kernelSettings = config.GetSection(KernelSettings.SectionName).Get<KernelSettings>()
                             ?? throw new Exception("Kernel Settings not found in appsettings.json");
var dbSettings = config.GetSection(MongoDBSettings.SectionName).Get<MongoDBSettings>()
                             ?? throw new Exception("DB Settings not found in appsettings.json");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions<KernelSettings>().Bind(config.GetSection(KernelSettings.SectionName));
builder.Services.AddScoped<KernelOps>();
builder.Services.AddOptions<MongoDBSettings>().Bind(config.GetSection(MongoDBSettings.SectionName));
builder.Services.AddScoped<DBOps>();
builder.Services.AddControllersWithViews();
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
    pattern: "{controller=QuizGen}/{action=Index}/{id?}");

app.Run();
