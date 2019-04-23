using RoutingProblem.Models;
using RoutingProblem.Services.Data;
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
            startNode.EstimateDistanceToEnd = Utils.Distance(startNode.Node, endNode.Node);
            startNode.FScore = startNode.EstimateDistanceToEnd;
            startNode.Unsettled = true;

            NodeGraphDTO currentNode = null;
            BinaryTree unsettledTree = new BinaryTree(2);
            unsettledTree.Add(startNode);

            while (!unsettledTree.IsEmpty())
            {
                currentNode = GetLowestDistanceNode(unsettledTree);
                if (currentNode.Equals(endNode))
                {
                    return currentNode;
                }

                foreach (Edge edge in currentNode.Node.EdgeStartNodeNavigation)
                {
                    NodeGraphDTO adjacentNode = edge.EndNodeNavigation.NodeGraphDTO;
                    double edgeWeight = edge.DistanceInMeters;

                    if (!adjacentNode.Settled)
                    {
                        CalculateMinimumDistance(adjacentNode, edgeWeight, currentNode, endNode, unsettledTree);
                        if (!adjacentNode.Unsettled)
                        {
                            unsettledTree.Add(adjacentNode);
                            adjacentNode.Unsettled = true;
                        }
                    }
                }

                Utils.PocetSpracovanychVrcholov++;
                currentNode.Settled = true;
            }
            return null;
        }

        private NodeGraphDTO GetLowestDistanceNode(BinaryTree unsettledTree)
        {
            return unsettledTree.GetMin();
        }

        private void CalculateMinimumDistance(NodeGraphDTO evaluationNode, double edgeWeigh, NodeGraphDTO sourceNode, NodeGraphDTO endNode, BinaryTree unsettledTree)
        {
            double sourceDistance = sourceNode.CurrentDistance;
            if (sourceDistance + edgeWeigh < evaluationNode.CurrentDistance)
            {
                if (evaluationNode.Unsettled)
                {
                    evaluationNode.Unsettled = false;
                    unsettledTree.Remove(evaluationNode);
                }
                evaluationNode.CurrentDistance = sourceDistance + edgeWeigh;
                evaluationNode.EstimateDistanceToEnd = Utils.Distance(evaluationNode.Node, endNode.Node);
                evaluationNode.FScore = evaluationNode.CurrentDistance + evaluationNode.EstimateDistanceToEnd;
                evaluationNode.PreviousNode = sourceNode;
            }
        }
    }
}
