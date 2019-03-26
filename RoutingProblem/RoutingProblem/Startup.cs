using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoutingProblem.Models;
using RoutingProblem.Services.Data;

namespace RoutingProblem
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
            services.AddMvc();
            services.AddDbContext<GraphContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DopravnaSiet")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            GraphContext dopravnaSietContext = new GraphContext();
            Models.Data data = dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault();
            if (data == null)
            {
                PrepareData.PrepareNodesGraph(dopravnaSietContext.Node, dopravnaSietContext.DisabledMovement);
            }
            else
            {
                PrepareData.PrepareNodesGraph(dopravnaSietContext.Node.Where(d => d.IdData == data.IdData), dopravnaSietContext.DisabledMovement.Where(d => d.IdData == data.IdData));
            }
        }
    }
}
