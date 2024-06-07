using GymTraining.Models;
using Microsoft.AspNetCore.Identity;
using GymTraining.Services;
using Microsoft.EntityFrameworkCore;

namespace GymTraining
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Рядок підключення для Core Identity з файла конфігурації.
            string? identityConnection = builder.Configuration.GetConnectionString("IdentityConnection");

            // Додаю контекст ApplicationDbContext в якості сервіса.
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(identityConnection));
            // Додаю з IdentitySample.Mvc (файл Startup.cs).
            // https://github.com/dotnet/aspnetcore/tree/main/src/Identity/samples/IdentitySample.Mvc
            builder.Services.AddMvc();
            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                        .AddRoles<IdentityRole>()
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddSignInManager()
                        .AddDefaultTokenProviders();
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies(o => { });
            builder.Services.AddDistributedMemoryCache(); // Додаю використання IDistributedCache.
            builder.Services.AddSession(); // Додаю сервіси сесії.
                                           // Add application services.
            builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
            builder.Services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    ApplicationDbContext applicationIdentityDbContext = services.GetRequiredService<ApplicationDbContext>();
                    applicationIdentityDbContext.Database.Migrate();

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    string userRoleTrainer = "Trainer";
                    string userRoleSportsman = "Sportsman";
                    if (await roleManager.FindByNameAsync(userRoleTrainer) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(userRoleTrainer));
                    }
                    if (await roleManager.FindByNameAsync(userRoleSportsman) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(userRoleSportsman));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    //logger.LogError(ex, "An error occurred while seeding the database.");
                    logger.LogError(ex, ex.Message);
                }
            }
            app.UseSession(); // Додаю middleware для роботи з сесіями.

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

            // Обробка StatusCodes.
            app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");

            // Підключення аутентифікації. 
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}