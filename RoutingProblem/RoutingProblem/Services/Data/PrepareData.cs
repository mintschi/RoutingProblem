using Microsoft.EntityFrameworkCore;
using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services.Data
{
    public class PrepareData
    {
        public static Dictionary<Node, NodeGraphDTO> NodesGraph { get; set; }
        public static Dictionary<Node, NodeGraphDTO> DisabledMovementGraph { get; set; }
        private static Dictionary<Node, List<KeyValuePair<NodeDisabledMovementDTO, double>>> NodesTempEnd;
        private static Dictionary<Node, List<KeyValuePair<NodeDisabledMovementDTO, double>>> NodesTempStart;
        private static Dictionary<Node, List<DisabledMovement>> DisabledMovement;

        public static void PrepareNodesGraph(Models.Data data)
        {
            List<Edge> edgesDTO;
            List<DisabledMovement> disMov;
            using (GraphContext dopravnaSietContext = new GraphContext())
            {
                var nDTO = (from n in dopravnaSietContext.Node
                                where n.IdData == data.IdData
                                select new NodeGraphDTO()
                                {
                                    Node = n,
                                });
                nDTO.Load();

                var dm = (from e in dopravnaSietContext.DisabledMovement
                              where e.IdData == data.IdData
                              select new DisabledMovement()
                              {
                                  IdData = data.IdData,
                                  StartEdgeNavigation = e.StartEdgeNavigation,
                                  EndEdgeNavigation = e.EndEdgeNavigation
                              });
                dm.Load();
                disMov = dm.ToList();

                var edges = (from e in dopravnaSietContext.Edge
                             where e.IdData == data.IdData
                             select new Edge()
                             {
                                 IdData = data.IdData,
                                 DistanceInMeters = e.DistanceInMeters,
                                 StartNodeNavigation = e.StartNodeNavigation,
                                 EndNodeNavigation = e.EndNodeNavigation,
                             });
                edges.Load();
                edgesDTO = edges.ToList();
            }

            var nodes = edgesDTO.Select(e => e.StartNodeNavigation).Concat(edgesDTO.Select(e => e.EndNodeNavigation)).Distinct();
            var a = nodes.Count();
            var nodesDTO = (from n in nodes
                        where n.IdData == data.IdData
                        select new NodeGraphDTO()
                        {
                            Node = n
                        });
            NodesGraph = nodesDTO.ToDictionary(n => n.Node);
            foreach (NodeGraphDTO node in NodesGraph.Values)
            {
                node.Node.NodeGraphDTO = node;
            }

            foreach (Edge edge in edgesDTO)
            {
                if (!edge.StartNodeNavigation.EdgeStartNodeNavigation.Contains(edge))
                    edge.StartNodeNavigation.EdgeStartNodeNavigation.Add(edge);
                if (!edge.EndNodeNavigation.EdgeEndNodeNavigation.Contains(edge))
                    edge.EndNodeNavigation.EdgeEndNodeNavigation.Add(edge);
            }

            PrepareDisabledMovement(edgesDTO, data, disMov);
        }

        private static void PrepareDisabledMovement(List<Edge> edges, Models.Data data, List<DisabledMovement> dm)
        {
            DisabledMovement = new Dictionary<Node, List<DisabledMovement>>();
            foreach (var d in dm)
            {
                if (!DisabledMovement.Keys.Contains(d.StartEdgeNavigation.EndNodeNavigation))
                {
                    DisabledMovement.Add(d.StartEdgeNavigation.EndNodeNavigation, new List<DisabledMovement>());
                }
                DisabledMovement[d.StartEdgeNavigation.EndNodeNavigation].Add(d);
            }
        }

        public static void PrepareDisabledMovementGraph()
        {
            DisabledMovementGraph = new Dictionary<Node, NodeGraphDTO>(NodesGraph);
            NodesTempEnd = new Dictionary<Node, List<KeyValuePair<NodeDisabledMovementDTO, double>>>();
            NodesTempStart = new Dictionary<Node, List<KeyValuePair<NodeDisabledMovementDTO, double>>>();

            foreach (var n in NodesGraph)
            {
                foreach (Edge e in n.Key.EdgeStartNodeNavigation)
                {
                    NodeDisabledMovementDTO ndm = new NodeDisabledMovementDTO()
                    {
                        NodeFirst = n.Key,
                        Node = new Node()
                        {
                            IdNode = e.EndNodeNavigation.IdNode,
                            Lat = e.EndNodeNavigation.Lat,
                            Lon = e.EndNodeNavigation.Lon
                        },
                    };
                    ndm.Node.NodeGraphDTO = ndm;
                    DisabledMovementGraph.Add(ndm.Node, ndm);

                    if (!NodesTempEnd.Keys.Contains(e.EndNodeNavigation))
                    {
                        NodesTempEnd.Add(e.EndNodeNavigation, new List<KeyValuePair<NodeDisabledMovementDTO, double>>());
                    }
                    NodesTempEnd[e.EndNodeNavigation].Add(new KeyValuePair<NodeDisabledMovementDTO, double>(ndm, e.DistanceInMeters));

                    if (!NodesTempStart.Keys.Contains(n.Key))
                    {
                        NodesTempStart.Add(n.Key, new List<KeyValuePair<NodeDisabledMovementDTO, double>>());
                    }
                    NodesTempStart[n.Key].Add(new KeyValuePair<NodeDisabledMovementDTO, double>(ndm, e.DistanceInMeters));
                }
            }

            foreach (var n in NodesGraph)
            {
                if (NodesTempEnd.Keys.Contains(n.Key))
                {
                    foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmEnd in NodesTempEnd[n.Key])
                    {
                        if (NodesTempStart.Keys.Contains(n.Key))
                        {
                            foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmStart in NodesTempStart[n.Key])
                            {
                                bool disMov = false;
                                if (DisabledMovement.Keys.Contains(n.Key))
                                {
                                    foreach (DisabledMovement dm in DisabledMovement[n.Key])
                                    {
                                        if (ndmEnd.Key.NodeFirst.Equals(dm.StartEdgeNavigation.StartNodeNavigation)
                                            && ndmStart.Key.Node.IdNode.Equals(dm.EndEdgeNavigation.EndNodeNavigation.IdNode))
                                        {
                                            disMov = true;
                                            break;
                                        }
                                    }
                                }

                                if (!disMov)
                                {
                                    Edge edge = new Edge()
                                    {
                                        DistanceInMeters = ndmStart.Value,
                                        EndNodeNavigation = ndmStart.Key.Node,
                                        StartNodeNavigation = ndmEnd.Key.Node
                                    };
                                    ndmEnd.Key.Node.EdgeStartNodeNavigation.Add(edge);
                                    ndmStart.Key.Node.EdgeEndNodeNavigation.Add(edge);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static List<Edge> tempStartEdge;
        private static List<Edge> tempEndEdge;
        public static void PutStartEnd(NodeGraphDTO nodeStart, NodeGraphDTO nodeEnd)
        {
            int idEdge = -1;
            tempStartEdge = nodeStart.Node.EdgeStartNodeNavigation.ToList();
            nodeStart.Node.EdgeStartNodeNavigation.Clear();
            if (NodesGraph.Keys.Contains(nodeStart.Node))
            {
                foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmStart in NodesTempStart[nodeStart.Node])
                {
                    Edge edge = new Edge()
                    {
                        IdEdge = idEdge,
                        DistanceInMeters = ndmStart.Value,
                        EndNodeNavigation = ndmStart.Key.Node,
                        StartNodeNavigation = nodeStart.Node
                    };
                    nodeStart.Node.EdgeStartNodeNavigation.Add(edge);
                    ndmStart.Key.Node.EdgeEndNodeNavigation.Add(edge);
                }
            }

            tempEndEdge = nodeEnd.Node.EdgeEndNodeNavigation.ToList();
            nodeEnd.Node.EdgeEndNodeNavigation.Clear();
            if (NodesGraph.Keys.Contains(nodeEnd.Node))
            {
                foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmEnd in NodesTempEnd[nodeEnd.Node])
                {
                    Edge edge = new Edge()
                    {
                        IdEdge = idEdge,
                        DistanceInMeters = 0,
                        EndNodeNavigation = nodeEnd.Node,
                        StartNodeNavigation = ndmEnd.Key.Node
                    };
                    ndmEnd.Key.Node.EdgeStartNodeNavigation.Add(edge);
                    nodeEnd.Node.EdgeEndNodeNavigation.Add(edge);
                }
            }
        }

        public static void RemoveStartEnd(NodeGraphDTO nodeStart, NodeGraphDTO nodeEnd)
        {
            nodeStart.Node.EdgeStartNodeNavigation.Clear();
            tempStartEdge.ForEach(e => nodeStart.Node.EdgeStartNodeNavigation.Add(e));

            Dictionary<NodeDisabledMovementDTO, Edge> edgeForRemoveEnd = new Dictionary<NodeDisabledMovementDTO, Edge>();
            foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmEnd in NodesTempEnd[nodeEnd.Node])
            {
                foreach (Edge edge in ndmEnd.Key.Node.EdgeStartNodeNavigation)
                {
                    if (edge.IdEdge == -1)
                    {
                        edgeForRemoveEnd.Add(ndmEnd.Key, edge);
                    }
                }
            }
            foreach (KeyValuePair<NodeDisabledMovementDTO, Edge> remove in edgeForRemoveEnd)
            {
                remove.Key.Node.EdgeStartNodeNavigation.Remove(remove.Value);
            }

            nodeEnd.Node.EdgeEndNodeNavigation.Clear();
            tempEndEdge.ForEach(e => nodeEnd.Node.EdgeEndNodeNavigation.Add(e));

            edgeForRemoveEnd = new Dictionary<NodeDisabledMovementDTO, Edge>();
            foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmStart in NodesTempStart[nodeStart.Node])
            {
                foreach (Edge edge in ndmStart.Key.Node.EdgeEndNodeNavigation)
                {
                    if (edge.IdEdge == -1)
                    {
                        edgeForRemoveEnd.Add(ndmStart.Key, edge);
                    }
                }
            }
            foreach (KeyValuePair<NodeDisabledMovementDTO, Edge> remove in edgeForRemoveEnd)
            {
                remove.Key.Node.EdgeEndNodeNavigation.Remove(remove.Value);
            }
        }

        public static void PrepareNodesGraph()
        {
            foreach (var n in DisabledMovementGraph)
            {
                PrepareNode(n.Value);
            }
            Utils.PocetSpracovanychVrcholov = 0;
        }

        private static void PrepareNode(NodeGraphDTO n)
        {
            n.PreviousNode = null;
            n.CurrentDistance = Double.MaxValue;
            n.EstimateDistanceToEnd = Double.MaxValue;
            n.FScore = Double.MaxValue;
            n.PreviousNodeR = null;
            n.CurrentDistanceR = Double.MaxValue;
            n.MultiLabelMark.Clear();
            n.Settled = false;
            n.Unsettled = false;
            n.SettledR = false;
            n.UnsettledR = false;
        }

        private static void PrepareNode(NodeDisabledMovementDTO n)
        {
            n.PreviousNode = null;
            n.CurrentDistance = Double.MaxValue;
            n.EstimateDistanceToEnd = Double.MaxValue;
            n.FScore = Double.MaxValue;
            n.PreviousNodeR = null;
            n.CurrentDistanceR = Double.MaxValue;
            n.MultiLabelMark.Clear();
            n.Settled = false;
            n.Unsettled = false;
            n.SettledR = false;
            n.UnsettledR = false;
        }
    }
}
