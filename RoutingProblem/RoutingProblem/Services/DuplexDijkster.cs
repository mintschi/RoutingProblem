using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutingProblem.Models;
using RoutingProblem.Services.Data;

namespace RoutingProblem.Services
{
    public class DuplexDijkster
    {
        private bool straight = true;
        BinaryTree unsettledTree = new BinaryTree(1);
        BinaryTree unsettledTreeR = new BinaryTree(3);
        private HashSet<NodeGraphDTO> settledNodesB = new HashSet<NodeGraphDTO>();

        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode)
        {
            straight = true;
            startNode.CurrentDistance = 0;
            endNode.CurrentDistanceR = 0;
            unsettledTree.Clear();
            unsettledTreeR.Clear();
            settledNodesB = new HashSet<NodeGraphDTO>();

            unsettledTree.Add(startNode);
            unsettledTreeR.Add(endNode);

            bool found = false;

            while (!unsettledTree.IsEmpty() && !unsettledTreeR.IsEmpty() && !found)
            {
                if (straight)
                {
                    found = CalculateShortestPathS(endNode);
                    straight = false;
                }
                else
                {
                    found = CalculateShortestPathR(startNode);
                    straight = true;
                }
            }

            if (found)
            {
                NodeGraphDTO lowestDistanceNode = null;
                double lowestDistance = Double.MaxValue;
                foreach (NodeGraphDTO node in settledNodesB)
                {
                    double nodeDistance = node.CurrentDistance + node.CurrentDistanceR;
                    if (nodeDistance < lowestDistance)
                    {
                        lowestDistance = nodeDistance;
                        lowestDistanceNode = node;
                    }
                }
                lowestDistanceNode.CurrentDistance = lowestDistance;
                return lowestDistanceNode;
            }
            return null;
        }

        private bool CalculateShortestPathS(NodeGraphDTO endNode)
        {
            NodeGraphDTO currentNode = GetLowestDistanceNode(unsettledTree);
            if (currentNode.SettledR)
            {
                return true;
            }
            
            foreach (Edge edge in currentNode.Node.EdgeStartNodeNavigation)
            {
                NodeGraphDTO adjacentNode = edge.EndNodeNavigation.NodeGraphDTO;
                double edgeWeight = edge.DistanceInMeters;

                if (!adjacentNode.Settled)
                {
                    CalculateMinimumDistance(adjacentNode, edgeWeight, currentNode, unsettledTree);
                    if (adjacentNode.Equals(endNode))
                    {
                        settledNodesB.Add(adjacentNode);
                    }
                    if (!adjacentNode.Unsettled)
                    {
                        unsettledTree.Add(adjacentNode);
                        adjacentNode.Unsettled = true;
                    }
                }
            }

            Utils.PocetSpracovanychVrcholov++;
            currentNode.Settled = true;
            return false;
        }

        private bool CalculateShortestPathR(NodeGraphDTO startNode)
        {
            NodeGraphDTO currentNode = GetLowestDistanceNode(unsettledTreeR);
            if (currentNode.Settled)
            {
                return true;
            }

            foreach (Edge edge in currentNode.Node.EdgeEndNodeNavigation)
            {
                NodeGraphDTO adjacentNode = edge.StartNodeNavigation.NodeGraphDTO;
                double edgeWeight = edge.DistanceInMeters;

                if (!adjacentNode.SettledR)
                {
                    CalculateMinimumDistance(adjacentNode, edgeWeight, currentNode, unsettledTreeR);
                    if (adjacentNode.Equals(startNode))
                    {
                        settledNodesB.Add(adjacentNode);
                    }
                    if (!adjacentNode.UnsettledR)
                    {
                        unsettledTreeR.Add(adjacentNode);
                        adjacentNode.UnsettledR = true;
                    }
                }
            }

            Utils.PocetSpracovanychVrcholov++;
            currentNode.SettledR = true;
            return false;
        }

        private NodeGraphDTO GetLowestDistanceNode(BinaryTree unsettledTree)
        {
            return unsettledTree.GetMin();
        }

        private void CalculateMinimumDistance(NodeGraphDTO evaluationNode, double edgeWeigh, NodeGraphDTO sourceNode, BinaryTree unsettledTree)
        {
            double sourceDistance = straight ? sourceNode.CurrentDistance : sourceNode.CurrentDistanceR;
            if ((straight && sourceDistance + edgeWeigh < evaluationNode.CurrentDistance) ||
                (!straight && sourceDistance + edgeWeigh < evaluationNode.CurrentDistanceR))
            {
                if (straight)
                {
                    if (evaluationNode.Unsettled)
                    {
                        evaluationNode.Unsettled = false;
                        unsettledTree.Remove(evaluationNode);
                    }
                    evaluationNode.CurrentDistance = sourceDistance + edgeWeigh;
                    evaluationNode.PreviousNode = sourceNode;
                }
                else
                {
                    if (evaluationNode.UnsettledR)
                    {
                        evaluationNode.UnsettledR = false;
                        unsettledTree.Remove(evaluationNode);
                    }
                    evaluationNode.CurrentDistanceR = sourceDistance + edgeWeigh;
                    evaluationNode.PreviousNodeR = sourceNode;
                }
                if (evaluationNode.PreviousNode != null && evaluationNode.PreviousNodeR != null)
                {
                    settledNodesB.Add(evaluationNode);
                }
            }
        }
    }
}
