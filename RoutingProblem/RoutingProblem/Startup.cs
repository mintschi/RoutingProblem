using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoutingProblem.Models;
using RoutingProblem.Services;
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

            Models.Data data;
            using (GraphContext dopravnaSietContext = new GraphContext())
            {
                data = dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault();

                if (data == null && dopravnaSietContext.Data.Count() > 0)
                {
                    data = dopravnaSietContext.Data.First();
                }
            }

            if (data != null)
            {
                long memBefore = GC.GetTotalMemory(true);
                var watch = Stopwatch.StartNew();
                PrepareData.PrepareNodesGraph(data);
                watch.Stop();
                long memAfter = GC.GetTotalMemory(true);
                Utils.GraphTime = watch.Elapsed;
                Utils.GraphMemory = memAfter - memBefore;

                memBefore = GC.GetTotalMemory(true);
                watch = Stopwatch.StartNew();
                PrepareData.PrepareDisabledMovementGraph();
                watch.Stop();
                memAfter = GC.GetTotalMemory(true);
                Utils.DisabledGraphTime = Utils.GraphTime + watch.Elapsed;
                Utils.DisabledGraphMemory = Utils.GraphMemory + memAfter - memBefore;

                //TimeSpan time;
                //double lat, lon;
                //Random random = new Random();
                //double maximumLat = data.MaxLat, minimumLat = data.MinLat;
                //double maximumLon = data.MaxLon, minimumLon = data.MinLon;
                //for (int i = 0; i < 1000; i++)
                //{
                //    lat = random.NextDouble() * (maximumLat - minimumLat) + minimumLat;
                //    lon = random.NextDouble() * (maximumLon - minimumLon) + minimumLon;
                //    watch = Stopwatch.StartNew();
                //    var startNode = PrepareData.NodesGraph.OrderBy(n1 => (Utils.Distance(n1.Key.Lat,
                //                                                            n1.Key.Lon,
                //                                                            lat,
                //                                                            lon)
                //                                                            )).First();
                //    watch.Stop();
                //    time += watch.Elapsed;
                //}
                //var timeAverage = time / 1000;

                //System.Device.Location.GeoCoordinate coord;
                //for (int i = 0; i < 1000; i++)
                //{
                //    lat = random.NextDouble() * (maximumLat - minimumLat) + minimumLat;
                //    lon = random.NextDouble() * (maximumLon - minimumLon) + minimumLon;
                //    coord = new System.Device.Location.GeoCoordinate(lat, lon);
                //    watch = Stopwatch.StartNew();
                //    var startNode = PrepareData.NodesGraph.Select(x => new System.Device.Location.GeoCoordinate(x.Key.Lat, x.Key.Lon))
                //                               .OrderBy(x => x.GetDistanceTo(coord))
                //                               .First();
                //    watch.Stop();
                //    time += watch.Elapsed;
                //}
                //var timeAverageOrdered = time / 1000;

                //        int opakovani = 10;
                //        int algoritmus = 0;
                //        TimeSpan time;
                //        Zakladny zakladny = new Zakladny();
                //        Dijkster dijkster = new Dijkster();
                //        AStar astar = new AStar();
                //        LabelCorrect labelCorrect = new LabelCorrect();
                //        LabelSet labelSet = new LabelSet();
                //        DuplexDijkster duplexDijkster = new DuplexDijkster();
                //        watch = Stopwatch.StartNew();
                //        Random random = new Random();
                //        NodeGraphDTO[] startNodes = new NodeGraphDTO[opakovani];
                //        NodeGraphDTO[] endNodes = new NodeGraphDTO[opakovani];
                //        List<RoutesDTO> routesAll = new List<RoutesDTO>(6);

                //        for (int i = 0; i < opakovani; i++)
                //        {
                //            startNodes[i] = PrepareData.NodesGraph.Values.ElementAt(random.Next(PrepareData.NodesGraph.Count));
                //            endNodes[i] = PrepareData.NodesGraph.Values.ElementAt(random.Next(PrepareData.NodesGraph.Count));
                //        }

                //        for (int j = 0; j < 6; j++)
                //        {
                //            RoutesDTO routes = new RoutesDTO();
                //            for (int i = 0; i < opakovani; i++)
                //            {
                //                PrepareData.PrepareNodesGraph();
                //                switch (j)
                //                {
                //                    case 0:
                //                        PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                //                        watch.Restart();
                //                        var n = zakladny.CalculateShortestPath(startNodes[i], endNodes[i], PrepareData.NodesGraph);
                //                        watch.Stop();
                //                        time = watch.Elapsed;
                //                        AfterCalculateShortestPath(n, time, routes);
                //                        PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                //                        break;
                //                    case 1:
                //                        PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                //                        watch.Restart();
                //                        n = dijkster.CalculateShortestPath(startNodes[i], endNodes[i]);
                //                        watch.Stop();
                //                        time = watch.Elapsed;
                //                        AfterCalculateShortestPath(n, time, routes);
                //                        PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                //                        break;
                //                    case 2:
                //                        PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                //                        watch.Restart();
                //                        n = astar.CalculateShortestPath(startNodes[i], endNodes[i]);
                //                        watch.Stop();
                //                        time = watch.Elapsed;
                //                        var a = routes.Route.Last;
                //                        AfterCalculateShortestPath(n, time, routes);
                //                        PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                //                        break;
                //                    case 3:
                //                        PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                //                        watch.Restart();
                //                        n = labelSet.CalculateShortestPath(startNodes[i], endNodes[i]);
                //                        watch.Stop();
                //                        time = watch.Elapsed;
                //                        AfterCalculateShortestPath(n, time, routes);
                //                        PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                //                        break;
                //                    case 4:
                //                        PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                //                        watch.Restart();
                //                        n = labelCorrect.CalculateShortestPath(startNodes[i], endNodes[i]);
                //                        watch.Stop();
                //                        time = watch.Elapsed;
                //                        AfterCalculateShortestPath(n, time, routes);
                //                        PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                //                        break;
                //                    case 5:
                //                        PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                //                        watch.Restart();
                //                        n = duplexDijkster.CalculateShortestPath(startNodes[i], endNodes[i]);
                //                        watch.Stop();
                //                        time = watch.Elapsed;
                //                        AfterCalculateShortestPathDuplex(n, time, routes);
                //                        PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                //                        break;
                //                }
                //            }
                //            routesAll.Add(routes);
                //        }

                //        double[] PocetHranCesty = new double[6];
                //        double[] PocetSpracovanychVrcholov = new double[6];
                //        double[] DlzkaCesty = new double[6];
                //        TimeSpan[] CasVypoctu = new TimeSpan[6];
                //        algoritmus = 0;
                //        foreach (RoutesDTO r in routesAll)
                //        {
                //            opakovani = r.Route.Count == 0 ? opakovani : r.Route.Count;
                //            long PocetHranCestyA = 0;
                //            long PocetSpracovanychVrcholovA = 0;
                //            double DlzkaCestyA = 0;
                //            TimeSpan CasVypoctuA = new TimeSpan();
                //            foreach (RouteDTO route in r.Route)
                //            {
                //                PocetHranCestyA += route.PocetHranCesty;
                //                PocetSpracovanychVrcholovA += route.PocetSpracovanychVrcholov;
                //                DlzkaCestyA += route.DlzkaCesty;
                //                CasVypoctuA += route.CasVypoctu;
                //            }
                //            PocetHranCesty[algoritmus] = (double) PocetHranCestyA / opakovani;
                //            PocetSpracovanychVrcholov[algoritmus] = (double) PocetSpracovanychVrcholovA / opakovani;
                //            DlzkaCesty[algoritmus] = DlzkaCestyA / opakovani;
                //            CasVypoctu[algoritmus] = CasVypoctuA / opakovani;
                //            algoritmus++;
                //        }
                //    }
                //}

                //private void AfterCalculateShortestPath(NodeGraphDTO nodeCon, TimeSpan time, RoutesDTO routes)
                //{
                //    if (nodeCon == null)
                //        return;

                //    NodeGraphDTO node = nodeCon;
                //    LinkedList<NodeLocationDTO> nodeRoute = new LinkedList<NodeLocationDTO>();
                //    while (node != null)
                //    {
                //        nodeRoute.AddLast(new NodeLocationDTO()
                //        {
                //            Lat = node.Node.Lat,
                //            Lon = node.Node.Lon
                //        });
                //        node = node.PreviousNode;
                //    }

                //    RouteDTO route = new RouteDTO()
                //    {
                //        PocetHranCesty = nodeRoute.Count,
                //        PocetSpracovanychVrcholov = Utils.PocetSpracovanychVrcholov,
                //        DlzkaCesty = nodeCon.CurrentDistance / 1000,
                //        CasVypoctu = time
                //    };

                //    routes.Route.AddLast(route);
                //}

                //private void AfterCalculateShortestPathDuplex(NodeGraphDTO nodeCon, TimeSpan time, RoutesDTO routes)
                //{
                //    if (nodeCon == null)
                //        return;

                //    NodeGraphDTO node = nodeCon;
                //    LinkedList<NodeLocationDTO> nodeRoute = new LinkedList<NodeLocationDTO>();
                //    while (node != null)
                //    {
                //        nodeRoute.AddLast(new NodeLocationDTO()
                //        {
                //            Lat = node.Node.Lat,
                //            Lon = node.Node.Lon
                //        });
                //        node = node.PreviousNode;
                //    }

                //    node = nodeCon != null ? nodeCon.PreviousNodeR : null;
                //    while (node != null)
                //    {
                //        nodeRoute.AddFirst(new NodeLocationDTO()
                //        {
                //            Lat = node.Node.Lat,
                //            Lon = node.Node.Lon
                //        });
                //        node = node.PreviousNodeR;
                //    }

                //    RouteDTO route = new RouteDTO()
                //    {
                //        PocetHranCesty = nodeRoute.Count,
                //        PocetSpracovanychVrcholov = Utils.PocetSpracovanychVrcholov,
                //        DlzkaCesty = nodeCon.CurrentDistance / 1000,
                //        CasVypoctu = time
                //    };

                //    routes.Route.AddLast(route);
            }
        }
    }
}
