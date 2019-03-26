using RoutingProblem.Models;
using RoutingProblem.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services
{
    public class LabelSet
    {
        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode)
        {
            startNode.CurrentDistance = 0;

            NodeGraphDTO currentNode = null;
            HashSet<NodeGraphDTO> settledNodes = new HashSet<NodeGraphDTO>();
            HashSet<NodeGraphDTO> unsettledNodes = new HashSet<NodeGraphDTO>();

            unsettledNodes.Add(startNode);

            while (unsettledNodes.Count() != 0)
            {
                currentNode = unsettledNodes.OrderBy(n => n.CurrentDistance).First();
                unsettledNodes.Remove(currentNode);
                if (currentNode.Equals(endNode))
                {
                    return currentNode;
                }

                foreach (KeyValuePair<NodeGraphDTO, double> edge in currentNode.NeighborNodeNavigation)
                {
                    NodeGraphDTO adjacentNode = edge.Key;
                    double edgeWeight = edge.Value;

                    if (!settledNodes.Contains(adjacentNode))
                    {
                        CalculateMinimumDistance(adjacentNode, edgeWeight, currentNode, unsettledNodes);
                        Utils.PocetNavstivenychHran++;
                    }
                }
                settledNodes.Add(currentNode);
            }
            return null;
        }

        private void CalculateMinimumDistance(NodeGraphDTO evaluationNode, double edgeWeigh, NodeGraphDTO sourceNode, HashSet<NodeGraphDTO> unsettledNodes)
        {
            double sourceDistance = sourceNode.CurrentDistance;
            if (sourceDistance + edgeWeigh < evaluationNode.CurrentDistance)
            {
                evaluationNode.CurrentDistance = sourceDistance + edgeWeigh;
                evaluationNode.PreviousNode = sourceNode;
                unsettledNodes.Add(evaluationNode);
            }
        }
    }
}
