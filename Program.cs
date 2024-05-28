using Examensarbete.Data;
using Examensarbete.Services;
using Examensarbete.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Examensarbete
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Konfigurera tjänster för sessioner.
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // 30min sessions
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Konfigurera tjänster för databasanslutning.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Registrera services
            builder.Services.AddScoped<IOrderDataService, OrderDataService>();
            builder.Services.AddScoped<IProductService, ProductService>();

            // Undvik cykliska referenser under serialisering
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.MaxDepth = 64;
            });


            // Konfigurera lokaliseringsalternativ för att stödja svenska kulturer.
            var supportedCultures = new[] { "sv-SE" }; // Lägg till flera om behov finns.
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "sv-SE", uiCulture: "sv-SE");
                options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            });

            // Konfigurera tjänster för autensiering och auktorisering.
            //builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            //   .AddNegotiate();

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.FallbackPolicy = options.DefaultPolicy;
            //});

            // Konfigurera Razor Pages.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Konfigurera HTTP-pipeline för produktionsmiljön.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Konfigurera HTTP-pipeline för allmänna scenarier.
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            //app.UseAuthentication();
            app.UseRequestLocalization(localizationOptions);
            app.UseAuthorization();
            app.MapRazorPages();

            app.Run(); // Starta webbapplikationen.
        }
    }
}