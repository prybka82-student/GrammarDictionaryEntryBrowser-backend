using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http.Routing;
using Dictionary.Core;
using Dictionary.Core.Services;
using Dictionary.Data;
using Dictionary.Service;
using Dictionary.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dictionary.Api
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
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .WithMethods(new[] { "GET" });
                });
            });

            //MongoDb
            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));

            //dane dostepowe do bazy danych
            services.AddSingleton<IDatabaseSettings>(dbSettings => dbSettings.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            //unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //core
            services.AddTransient<IDictionary, DictionaryService>();

            services
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Latest); //zapewnia kompatybilnosc ze wskazana wersja frameworka
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
                app.UseHsts(); //wymusza na przegladarce korzystanie wyłacznie z HTTPS
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
