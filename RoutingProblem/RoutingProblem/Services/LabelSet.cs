using RoutingProblem.Models;
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
            Queue<NodeGraphDTO> unsettledNodes = new Queue<NodeGraphDTO>();

            unsettledNodes.Enqueue(startNode);

            while (unsettledNodes.Count() != 0)
            {
                currentNode = unsettledNodes.Dequeue();
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

        private void CalculateMinimumDistance(NodeGraphDTO evaluationNode, double edgeWeigh, NodeGraphDTO sourceNode, Queue<NodeGraphDTO> unsettledNodes)
        {
            double sourceDistance = sourceNode.CurrentDistance;
            if (sourceDistance + edgeWeigh < evaluationNode.CurrentDistance)
            {
                evaluationNode.CurrentDistance = sourceDistance + edgeWeigh;
                evaluationNode.PreviousNode = sourceNode;
                unsettledNodes.Enqueue(evaluationNode);
            }
        }
    }
}
