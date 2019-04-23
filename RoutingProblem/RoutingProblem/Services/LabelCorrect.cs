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
                currentNode.Unsettled = false;

                foreach (Edge edge in currentNode.Node.EdgeStartNodeNavigation)
                {
                    NodeGraphDTO adjacentNode = edge.EndNodeNavigation.NodeGraphDTO;
                    double edgeWeight = edge.DistanceInMeters;

                    double sourceDistance = currentNode.CurrentDistance;
                    if (sourceDistance + edgeWeight < adjacentNode.CurrentDistance)
                    {
                        adjacentNode.CurrentDistance = sourceDistance + edgeWeight;
                        adjacentNode.PreviousNode = currentNode;
                        if (!adjacentNode.Unsettled)
                        {
                            unsettledNodes.Enqueue(adjacentNode);
                            adjacentNode.Unsettled = true;
                        }
                    }
                }
                Utils.PocetSpracovanychVrcholov++;
            }

            if (endNode.PreviousNode == null)
            {
                return null;
            }
            return endNode;
        }
    }
}
