using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entity;
using ApplicationCore.Interfaces;

using BankingSystem.ApplicationCore.Interfaces;
using BankingSystem.Infrastructure.Data;

using Infrastructure.Data;
using Infrastructure.Identity;

using MediatR;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Services;
using Web.Services.Background;
using Web.Services.HttpClient;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));

            services.AddDbContext<BankOperationsContext>(options =>
                options.UseSqlServer(
                    Configuration["Data:ConnectionStrings:DefaultConnection"]));

            //add identity
            services.AddDbContext<BankOperationsIdentityContext>(options =>
                options.UseSqlServer(
                    Configuration["Data:BankOperationsIdentity:ConnectionString"]));
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<BankOperationsIdentityContext>()
                    .AddDefaultTokenProviders();

            services.AddTransient(typeof(ICreditRepository), typeof(CreditRepository));
            services.AddTransient(typeof(IDepositRepository), typeof(DepositRepository));
            services.AddTransient(serviceType: typeof(IBankAccountRepository), implementationType: typeof(BankAccountEfRepository));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            
            services.AddScoped<ICurrencyViewModelService, CurrencyViewModelService>();

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddMvc();
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize);
            services.AddRazorPages();
            
            services.AddHttpClient<CurrencyExchangeService>();
            services.AddHttpClient<PrivatApiService>();
            services.AddHttpContextAccessor();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".BankingSystem.Session";
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
            });

            //services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            //IdentityDataSeed.RolesSeed(app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>());
        }
    }
}
