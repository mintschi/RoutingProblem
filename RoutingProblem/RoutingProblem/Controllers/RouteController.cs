using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoutingProblem.Data;
using RoutingProblem.Models;
using RoutingProblem.Services;

namespace RoutingProblem.Controllers
{
    [Produces("application/json")]
    [Route("api/Route")]
    public class RouteController : Controller
    {
        DopravnaSietContext dopravnaSietContext = new DopravnaSietContext();
        Dijkster dijkster = new Dijkster();
        AStar astar = new AStar();
        LabelCorrect labelCorrect = new LabelCorrect();
        LabelSet labelSet = new LabelSet();
        DuplexDijkster duplexDijkster = new DuplexDijkster();
        MultiLabel multiLabel = new MultiLabel();
        TimeSpan timeItTook;

    [HttpGet]
        [Route("dijkster/{startLatLon}/{endLatLon}")]
        public Routes Dijkster(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = dijkster.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            timeItTook = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("astar/{startLatLon}/{endLatLon}")]
        public Routes AStar(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = astar.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            timeItTook = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("labelcorrect/{startLatLon}/{endLatLon}")]
        public Routes LabelCorrect(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = labelCorrect.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            timeItTook = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("labelset/{startLatLon}/{endLatLon}")]
        public Routes LabelSet(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = labelSet.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            timeItTook = DateTime.Now - start;
            return AfterCalculateShortestPath(node);
        }

        [HttpGet]
        [Route("duplexdijkster/{startLatLon}/{endLatLon}")]
        public Routes DuplexDijkster(string startLatLon, string endLatLon)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = duplexDijkster.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            timeItTook = DateTime.Now - start;
            return AfterCalculateShortestPathDuplex(node);
        }

        [HttpGet]
        [Route("multilabel/{startLatLon}/{endLatLon}/{k}")]
        public Routes MultiLabel(string startLatLon, string endLatLon, string k)
        {
            KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> nodes = BeforeCalculateShortestPath(startLatLon, endLatLon);
            multiLabel.K = Int32.Parse(k);
            DateTime start = DateTime.Now;
            NodeGraphDTO node = multiLabel.CalculateShortestPath(nodes.Key.Value, nodes.Value.Value);
            timeItTook = DateTime.Now - start;
            return AfterCalculateShortestPathMultiLabel(node);
        }

        private KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>> BeforeCalculateShortestPath(string startLatLon, string endLatLon)
        {
            PrepareData.PrepareNodesGraph();

            //var startLL = startLatLon.Split(',');
            //var startNode = nodes.Aggregate((n1, n2) => Utils.Vzdialenost(n1.Value.Lat, double.Parse(startLL[0], CultureInfo.InvariantCulture),
            //                                                              n1.Value.Lon, double.Parse(startLL[1], CultureInfo.InvariantCulture)) <
            //                                            Utils.Vzdialenost(n2.Value.Lat, double.Parse(startLL[0], CultureInfo.InvariantCulture),
            //                                                              n2.Value.Lon, double.Parse(startLL[1], CultureInfo.InvariantCulture))
            //                                            ? n1 : n2);

            //var endLL = endLatLon.Split(',');
            //var endNode = nodes.Aggregate((n1, n2) => Utils.Vzdialenost(n1.Value.Lat, double.Parse(endLL[0], CultureInfo.InvariantCulture),
            //                                                            n1.Value.Lon, double.Parse(endLL[1], CultureInfo.InvariantCulture)) <
            //                                          Utils.Vzdialenost(n2.Value.Lat, double.Parse(endLL[0], CultureInfo.InvariantCulture),
            //                                                            n2.Value.Lon, double.Parse(endLL[1], CultureInfo.InvariantCulture))
            //                                          ? n1 : n2);
            var startLL = startLatLon.Split(',');
            var startNode = PrepareData.NodesGraph.Aggregate((n1, n2) => Math.Abs(n1.Key.Lat - double.Parse(startLL[0], CultureInfo.InvariantCulture)) +
                                                                Math.Abs(n1.Key.Lon - double.Parse(startLL[1], CultureInfo.InvariantCulture)) <
                                                                Math.Abs(n2.Key.Lat - double.Parse(startLL[0], CultureInfo.InvariantCulture)) +
                                                                Math.Abs(n2.Key.Lon - double.Parse(startLL[1], CultureInfo.InvariantCulture))
                                                                ? n1 : n2);

            var endLL = endLatLon.Split(',');
            var endNode = PrepareData.NodesGraph.Aggregate((n1, n2) => Math.Abs(n1.Key.Lat - double.Parse(endLL[0], CultureInfo.InvariantCulture)) +
                                                            Math.Abs(n1.Key.Lon - double.Parse(endLL[1], CultureInfo.InvariantCulture)) <
                                                            Math.Abs(n2.Key.Lat - double.Parse(endLL[0], CultureInfo.InvariantCulture)) +
                                                            Math.Abs(n2.Key.Lon - double.Parse(endLL[1], CultureInfo.InvariantCulture))
                                                            ? n1 : n2);

            return new KeyValuePair<KeyValuePair<Node, NodeGraphDTO>, KeyValuePair<Node, NodeGraphDTO>>(startNode, endNode);
        }

        private Routes AfterCalculateShortestPath(NodeGraphDTO nodeCon)
        {
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

            Route route = new Route()
            {
                PocetHranCesty = nodeRoute.Count,
                PocetNavstivenychHran = Utils.PocetNavstivenychHran,
                DlzkaCesty = Math.Round(nodeCon.CurrentDistance, 5),
                CasVypoctu = timeItTook.Milliseconds,
                Nodes = nodeRoute
            };

            Routes routes = new Routes();
            routes.Route.AddLast(route);
            return routes;
        }

        private Routes AfterCalculateShortestPathDuplex(NodeGraphDTO nodeCon)
        {
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

            Route route = new Route()
            {
                PocetHranCesty = nodeRoute.Count,
                PocetNavstivenychHran = Utils.PocetNavstivenychHran,
                DlzkaCesty = Math.Round(nodeCon.CurrentDistance, 5),
                CasVypoctu = timeItTook.Milliseconds,
                Nodes = nodeRoute
            };

            Routes routes = new Routes();
            routes.Route.AddLast(route);
            return routes;
        }

        private Routes AfterCalculateShortestPathMultiLabel(NodeGraphDTO nodeCon)
        {
            Routes routes = new Routes();

            foreach (MultiLabelMark n in nodeCon.MultiLabelMark)
            {
                int k = n.K;
                int xk = k;
                NodeGraphDTO node = nodeCon;
                Route route = null;
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

                route = new Route()
                {
                    PocetHranCesty = nodeRoute.Count,
                    PocetNavstivenychHran = Utils.PocetNavstivenychHran,
                    DlzkaCesty = Math.Round(nodeCon.MultiLabelMark[n.K - 1].T, 5),
                    CasVypoctu = timeItTook.Milliseconds,
                    Nodes = nodeRoute
                };
                routes.Route.AddLast(route);
            }
            return routes;
        }
    }
}