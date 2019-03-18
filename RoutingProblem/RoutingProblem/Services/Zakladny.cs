﻿using Microsoft.EntityFrameworkCore;
using RoutingProblem.Models;
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

            while(repeat)
            {
                repeat = false;
                foreach (KeyValuePair<Node, NodeGraphDTO> node in nodes)
                {
                    currentNode = node.Value;
                    foreach (KeyValuePair<NodeGraphDTO, double> edge in node.Value.NeighborNodeNavigation)
                    {
                        NodeGraphDTO adjacentNode = edge.Key;
                        double edgeWeight = edge.Value;

                        double sourceDistance = currentNode.CurrentDistance;
                        if (sourceDistance + edgeWeight < adjacentNode.CurrentDistance)
                        {
                            adjacentNode.CurrentDistance = sourceDistance + edgeWeight;
                            adjacentNode.PreviousNode = currentNode;
                            repeat = true;
                        }

                        Utils.PocetNavstivenychHran++;
                    }
                }
            }
            
            return endNode;
        }
    }
}