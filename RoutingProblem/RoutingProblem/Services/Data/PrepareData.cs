using Microsoft.EntityFrameworkCore;
using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services.Data
{
    public class PrepareData
    {
        public static Dictionary<Node, NodeGraphDTO> NodesGraph { get; set; }
        public static LinkedList<NodeDisabledMovementDTO> NodesDisabledAll { get; set; }
            = new LinkedList<NodeDisabledMovementDTO>();
        public static Dictionary<Node, NodeDisabledMovementDTO> DisabledMovementGraph { get; set; }
        private static Dictionary<Node, List<KeyValuePair<NodeDisabledMovementDTO, double>>> NodesTempEnd 
            = new Dictionary<Node, List<KeyValuePair<NodeDisabledMovementDTO, double>>>();
        private static Dictionary<Node, List<NodeDisabledMovementDTO>> NodesTempStart
            = new Dictionary<Node, List<NodeDisabledMovementDTO>>();
        private static Dictionary<Node, List<DisabledMovement>> disabledMovement
            = new Dictionary<Node, List<DisabledMovement>>();

        public static void PrepareNodesGraph(IQueryable<Node> nodes, IQueryable<DisabledMovement> disabledMovements)
        {
            NodesGraph = (from n in nodes
                          select new NodeGraphDTO()
                          {
                              Node = n,
                              PreviousNode = null,
                              CurrentDistance = Double.MaxValue,
                              EstimateDistanceToEnd = Double.MaxValue,
                              FScore = Double.MaxValue,
                              EdgeNavigation = n.EdgeStartNodeNavigation,
                              EdgeNavigationR = n.EdgeEndNodeNavigation
                          }).ToDictionary(n => n.Node);

            DisabledMovementGraph = (from n in nodes
                                    select new NodeDisabledMovementDTO()
                                    {
                                        NodeFirst = n,
                                        Node = n,
                                        PreviousNode = null,
                                        CurrentDistance = Double.MaxValue,
                                        EstimateDistanceToEnd = Double.MaxValue,
                                        FScore = Double.MaxValue,
                                        EdgeNavigation = n.EdgeStartNodeNavigation,
                                        EdgeNavigationR = n.EdgeEndNodeNavigation
                                    }).ToDictionary(n => n.Node);

            foreach (var n in NodesGraph)
            {
                foreach (Edge e in n.Value.EdgeNavigation)
                {
                    if (!n.Value.NeighborNodeNavigation.Keys.Contains(NodesGraph[e.EndNodeNavigation]))
                    {
                        n.Value.NeighborNodeNavigation.Add(NodesGraph[e.EndNodeNavigation], e.DistanceInMeters);
                    }
                }

                foreach (Edge e in n.Value.EdgeNavigationR)
                {
                    if (!n.Value.NeighborNodeNavigationR.Keys.Contains(NodesGraph[e.StartNodeNavigation]))
                    {
                        n.Value.NeighborNodeNavigationR.Add(NodesGraph[e.StartNodeNavigation], e.DistanceInMeters);
                    }
                }
            }

            PrepareDisabledMovement(disabledMovements);
            PrepareDisabledMovementGraph();
        }

        private static void PrepareDisabledMovement(IQueryable<DisabledMovement> disabledMovements)
        {
            foreach (var d in disabledMovements)
            {
                if (!disabledMovement.Keys.Contains(d.StartEdgeNavigation.EndNodeNavigation))
                {
                    disabledMovement.Add(d.StartEdgeNavigation.EndNodeNavigation, new List<DisabledMovement>());
                }
                disabledMovement[d.StartEdgeNavigation.EndNodeNavigation].Add(d);
            }
        }

        public static void PrepareDisabledMovementGraph()
        {
            foreach (var n in DisabledMovementGraph)
            {
                NodesDisabledAll.AddLast(n.Value);

                foreach (Edge e in n.Value.EdgeNavigation)
                {
                    if (!n.Value.NeighborNodeNavigation.Keys.Contains(DisabledMovementGraph[e.EndNodeNavigation]))
                    {
                        NodeDisabledMovementDTO ndm = new NodeDisabledMovementDTO()
                        {
                            NodeFirst = n.Key,
                            Node = e.EndNodeNavigation,
                            PreviousNode = null,
                            CurrentDistance = Double.MaxValue,
                            EstimateDistanceToEnd = Double.MaxValue,
                            FScore = Double.MaxValue
                        };
                        NodesDisabledAll.AddLast(ndm);

                        if (!NodesTempEnd.Keys.Contains(e.EndNodeNavigation))
                        {
                            NodesTempEnd.Add(e.EndNodeNavigation, new List<KeyValuePair<NodeDisabledMovementDTO, double>>());
                        }
                        NodesTempEnd[e.EndNodeNavigation].Add(new KeyValuePair<NodeDisabledMovementDTO, double>(ndm, e.DistanceInMeters));

                        if (!NodesTempStart.Keys.Contains(n.Key))
                        {
                            NodesTempStart.Add(n.Key, new List<NodeDisabledMovementDTO>());
                        }
                        NodesTempStart[n.Key].Add(ndm);
                    }
                }
            }

            foreach (var n in DisabledMovementGraph)
            {
                if (NodesTempEnd.Keys.Contains(n.Key))
                {
                    foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmEnd in NodesTempEnd[n.Key])
                    {
                        if (NodesTempStart.Keys.Contains(n.Key))
                        {
                            foreach (NodeDisabledMovementDTO ndmStart in NodesTempStart[n.Key])
                            {
                                bool disMov = false;
                                if (disabledMovement.Keys.Contains(n.Key))
                                {
                                    foreach (DisabledMovement dm in disabledMovement[n.Key])
                                    {
                                        if (ndmEnd.Key.NodeFirst.Equals(dm.StartEdgeNavigation.StartNodeNavigation)
                                            && ndmStart.Node.Equals(dm.EndEdgeNavigation.EndNodeNavigation))
                                        {
                                            disMov = true;
                                            break;
                                        }
                                    }
                                }

                                if (!disMov && !ndmEnd.Key.NeighborNodeNavigation.Keys.Contains(ndmStart))
                                {
                                    ndmEnd.Key.NeighborNodeNavigation.Add(ndmStart, ndmEnd.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void PutStartEnd(Node nodeStart, Node nodeEnd)
        {
            if (DisabledMovementGraph.Keys.Contains(nodeStart))
            {
                foreach (NodeDisabledMovementDTO ndmStart in NodesTempStart[nodeStart])
                {
                    DisabledMovementGraph[nodeStart].NeighborNodeNavigation.Add(ndmStart, 0);
                }
            }

            if (DisabledMovementGraph.Keys.Contains(nodeEnd))
            {
                foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmEnd in NodesTempEnd[nodeEnd])
                {
                    ndmEnd.Key.NeighborNodeNavigation.Add(DisabledMovementGraph[nodeEnd], ndmEnd.Value);
                }
            }
        }

        public static void RemoveStartEnd(Node nodeStart, Node nodeEnd)
        {
            foreach (NodeDisabledMovementDTO ndmStart in NodesTempStart[nodeStart])
            {
                DisabledMovementGraph[nodeStart].NeighborNodeNavigation.Remove(ndmStart);
            }

            foreach (KeyValuePair<NodeDisabledMovementDTO, double> ndmEnd in NodesTempEnd[nodeEnd])
            {
                ndmEnd.Key.NeighborNodeNavigation.Remove(DisabledMovementGraph[nodeEnd]);
            }
        }

        public static void PrepareNodesGraph()
        {
            foreach (var n in NodesGraph)
            {
                PrepareNode(n.Value);
            }

            foreach (var n in DisabledMovementGraph)
            {
                PrepareNode(n.Value);
            }

            foreach (var nd in NodesTempStart)
            {
                foreach (var n in nd.Value)
                {
                    PrepareNode(n);
                }
            }
            Utils.PocetNavstivenychHran = 0;
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
        }
    }
}
