using Core.Identity;
using DataLayer.Context;
using DataLayer.Entity.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Interfaces;
using Services.Services;

namespace Asp_Sample
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
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            #region Auth

            #region Liara Connection Configuration

            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            //builder.DataSource = "s11.liara.ir,30292";
            //builder.UserID = "sa";
            //builder.Password = "yYhnSKnhGkV8iFYPpgxtg3aE";
            //builder.InitialCatalog = "appDataBase";

            //services.AddDbContextPool<AppDbContext>(options =>
            //{
            //    options.UseSqlServer(new SqlConnection(builder.ConnectionString));
            //});

            #endregion

            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
            });

            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        options.ClientId = "597636127689-g50c5hi3q3h8h5dibukijkhv410gbnhp.apps.googleusercontent.com";
            //        options.ClientSecret = "W_xZMX7Mz1hwHcgL-J8eST7A";
            //    });

            services.AddAuthentication();

            services.AddIdentity<AppUser, IdentityRole>(options =>
                {
                    options.Password.RequiredUniqueChars = 0;
                    options.User.RequireUniqueEmail = true;
                    //options.User.AllowedUserNameCharacters =
                    //    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
                    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(4);
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<PersianIdentityErrorDescriber>();

            //services.AddScoped<IMessageSender, MessageSender>();

            #endregion

            services.AddTransient<IVoteService, VoteService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "Account",
                //    pattern:"{{area:exists}/{action = Index}/{id ?}}"
                //);
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
