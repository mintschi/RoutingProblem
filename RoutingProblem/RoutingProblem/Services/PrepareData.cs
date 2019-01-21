using Microsoft.EntityFrameworkCore;
using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services
{
    public class PrepareData
    {
        public static Dictionary<Node, NodeGraphDTO> NodesGraph { get; set; }

        public static void PrepareNodesGraph(DbSet<Node> nodes)
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
        }

        public static void PrepareNodesGraph()
        {
            foreach (var n in NodesGraph)
            {
                n.Value.PreviousNode = null;
                n.Value.CurrentDistance = Double.MaxValue;
                n.Value.EstimateDistanceToEnd = Double.MaxValue;
                n.Value.FScore = Double.MaxValue;
                n.Value.PreviousNodeR = null;
                n.Value.CurrentDistanceR = Double.MaxValue;
            }
            Utils.PocetNavstivenychHran = 0;
        }
    }
}
