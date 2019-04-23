using Microsoft.EntityFrameworkCore;
using RoutingProblem.Models;
using RoutingProblem.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoutingProblem.Services
{
    public class Zakladny
    {
        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode, Dictionary<Node, NodeGraphDTO> nodes)
        {
            startNode.CurrentDistance = 0;
            NodeGraphDTO currentNode = null;
            bool repeat = true;

            while (repeat)
            {
                repeat = false;
                foreach (KeyValuePair<Node, NodeGraphDTO> node in nodes)
                {
                    currentNode = node.Value;
                    foreach (Edge edge in currentNode.Node.EdgeStartNodeNavigation)
                    {
                        NodeGraphDTO adjacentNode = edge.EndNodeNavigation.NodeGraphDTO;
                        double edgeWeight = edge.DistanceInMeters;

                        double sourceDistance = currentNode.CurrentDistance;
                        if (sourceDistance + edgeWeight < adjacentNode.CurrentDistance)
                        {
                            adjacentNode.CurrentDistance = sourceDistance + edgeWeight;
                            adjacentNode.PreviousNode = currentNode;
                            repeat = true;
                        }
                    }
                    Utils.PocetSpracovanychVrcholov++;
                }
            }

            if (endNode.PreviousNode == null)
            {
                return null;
            }
            return endNode;
        }

        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode, LinkedList<NodeDisabledMovementDTO> nodes)
        {
            startNode.CurrentDistance = 0;
            NodeGraphDTO currentNode = null;
            bool repeat = true;

            while (repeat)
            {
                repeat = false;
                foreach (NodeDisabledMovementDTO node in nodes)
                {
                    currentNode = node;
                    foreach (Edge edge in currentNode.Node.EdgeStartNodeNavigation)
                    {
                        NodeGraphDTO adjacentNode = PrepareData.NodesGraph[edge.EndNodeNavigation];
                        double edgeWeight = edge.DistanceInMeters;

                        double sourceDistance = currentNode.CurrentDistance;
                        if (sourceDistance + edgeWeight < adjacentNode.CurrentDistance)
                        {
                            adjacentNode.CurrentDistance = sourceDistance + edgeWeight;
                            adjacentNode.PreviousNode = currentNode;
                            repeat = true;
                        }

                        Utils.PocetSpracovanychVrcholov++;
                    }
                }
            }

            return endNode;
        }
    }
}
