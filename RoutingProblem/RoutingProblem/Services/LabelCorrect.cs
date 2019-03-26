using RoutingProblem.Models;
using RoutingProblem.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services
{
    public class LabelCorrect
    {
        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode)
        {
            startNode.CurrentDistance = 0;

            NodeGraphDTO currentNode = null;
            Queue<NodeGraphDTO> unsettledNodes = new Queue<NodeGraphDTO>();

            unsettledNodes.Enqueue(startNode);

            while (unsettledNodes.Count() != 0)
            {
                currentNode = unsettledNodes.Dequeue();

                foreach (KeyValuePair<NodeGraphDTO, double> edge in currentNode.NeighborNodeNavigation)
                {
                    NodeGraphDTO adjacentNode = edge.Key;
                    double edgeWeight = edge.Value;

                    double sourceDistance = currentNode.CurrentDistance;
                    if (sourceDistance + edgeWeight < adjacentNode.CurrentDistance)
                    {
                        adjacentNode.CurrentDistance = sourceDistance + edgeWeight;
                        adjacentNode.PreviousNode = currentNode;
                        unsettledNodes.Enqueue(adjacentNode);
                    }

                    Utils.PocetNavstivenychHran++;
                }
            }
            return endNode;
        }
    }
}
