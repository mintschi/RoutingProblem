using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutingProblem.Models;

namespace RoutingProblem.Services
{
    public class DuplexDijkster
    {
        private bool straight = true;
        private HashSet<NodeGraphDTO> settledNodesS = new HashSet<NodeGraphDTO>();
        private HashSet<NodeGraphDTO> unsettledNodesS = new HashSet<NodeGraphDTO>();
        private HashSet<NodeGraphDTO> settledNodesR = new HashSet<NodeGraphDTO>();
        private HashSet<NodeGraphDTO> unsettledNodesR = new HashSet<NodeGraphDTO>();
        private HashSet<NodeGraphDTO> settledNodesB = new HashSet<NodeGraphDTO>();

        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode)
        {
            startNode.CurrentDistance = 0;
            endNode.CurrentDistanceR = 0;

            unsettledNodesS.Add(startNode);
            unsettledNodesR.Add(endNode);

            bool found = false;

            while (unsettledNodesS.Count() > 0 && unsettledNodesR.Count() > 0 && !found)
            {
                if (straight) {
                    found = CalculateShortestPathS();
                    straight = false;
                } else
                {
                    found = CalculateShortestPathR();
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

        private bool CalculateShortestPathS()
        {
            NodeGraphDTO currentNode = GetLowestDistanceNode(unsettledNodesS);
            if (this.settledNodesR.Contains(currentNode))
            {
                return true;
            }
            unsettledNodesS.Remove(currentNode);
            foreach (KeyValuePair<NodeGraphDTO, double> edge in currentNode.NeighborNodeNavigation)
            {
                NodeGraphDTO adjacentNode = edge.Key;
                double edgeWeight = edge.Value;

                if (!settledNodesS.Contains(adjacentNode))
                {
                    CalculateMinimumDistance(adjacentNode, edgeWeight, currentNode);
                    unsettledNodesS.Add(adjacentNode);
                    Utils.PocetNavstivenychHran++;
                }
            }
            settledNodesS.Add(currentNode);
            return false;
        }

        private bool CalculateShortestPathR()
        {
            NodeGraphDTO currentNode = GetLowestDistanceNode(unsettledNodesR);
            if (this.settledNodesS.Contains(currentNode))
            {
                return true;
            }
            unsettledNodesR.Remove(currentNode);
            foreach (KeyValuePair<NodeGraphDTO, double> edge in currentNode.NeighborNodeNavigationR)
            {
                NodeGraphDTO adjacentNode = edge.Key;
                double edgeWeight = edge.Value;

                if (!settledNodesR.Contains(adjacentNode))
                {
                    CalculateMinimumDistance(adjacentNode, edgeWeight, currentNode);
                    unsettledNodesR.Add(adjacentNode);
                    Utils.PocetNavstivenychHran++;
                }
            }
            settledNodesR.Add(currentNode);
            return false;
        }

        private NodeGraphDTO GetLowestDistanceNode(HashSet<NodeGraphDTO> unsettledNodes)
        {
            NodeGraphDTO lowestDistanceNode = null;
            double lowestDistance = Double.MaxValue;
            foreach (NodeGraphDTO node in unsettledNodes)
            {
                double nodeDistance = straight ? node.CurrentDistance : node.CurrentDistanceR;
                if (nodeDistance < lowestDistance)
                {
                    lowestDistance = nodeDistance;
                    lowestDistanceNode = node;
                }
            }
            return lowestDistanceNode;
        }

        private void CalculateMinimumDistance(NodeGraphDTO evaluationNode, double edgeWeigh, NodeGraphDTO sourceNode)
        {
            double sourceDistance = straight ? sourceNode.CurrentDistance : sourceNode.CurrentDistanceR;
            if ((straight && sourceDistance + edgeWeigh < evaluationNode.CurrentDistance) ||
                (!straight && sourceDistance + edgeWeigh < evaluationNode.CurrentDistanceR))
            {
                if (straight)
                {
                    evaluationNode.CurrentDistance = sourceDistance + edgeWeigh;
                    evaluationNode.PreviousNode = sourceNode;
                } else
                {
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
