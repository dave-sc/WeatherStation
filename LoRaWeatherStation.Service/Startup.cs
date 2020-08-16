using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LoRaWeatherStation.Service
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
            if (Configuration.GetValue<bool>("LRRC_USE_IN_MEMORY_DB"))
            {
                services.AddDbContext<WeatherStationContext>(options => options.UseInMemoryDatabase("lrrc"));
            }
            else
            {
                var connectionString = Configuration.GetValue<string>("LRRC_CONNECTION_STRING");
                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException(
                        "Database connection string may not be empty. Please ensure that environment variable 'LRRC_CONNECTION_STRING' is set.");

                services.AddDbContext<WeatherStationContext>(options => options.UseNpgsql(connectionString));                
            }            
            
            services.AddControllers();
            services.AddSingleton<SensorRecorder>();
            services.AddHostedService(provider => provider.GetService<SensorRecorder>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
