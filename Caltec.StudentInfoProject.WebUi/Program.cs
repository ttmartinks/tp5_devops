using Caltec.StudentInfoProject.Business;
using Caltec.StudentInfoProject.Persistence;
using Caltec.StudentInfoProject.Persistence.Initializer;
using Caltec.Dependency;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lis la variable d'environnement explicitement
string connectionString = "Server=db,1433;Database=AppCustomerDiiageDbe;User=sa;Password=Admin1234!;TrustServerCertificate=True;";

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<StudentService>();
builder.Services.AddTransient<StudentClassService>();
builder.Services.AddTransient<SchoolFeesService>();
builder.Services.AddTransient<DegreeService>();
builder.Services.AddDbContext<StudentInfoDbContext>(
            options =>
            {
                options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());

            }, ServiceLifetime.Transient);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCalTechDependency(connectionString);

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

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

CreateDbIfNotExists(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCaltechDependency();
app.Run();


static void CreateDbIfNotExists(WebApplication? host)
{
    using (var scope = host?.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<StudentInfoDbContext>();
            DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB.");
        }
    }
}