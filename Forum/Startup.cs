#define SET_TESTABLE_USER_AS_ADMIN // for testing

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Forum.Services;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Forum
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<UserSettings>(Configuration.GetSection("UserSettings"));

            services.AddSingleton<IEmailSender, MailKitEmailSender>();
            services.AddSingleton<AccessibilityChecker>();
            services.AddSingleton<ImagesManager>();

            services.AddMvc().AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IConfiguration config)
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
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            CreateRoles(serviceProvider);
            CreateSections(config, serviceProvider);
            CreateDefaultImages(serviceProvider);

#if SET_TESTABLE_USER_AS_ADMIN && DEBUG
            AddUserToAdmins(serviceProvider, "5ec74e8b-7f31-4b3b-891a-a7092305fc8f");
#endif
        }

        private void AddUserToAdmins(IServiceProvider serviceProvider, string userId)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var user = userManager.FindByIdAsync(userId).Result;

            if (user != null)
            {
                var result = userManager.AddToRoleAsync(user, "Admin").Result;
                Debug.Print(result.Succeeded ? "Succeeded" : "Not Succeeded");
            }
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Moderator", "User", "Guest" };

            foreach (var roleName in roleNames)
            {
                var roleExistsTask = roleManager.RoleExistsAsync(roleName);
                roleExistsTask.Wait();

                if (!roleExistsTask.Result)
                {
                    var createRoleTask = roleManager.CreateAsync(new IdentityRole(roleName));
                    createRoleTask.Wait();
                }
            }
        }

        private void CreateSections(IConfiguration config, IServiceProvider serviceProvider)
        {
            IConfigurationSection configurationSection = config.GetSection("Sections");
            var sections = configurationSection.AsEnumerable();

            if(sections.Count() == 0)
            {
                return;
            }

            sections = sections.Reverse();

            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            foreach (var section in sections)
            {
                if(section.Value == null)
                {
                    continue;
                }

                var searchingSection = dbContext.Sections.FirstOrDefault(s => s.Name.Equals(section.Value));

                if (searchingSection == null)
                {
                    var newSection = new Section
                    {
                        Name = section.Value,
                        Topics = new List<Topic>(),
                        Accessibility = Accessibility.FullAccess
                    };
                    dbContext.Sections.Add(newSection);
                    dbContext.SaveChanges();
                }
            }
        }

        private void CreateDefaultImages(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userSettings = serviceProvider.GetRequiredService<IOptions<UserSettings>>();
            var defaultProfilePicture = userSettings.Value.DefaultProfilePicture;

            var img = dbContext.Images.Where(i => i.Filename == defaultProfilePicture).FirstOrDefault();

            if(img == null)
            {
                var defaultImage = new Image { Filename = defaultProfilePicture };
                dbContext.Images.Add(defaultImage);
                dbContext.SaveChanges();
            }
        }
    }
}
