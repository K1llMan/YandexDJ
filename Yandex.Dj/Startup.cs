using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Yandex.Dj.Bot;
using Yandex.Dj.CommonServices.WebSocket;
using Yandex.Dj.Services;
using Yandex.Dj.Services.Rocksmith;

namespace Yandex.Dj
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
            services.AddMvc(opts => {
                    opts.EnableEndpointRouting = false;
            })
            .AddJsonOptions(opts => {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddSingleton(typeof(RocksmithService));
            services.AddSingleton(typeof(Broadcast));
            services.AddSingleton(typeof(BotService));
            services.AddSingleton(typeof(StreamingService));

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
#if DEBUG
                configuration.RootPath = "ClientApp/build";
#else
                configuration.RootPath = "web";
#endif

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // Использование сокетов
            app.UseWebSockets();
            app.UseMiddleware<WebSocketMiddleware>();
            
            app.UseMvc();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                /*
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
                */
            });
        }
    }
}
