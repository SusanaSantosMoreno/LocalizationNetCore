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
using XLocalizer.Routing;
using XLocalizer.Xml;
using XLocalizer;
using XLocalizer.Translate;
using XLocalizer.Translate.MyMemoryTranslate;

namespace ProyectoPostNetCore {
    public class Startup {
        public IConfiguration Configuration { get; }
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices (IServiceCollection services) {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
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
                options.RequestCultureProviders.Insert(0,
                    new RouteSegmentRequestCultureProvider(suportedCultures));
            });
            services.AddSingleton<IXResourceProvider, XmlResourceProvider>();

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

            //LOCALIZATION
            var supportedCultures = new[] { "en-US", "es-ES", "es" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            app.UseAuthorization();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
