using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoPostNetCore {
    public class Startup {
        public IConfiguration Configuration { get; }
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices (IServiceCollection services) {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            services.AddMvc()
               .AddViewLocalization(
                   LanguageViewLocationExpanderFormat.Suffix,
                   opts => { opts.ResourcesPath = "Resources"; })
               .AddDataAnnotationsLocalization();
            services.Configure<RequestLocalizationOptions>(options => {
                var suportedCultures = new List<CultureInfo> {
                    new CultureInfo("en-US"),
                    new CultureInfo("es-ES"),
                    new CultureInfo("en"),
                    new CultureInfo("es")
                };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = suportedCultures;
                options.SupportedUICultures = suportedCultures;
            });
            services.AddControllersWithViews(options => options.EnableEndpointRouting = false).
                AddSessionStateTempDataProvider();
        }


        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseRequestLocalization(app.ApplicationServices.
                GetService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseAuthorization();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
