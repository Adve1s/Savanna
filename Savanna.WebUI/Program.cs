using Savanna.Logic;
using Savanna.WebUI.Hubs;
using Savanna.WebUI.Services;

namespace Savanna.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<WorldService>();
            builder.Services.AddSingleton<GameLoopService>();
            builder.Services.AddHostedService<GameLoopService>(provider => provider.GetRequiredService<GameLoopService>());

            var app = builder.Build();
            app.MapHub<GameHub>(GameHub.HUB_URL);

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

            app.Run();
        }
    }
}
