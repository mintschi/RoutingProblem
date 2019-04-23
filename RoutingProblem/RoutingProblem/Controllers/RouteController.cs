using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoutingProblem.Models;
using RoutingProblem.Services;
using RoutingProblem.Services.Data;

namespace RoutingProblem.Controllers
{
    [Produces("application/json")]
    [Route("api/Route")]
    public class RouteController : Controller
    {
        Zakladny zakladny = new Zakladny();
        Dijkster dijkster = new Dijkster();
        AStar astar = new AStar();
        LabelCorrect labelCorrect = new LabelCorrect();
        LabelSet labelSet = new LabelSet();
        DuplexDijkster duplexDijkster = new DuplexDijkster();
        MultiLabel multiLabel = new MultiLabel();
        TimeSpan time;

        [HttpGet]
        [Route("zakladny/{startLatLon}/{endLatLon}")]
        public RoutesDTO Zakladny(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = zakladny.CalculateShortestPath(nodes.Key, nodes.Value, PrepareData.NodesGraph);
            watch.Stop();
            time = watch.Elapsed;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("dijkster/{startLatLon}/{endLatLon}")]
        public RoutesDTO Dijkster(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = dijkster.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("astar/{startLatLon}/{endLatLon}")]
        public RoutesDTO AStar(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = astar.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("labelcorrect/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelCorrect(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = labelCorrect.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("labelset/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelSet(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = labelSet.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("duplexdijkster/{startLatLon}/{endLatLon}")]
        public RoutesDTO DuplexDijkster(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = duplexDijkster.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            return AfterCalculateShortestPathDuplex(node);
        }

        [HttpGet]
        [Route("multilabel/{startLatLon}/{endLatLon}/{k}")]
        public RoutesDTO MultiLabel(string startLatLon, string endLatLon, string k)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            multiLabel.K = Int32.Parse(k);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = multiLabel.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            return AfterCalculateShortestPathMultiLabel(node);
        }

        [HttpGet]
        [Route("zakladny/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO ZakladnyDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key, nodes.Value);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = zakladny.CalculateShortestPath(nodes.Key, nodes.Value, PrepareData.DisabledMovementGraph);
            watch.Stop();
            time = watch.Elapsed;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key, nodes.Value);
            return routes;
        }

        [HttpGet]
        [Route("dijkster/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO DijksterDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key, nodes.Value);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = dijkster.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key, nodes.Value);
            return routes;
        }

        [HttpGet]
        [Route("astar/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO AStarDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key, nodes.Value);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = astar.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key, nodes.Value);
            return routes;
        }

        [HttpGet]
        [Route("labelcorrect/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelCorrectDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key, nodes.Value);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = labelCorrect.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key, nodes.Value);
            return routes;
        }

        [HttpGet]
        [Route("labelset/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelSetDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key, nodes.Value);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = labelSet.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key, nodes.Value);
            return routes;
        }

        [HttpGet]
        [Route("duplexdijkster/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO DuplexDijksterDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key, nodes.Value);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = duplexDijkster.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            RoutesDTO routes = AfterCalculateShortestPathDuplex(node);
            PrepareData.RemoveStartEnd(nodes.Key, nodes.Value);
            return routes;
        }

        [HttpGet]
        [Route("multilabel/disabled/{startLatLon}/{endLatLon}/{k}")]
        public RoutesDTO MultiLabelDIsabled(string startLatLon, string endLatLon, string k)
        {
            KeyValuePair<NodeGraphDTO, NodeGraphDTO> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key, nodes.Value);
            multiLabel.K = Int32.Parse(k);
            var watch = Stopwatch.StartNew();
            NodeGraphDTO node = multiLabel.CalculateShortestPath(nodes.Key, nodes.Value);
            watch.Stop();
            time = watch.Elapsed;
            RoutesDTO routes = AfterCalculateShortestPathMultiLabel(node);
            PrepareData.RemoveStartEnd(nodes.Key, nodes.Value);
            return routes;
        }

        [HttpGet]
        [Route("data/{title}/{minLat}/{minLon}/{maxLat}/{maxLon}")]
        public LinkedList<KeyValuePair<decimal, string[]>> DownloadData(string title, float minLat, float minLon, float maxLat, float maxLon)
        {
            Models.Data data;
            using (GraphContext dopravnaSietContext = new GraphContext())
            {
                decimal id = dopravnaSietContext.Data.Max(d => d.IdData);
                dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault().Active = false;
                dopravnaSietContext.SaveChanges();
                SpracovanieOSMDat spracovanieOSMDat = new SpracovanieOSMDat();
                spracovanieOSMDat.SpracovanieXMLDat(dopravnaSietContext, new Models.Data()
                {
                    IdData = ++id,
                    Title = title,
                    MinLat = minLat,
                    MinLon = minLon,
                    MaxLat = maxLat,
                    MaxLon = maxLon,
                    Active = true
                });
                data = dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault();
            }

            PrepareData.PrepareNodesGraph(data);
            PrepareData.PrepareDisabledMovementGraph();

            LinkedList<KeyValuePair<decimal, string[]>> array = new LinkedList<KeyValuePair<decimal, string[]>>();
            using (GraphContext dopravnaSietContext = new GraphContext())
            {
                foreach (Models.Data d in dopravnaSietContext.Data)
                {
                    string[] field = new string[5] {
                            d.Title,
                            d.MinLat.ToString(CultureInfo.InvariantCulture),
                            d.MinLon.ToString(CultureInfo.InvariantCulture),
                            d.MaxLat.ToString(CultureInfo.InvariantCulture),
                            d.MaxLon.ToString(CultureInfo.InvariantCulture) };
                    if (d.Active)
                    {
                        array.AddFirst(new KeyValuePair<decimal, string[]>(d.IdData, field));
                    }
                    else
                    {
                        array.AddLast(new KeyValuePair<decimal, string[]>(d.IdData, field));
                    }

                }
            }
            
            return array;
        }

        [HttpPost]
        [Route("data/{id}")]
        public bool SetData(string id)
        {
            Models.Data data;
            using (GraphContext dopravnaSietContext = new GraphContext())
            {
                dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault().Active = false;
                dopravnaSietContext.Data.Where(d => d.IdData == Convert.ToDecimal(id)).FirstOrDefault().Active = true;
                dopravnaSietContext.SaveChanges();
                data = dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault();
            }
            if (data != null)
            {
                PrepareData.PrepareNodesGraph(data);
                PrepareData.PrepareDisabledMovementGraph();
            }
            return true;
        }

        [HttpGet]
        [Route("fields")]
        public LinkedList<KeyValuePair<decimal, string[]>> LoadFields()
        {
            LinkedList<KeyValuePair<decimal, string[]>> array = new LinkedList<KeyValuePair<decimal, string[]>>();
            using (GraphContext dopravnaSietContext = new GraphContext())
            {
                foreach (Models.Data data in dopravnaSietContext.Data)
                {
                    string[] field = new string[5] {
                            data.Title,
                            data.MinLat.ToString(CultureInfo.InvariantCulture),
                            data.MinLon.ToString(CultureInfo.InvariantCulture),
                            data.MaxLat.ToString(CultureInfo.InvariantCulture),
                            data.MaxLon.ToString(CultureInfo.InvariantCulture) };
                    if (data.Active)
                    {
                        array.AddFirst(new KeyValuePair<decimal, string[]>(data.IdData, field));
                    }
                    else
                    {
                        array.AddLast(new KeyValuePair<decimal, string[]>(data.IdData, field));
                    }
                }
            }
            return array;
        }

        [HttpGet]
        [Route("statistics")]
        public ActionResult Statistics()
        {
            var statistic = new
            {
                GraphMemory = Utils.GraphMemory,
                DisabledGraphMemory = Utils.DisabledGraphMemory,
                GraphTime = Utils.GraphTime,
                DisabledGraphTime = Utils.DisabledGraphTime,
                PocetVrcholov = Utils.PocetVrcholov
            };
            int opakovani = 10;
            int algoritmus = 0;
            TimeSpan time;
            Zakladny zakladny = new Zakladny();
            Dijkster dijkster = new Dijkster();
            AStar astar = new AStar();
            LabelCorrect labelCorrect = new LabelCorrect();
            LabelSet labelSet = new LabelSet();
            DuplexDijkster duplexDijkster = new DuplexDijkster();
            var watch = Stopwatch.StartNew();
            Random random = new Random();
            NodeGraphDTO[] startNodes = new NodeGraphDTO[opakovani];
            NodeGraphDTO[] endNodes = new NodeGraphDTO[opakovani];
            List<RoutesDTO> routesAll = new List<RoutesDTO>(6);

            for (int i = 0; i < opakovani; i++)
            {
                startNodes[i] = PrepareData.NodesGraph.Values.ElementAt(random.Next(PrepareData.NodesGraph.Count));
                endNodes[i] = PrepareData.NodesGraph.Values.ElementAt(random.Next(PrepareData.NodesGraph.Count));
            }

            for (int j = 0; j < 6; j++)
            {
                RoutesDTO routes = new RoutesDTO();
                for (int i = 0; i < opakovani; i++)
                {
                    PrepareData.PrepareNodesGraph();
                    switch (j)
                    {
                        case 0:
                            PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                            watch.Restart();
                            var n = zakladny.CalculateShortestPath(startNodes[i], endNodes[i], PrepareData.DisabledMovementGraph);
                            watch.Stop();
                            time = watch.Elapsed;
                            AfterCalculateShortestPath(n, time, routes);
                            PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                            break;
                        case 1:
                            PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                            watch.Restart();
                            n = dijkster.CalculateShortestPath(startNodes[i], endNodes[i]);
                            watch.Stop();
                            time = watch.Elapsed;
                            AfterCalculateShortestPath(n, time, routes);
                            PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                            break;
                        case 2:
                            PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                            watch.Restart();
                            n = astar.CalculateShortestPath(startNodes[i], endNodes[i]);
                            watch.Stop();
                            time = watch.Elapsed;
                            var a = routes.Route.Last;
                            AfterCalculateShortestPath(n, time, routes);
                            PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                            break;
                        case 3:
                            PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                            watch.Restart();
                            n = labelSet.CalculateShortestPath(startNodes[i], endNodes[i]);
                            watch.Stop();
                            time = watch.Elapsed;
                            AfterCalculateShortestPath(n, time, routes);
                            PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                            break;
                        case 4:
                            PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                            watch.Restart();
                            n = labelCorrect.CalculateShortestPath(startNodes[i], endNodes[i]);
                            watch.Stop();
                            time = watch.Elapsed;
                            AfterCalculateShortestPath(n, time, routes);
                            PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                            break;
                        case 5:
                            PrepareData.PutStartEnd(startNodes[i], endNodes[i]);
                            watch.Restart();
                            n = duplexDijkster.CalculateShortestPath(startNodes[i], endNodes[i]);
                            watch.Stop();
                            time = watch.Elapsed;
                            AfterCalculateShortestPathDuplex(n, time, routes);
                            PrepareData.RemoveStartEnd(startNodes[i], endNodes[i]);
                            break;
                    }
                }
                routesAll.Add(routes);
            }

            double[] PocetHranCesty = new double[6];
            double[] PocetSpracovanychVrcholov = new double[6];
            double[] DlzkaCesty = new double[6];
            double[] CasVypoctu = new double[6];
            algoritmus = 0;
            foreach (RoutesDTO r in routesAll)
            {
                opakovani = r.Route.Count == 0 ? opakovani : r.Route.Count;
                long PocetHranCestyA = 0;
                long PocetSpracovanychVrcholovA = 0;
                double DlzkaCestyA = 0;
                double CasVypoctuA = 0;
                foreach (RouteDTO route in r.Route)
                {
                    PocetHranCestyA += route.PocetHranCesty;
                    PocetSpracovanychVrcholovA += route.PocetSpracovanychVrcholov;
                    DlzkaCestyA += route.DlzkaCesty;
                    CasVypoctuA += route.CasVypoctu;
                }
                PocetHranCesty[algoritmus] = (double)PocetHranCestyA / opakovani;
                PocetSpracovanychVrcholov[algoritmus] = (double)PocetSpracovanychVrcholovA / opakovani;
                DlzkaCesty[algoritmus] = DlzkaCestyA / opakovani;
                CasVypoctu[algoritmus] = CasVypoctuA / opakovani;
                algoritmus++;
            }
            return Json(statistic);
        }

        private void AfterCalculateShortestPath(NodeGraphDTO nodeCon, TimeSpan time, RoutesDTO routes)
        {
            if (nodeCon == null)
                return;

            NodeGraphDTO node = nodeCon;
            LinkedList<NodeLocationDTO> nodeRoute = new LinkedList<NodeLocationDTO>();
            while (node != null)
            {
                nodeRoute.AddLast(new NodeLocationDTO()
                {
                    Lat = node.Node.Lat,
                    Lon = node.Node.Lon
                });
                node = node.PreviousNode;
            }

            RouteDTO route = new RouteDTO()
            {
                PocetHranCesty = nodeRoute.Count - 1,
                PocetSpracovanychVrcholov = Utils.PocetSpracovanychVrcholov,
                DlzkaCesty = nodeCon.CurrentDistance / 1000,
                CasVypoctu = time.TotalMilliseconds
            };

            routes.Route.AddLast(route);
        }

        private void AfterCalculateShortestPathDuplex(NodeGraphDTO nodeCon, TimeSpan time, RoutesDTO routes)
        {
            if (nodeCon == null)
                return;

            NodeGraphDTO node = nodeCon;
            LinkedList<NodeLocationDTO> nodeRoute = new LinkedList<NodeLocationDTO>();
            while (node != null)
            {
                nodeRoute.AddLast(new NodeLocationDTO()
                {
                    Lat = node.Node.Lat,
                    Lon = node.Node.Lon
                });
                node = node.PreviousNode;
            }

            node = nodeCon != null ? nodeCon.PreviousNodeR : null;
            while (node != null)
            {
                nodeRoute.AddFirst(new NodeLocationDTO()
                {
                    Lat = node.Node.Lat,
                    Lon = node.Node.Lon
                });
                node = node.PreviousNodeR;
            }

            RouteDTO route = new RouteDTO()
            {
                PocetHranCesty = nodeRoute.Count - 1,
                PocetSpracovanychVrcholov = Utils.PocetSpracovanychVrcholov,
                DlzkaCesty = nodeCon.CurrentDistance / 1000,
                CasVypoctu = time.TotalMilliseconds
            };

            routes.Route.AddLast(route);
        }

        private KeyValuePair<NodeGraphDTO, NodeGraphDTO> BeforeCalculateShortestPath(string startLatLon, string endLatLon)
        {
            PrepareData.PrepareNodesGraph();

            var startLat = double.Parse(startLatLon.Split(',')[0], CultureInfo.InvariantCulture);
            var startLon = double.Parse(startLatLon.Split(',')[1], CultureInfo.InvariantCulture);
            var startNode = PrepareData.NodesGraph.OrderBy(n1 => (Utils.Distance(n1.Key.Lat, n1.Key.Lon, startLat, startLon))).First();

            var endLat = double.Parse(endLatLon.Split(',')[0], CultureInfo.InvariantCulture);
            var endLon = double.Parse(endLatLon.Split(',')[1], CultureInfo.InvariantCulture);
            var endNode = PrepareData.NodesGraph.OrderBy(n1 => (Utils.Distance(n1.Key.Lat, n1.Key.Lon, endLat, endLon))).First();

            return new KeyValuePair<NodeGraphDTO, NodeGraphDTO>(startNode.Value, endNode.Value);
        }

        private KeyValuePair<NodeGraphDTO, NodeGraphDTO> BeforeCalculateDisabledShortestPath(string startLatLon, string endLatLon)
        {
            PrepareData.PrepareNodesGraph();

            var startLat = double.Parse(startLatLon.Split(',')[0], CultureInfo.InvariantCulture);
            var startLon = double.Parse(startLatLon.Split(',')[1], CultureInfo.InvariantCulture);
            var startNode = PrepareData.NodesGraph.OrderBy(n1 => (Utils.Distance(n1.Key.Lat, n1.Key.Lon, startLat, startLon))).First();

            var endLat = double.Parse(endLatLon.Split(',')[0], CultureInfo.InvariantCulture);
            var endLon = double.Parse(endLatLon.Split(',')[1], CultureInfo.InvariantCulture);
            var endNode = PrepareData.NodesGraph.OrderBy(n1 => (Utils.Distance(n1.Key.Lat, n1.Key.Lon, endLat, endLon))).First();

            return new KeyValuePair<NodeGraphDTO, NodeGraphDTO>(startNode.Value, endNode.Value);
        }

        private RoutesDTO AfterCalculateShortestPath(NodeGraphDTO nodeCon)
        {
            if (nodeCon == null)
                return null;

            NodeGraphDTO node = nodeCon;
            LinkedList<NodeLocationDTO> nodeRoute = new LinkedList<NodeLocationDTO>();
            while (node != null)
            {
                nodeRoute.AddLast(new NodeLocationDTO()
                {
                    Lat = node.Node.Lat,
                    Lon = node.Node.Lon
                });
                node = node.PreviousNode;
            }

            RouteDTO route = new RouteDTO()
            {
                PocetHranCesty = nodeRoute.Count - 1,
                PocetSpracovanychVrcholov = Utils.PocetSpracovanychVrcholov,
                DlzkaCesty = Math.Round(nodeCon.CurrentDistance, 5),
                CasVypoctu = Math.Round(time.TotalMilliseconds, 5),
                Nodes = nodeRoute
            };

            RoutesDTO routes = new RoutesDTO();
            routes.Route.AddLast(route);
            return routes;
        }

        private RoutesDTO AfterCalculateShortestPathDuplex(NodeGraphDTO nodeCon)
        {
            if (nodeCon == null)
                return null;

            NodeGraphDTO node = nodeCon;
            LinkedList<NodeLocationDTO> nodeRoute = new LinkedList<NodeLocationDTO>();
            while (node != null)
            {
                nodeRoute.AddLast(new NodeLocationDTO()
                {
                    Lat = node.Node.Lat,
                    Lon = node.Node.Lon
                });
                node = node.PreviousNode;
            }

            node = nodeCon != null ? nodeCon.PreviousNodeR : null;
            while (node != null)
            {
                nodeRoute.AddFirst(new NodeLocationDTO()
                {
                    Lat = node.Node.Lat,
                    Lon = node.Node.Lon
                });
                node = node.PreviousNodeR;
            }

            RouteDTO route = new RouteDTO()
            {
                PocetHranCesty = nodeRoute.Count - 1,
                PocetSpracovanychVrcholov = Utils.PocetSpracovanychVrcholov,
                DlzkaCesty = Math.Round(nodeCon.CurrentDistance, 5),
                CasVypoctu = Math.Round(time.TotalMilliseconds, 5),
                Nodes = nodeRoute
            };

            RoutesDTO routes = new RoutesDTO();
            routes.Route.AddLast(route);
            return routes;
        }

        private RoutesDTO AfterCalculateShortestPathMultiLabel(NodeGraphDTO nodeCon)
        {
            if (nodeCon == null)
                return null;

            RoutesDTO routes = new RoutesDTO();

            foreach (MultiLabelMark n in nodeCon.MultiLabelMark)
            {
                int k = n.K;
                int xk = k;
                NodeGraphDTO node = nodeCon;
                RouteDTO route = null;
                LinkedList<NodeLocationDTO> nodeRoute = new LinkedList<NodeLocationDTO>();
                while (xk != 0)
                {
                    nodeRoute.AddLast(new NodeLocationDTO()
                    {
                        Lat = node.Node.Lat,
                        Lon = node.Node.Lon
                    });
                    xk = node.MultiLabelMark[k - 1].Xk;
                    node = node.MultiLabelMark[k - 1].X;
                    k = xk;
                }

                route = new RouteDTO()
                {
                    PocetHranCesty = nodeRoute.Count - 1,
                    PocetSpracovanychVrcholov = Utils.PocetSpracovanychVrcholov,
                    DlzkaCesty = Math.Round(nodeCon.MultiLabelMark[n.K - 1].T, 5),
                    CasVypoctu = Math.Round(time.TotalMilliseconds, 5),
                    Nodes = nodeRoute
                };
                routes.Route.AddLast(route);
            }
            return routes;
        }
    }
}