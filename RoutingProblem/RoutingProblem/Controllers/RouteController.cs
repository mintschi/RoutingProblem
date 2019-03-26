using System;
using System.Collections.Generic;
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
        GraphContext dopravnaSietContext = new GraphContext();
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
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = zakladny.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value, PrepareData.NodesGraph);
            time = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("dijkster/{startLatLon}/{endLatLon}")]
        public RoutesDTO Dijkster(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = dijkster.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("astar/{startLatLon}/{endLatLon}")]
        public RoutesDTO AStar(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = astar.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("labelcorrect/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelCorrect(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = labelCorrect.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("labelset/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelSet(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = labelSet.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("duplexdijkster/{startLatLon}/{endLatLon}")]
        public RoutesDTO DuplexDijkster(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = duplexDijkster.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            return AfterCalculateShortestPathDuplex(node);
        }

        [HttpGet]
        [Route("multilabel/{startLatLon}/{endLatLon}/{k}")]
        public RoutesDTO MultiLabel(string startLatLon, string endLatLon, string k)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            multiLabel.K = Int32.Parse(k);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = multiLabel.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            return AfterCalculateShortestPathMultiLabel(node);
        }

        [HttpGet]
        [Route("zakladny/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO ZakladnyDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key.Key, nodes.Value.Key);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = zakladny.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value, PrepareData.NodesDisabledAll);
            time = DateTime.Now - start;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key.Key, nodes.Value.Key);
            return routes;
        }

        [HttpGet]
        [Route("dijkster/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO DijksterDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key.Key, nodes.Value.Key);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = dijkster.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key.Key, nodes.Value.Key);
            return routes;
        }

        [HttpGet]
        [Route("astar/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO AStarDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key.Key, nodes.Value.Key);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = astar.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key.Key, nodes.Value.Key);
            return routes;
        }

        [HttpGet]
        [Route("labelcorrect/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelCorrectDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key.Key, nodes.Value.Key);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = labelCorrect.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key.Key, nodes.Value.Key);
            return routes;
        }

        [HttpGet]
        [Route("labelset/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO LabelSetDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key.Key, nodes.Value.Key);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = labelSet.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            RoutesDTO routes = AfterCalculateShortestPath(node);
            PrepareData.RemoveStartEnd(nodes.Key.Key, nodes.Value.Key);
            return routes;
        }

        [HttpGet]
        [Route("duplexdijkster/disabled/{startLatLon}/{endLatLon}")]
        public RoutesDTO DuplexDijksterDisabled(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key.Key, nodes.Value.Key);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = duplexDijkster.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            RoutesDTO routes = AfterCalculateShortestPathDuplex(node);
            PrepareData.RemoveStartEnd(nodes.Key.Key, nodes.Value.Key);
            return routes;
        }

        [HttpGet]
        [Route("multilabel/disabled/{startLatLon}/{endLatLon}/{k}")]
        public RoutesDTO MultiLabelDIsabled(string startLatLon, string endLatLon, string k)
        {
            KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> nodes = BeforeCalculateDisabledShortestPath(startLatLon, endLatLon);
            PrepareData.PutStartEnd(nodes.Key.Key, nodes.Value.Key);
            multiLabel.K = Int32.Parse(k);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = multiLabel.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            time = DateTime.Now - start;
            RoutesDTO routes = AfterCalculateShortestPathMultiLabel(node);
            PrepareData.RemoveStartEnd(nodes.Key.Key, nodes.Value.Key);
            return routes;
        }

        [HttpGet]
        [Route("data/{title}/{minLat}/{minLon}/{maxLat}/{maxLon}")]
        public bool DownloadData(string title, float minLat, float minLon, float maxLat, float maxLon)
        {
            decimal id = dopravnaSietContext.Data.Max(d => d.IdData);
            dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault().Active = false;
            dopravnaSietContext.SaveChanges();
            SpracovanieOSMDat spracovanieOSMDat = new SpracovanieOSMDat();
            spracovanieOSMDat.SpracovanieXMLDat(dopravnaSietContext, new Models.Data() {
                IdData = ++id,
                Title = title,
                MinLat = minLat,
                MinLon = minLon,
                MaxLat = maxLat,
                MaxLon = maxLon,
                Active = true
            });
            Models.Data data = dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault();
            PrepareData.PrepareNodesGraph(dopravnaSietContext.Node.Where(d => d.IdData == data.IdData), dopravnaSietContext.DisabledMovement.Where(d => d.IdData == data.IdData));
            return true;
        }

        [HttpPost]
        [Route("data/{id}")]
        public bool SetData(string id)
        {
            dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault().Active = false;
            dopravnaSietContext.Data.Where(d => d.IdData == Convert.ToDecimal(id)).FirstOrDefault().Active = true;
            dopravnaSietContext.SaveChanges();
            Models.Data data = dopravnaSietContext.Data.Where(d => d.Active == true).FirstOrDefault();
            PrepareData.PrepareNodesGraph(dopravnaSietContext.Node.Where(d => d.IdData == data.IdData), dopravnaSietContext.DisabledMovement.Where(d => d.IdData == data.IdData));
            return true;
        }

        [HttpGet]
        [Route("fields")]
        public LinkedList<KeyValuePair<decimal, string>> LoadFields()
        {
            LinkedList<KeyValuePair<decimal, string>> array = new LinkedList<KeyValuePair<decimal, string>>();
            foreach (Models.Data data in dopravnaSietContext.Data)
            {
                if (data.Active)
                {
                    array.AddFirst(new KeyValuePair<decimal, string> (data.IdData, data.Title));
                } else
                {
                    array.AddLast(new KeyValuePair<decimal, string>(data.IdData, data.Title));
                }
                
            }
            return array;
        }

        private KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> BeforeCalculateShortestPath(string startLatLon, string endLatLon)
        {
            PrepareData.PrepareNodesGraph();

            var startLL = startLatLon.Split(',');
            var startNode = PrepareData.NodesGraph.Aggregate((n1, n2) => Utils.Distance(n1.Key.Lat, 
                                                                                           n1.Key.Lon, 
                                                                                           double.Parse(startLL[0], CultureInfo.InvariantCulture),
                                                                                           double.Parse(startLL[1], CultureInfo.InvariantCulture)) <
                                                                         Utils.Distance(n2.Key.Lat, 
                                                                                           n2.Key.Lon,
                                                                                           double.Parse(startLL[0], CultureInfo.InvariantCulture),
                                                                                           double.Parse(startLL[1], CultureInfo.InvariantCulture))
                                                             ? n1 : n2);

            var endLL = endLatLon.Split(',');
            var endNode = PrepareData.NodesGraph.Aggregate((n1, n2) => Utils.Distance(n1.Key.Lat,
                                                                                         n1.Key.Lon,
                                                                                         double.Parse(endLL[0], CultureInfo.InvariantCulture),
                                                                                         double.Parse(endLL[1], CultureInfo.InvariantCulture)) <
                                                                       Utils.Distance(n2.Key.Lat,
                                                                                         n2.Key.Lon,
                                                                                         double.Parse(endLL[0], CultureInfo.InvariantCulture),
                                                                                         double.Parse(endLL[1], CultureInfo.InvariantCulture))
                                                           ? n1 : n2);

            return new KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>>(startNode, endNode);
        }

        private KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>> BeforeCalculateDisabledShortestPath(string startLatLon, string endLatLon)
        {
            PrepareData.PrepareNodesGraph();

            var startLL = startLatLon.Split(',');
            var startNode = PrepareData.DisabledMovementGraph.Aggregate((n1, n2) => Utils.Distance(n1.Key.Lat,
                                                                                           n1.Key.Lon,
                                                                                           double.Parse(startLL[0], CultureInfo.InvariantCulture),
                                                                                           double.Parse(startLL[1], CultureInfo.InvariantCulture)) <
                                                                         Utils.Distance(n2.Key.Lat,
                                                                                           n2.Key.Lon,
                                                                                           double.Parse(startLL[0], CultureInfo.InvariantCulture),
                                                                                           double.Parse(startLL[1], CultureInfo.InvariantCulture))
                                                             ? n1 : n2);

            var endLL = endLatLon.Split(',');
            var endNode = PrepareData.DisabledMovementGraph.Aggregate((n1, n2) => Utils.Distance(n1.Key.Lat,
                                                                                         n1.Key.Lon,
                                                                                         double.Parse(endLL[0], CultureInfo.InvariantCulture),
                                                                                         double.Parse(endLL[1], CultureInfo.InvariantCulture)) <
                                                                       Utils.Distance(n2.Key.Lat,
                                                                                         n2.Key.Lon,
                                                                                         double.Parse(endLL[0], CultureInfo.InvariantCulture),
                                                                                         double.Parse(endLL[1], CultureInfo.InvariantCulture))
                                                           ? n1 : n2);

            return new KeyValuePair<KeyValuePair<Node, NodeDisabledMovementDTO>, KeyValuePair<Node, NodeDisabledMovementDTO>>(startNode, endNode);
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
                PocetHranCesty = nodeRoute.Count,
                PocetNavstivenychHran = Utils.PocetNavstivenychHran,
                DlzkaCesty = Math.Round(nodeCon.CurrentDistance, 5),
                CasVypoctu = time.Milliseconds,
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
                PocetHranCesty = nodeRoute.Count,
                PocetNavstivenychHran = Utils.PocetNavstivenychHran,
                DlzkaCesty = Math.Round(nodeCon.CurrentDistance, 5),
                CasVypoctu = time.Milliseconds,
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
                    PocetHranCesty = nodeRoute.Count,
                    PocetNavstivenychHran = Utils.PocetNavstivenychHran,
                    DlzkaCesty = Math.Round(nodeCon.MultiLabelMark[n.K - 1].T, 5),
                    CasVypoctu = time.Milliseconds,
                    Nodes = nodeRoute
                };
                routes.Route.AddLast(route);
            }
            return routes;
        }
    }
}