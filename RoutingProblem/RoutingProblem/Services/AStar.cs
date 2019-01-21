using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services
{
    public class AStar
    {
        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode)
        {
            startNode.CurrentDistance = 0;
            startNode.EstimateDistanceToEnd = Utils.Vzdialenost(startNode.Node, endNode.Node);
            startNode.FScore = startNode.EstimateDistanceToEnd;

            NodeGraphDTO currentNode = null;
            HashSet<NodeGraphDTO> settledNodes = new HashSet<NodeGraphDTO>();
            HashSet<NodeGraphDTO> unsettledNodes = new HashSet<NodeGraphDTO>();

            unsettledNodes.Add(startNode);

            while (unsettledNodes.Count() != 0)
            {
                currentNode = GetLowestDistanceNode(unsettledNodes);
                if (currentNode.Equals(endNode))
                {
                    return currentNode;
                }
                unsettledNodes.Remove(currentNode);
                foreach (KeyValuePair<NodeGraphDTO, double> edge in currentNode.NeighborNodeNavigation)
                {
                    NodeGraphDTO adjacentNode = edge.Key;
                    double edgeWeight = edge.Value;

                    if (!settledNodes.Contains(adjacentNode))
                    {
                        CalculateMinimumDistance(adjacentNode, edgeWeight, currentNode, endNode);
                        unsettledNodes.Add(adjacentNode);
                        Utils.PocetNavstivenychHran++;
                    }
                }
                settledNodes.Add(currentNode);
            }
            return null;
        }

        private NodeGraphDTO GetLowestDistanceNode(HashSet<NodeGraphDTO> unsettledNodes)
        {
            NodeGraphDTO lowestDistanceNode = null;
            double lowestDistance = Double.MaxValue;
            foreach (NodeGraphDTO node in unsettledNodes)
            {
                double nodeDistance = node.FScore;
                if (nodeDistance < lowestDistance)
                {
                    lowestDistance = nodeDistance;
                    lowestDistanceNode = node;
                }
            }
            return lowestDistanceNode;
        }

        private void CalculateMinimumDistance(NodeGraphDTO evaluationNode, double edgeWeigh, NodeGraphDTO sourceNode, NodeGraphDTO endNode)
        {
            double sourceDistance = sourceNode.CurrentDistance;
            if (sourceDistance + edgeWeigh < evaluationNode.CurrentDistance)
            {
                evaluationNode.CurrentDistance = sourceDistance + edgeWeigh;
                evaluationNode.EstimateDistanceToEnd = Utils.Vzdialenost(evaluationNode.Node, endNode.Node);
                evaluationNode.FScore = evaluationNode.CurrentDistance + evaluationNode.EstimateDistanceToEnd;
                evaluationNode.PreviousNode = sourceNode;
            }
        }
    }
}
