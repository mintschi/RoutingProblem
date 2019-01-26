using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutingProblem.Data;
using RoutingProblem.Models;

namespace RoutingProblem.Services
{
    public class MultiLabel
    {
        public int K { get; set; }

        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode)
        {
            startNode.MultiLabelMark.Add(new MultiLabelMark(0, 0, null, 0));

            MultiLabelMarkQueue currentMark = null;
            HashSet<MultiLabelMarkQueue> unsettledNodes = new HashSet<MultiLabelMarkQueue>();

            foreach (KeyValuePair<NodeGraphDTO, double> edge in startNode.NeighborNodeNavigation)
            {
                unsettledNodes.Add(new MultiLabelMarkQueue(edge.Key, edge.Value, startNode, 0));
            }

            while (unsettledNodes.Count() != 0 && endNode.MultiLabelMark.Count < K)
            {
                currentMark = GetLowestDistanceNode(unsettledNodes);
                unsettledNodes.Remove(currentMark);

                int k = currentMark.W.MultiLabelMark.Count;
                if (k < K)
                {
                    k++;
                    currentMark.W.MultiLabelMark.Add(new MultiLabelMark(k, currentMark.T, currentMark.X, currentMark.Xk));

                    foreach (KeyValuePair<NodeGraphDTO, double> edge in currentMark.W.NeighborNodeNavigation)
                    {
                        if (!PathContainsNode(edge.Key, currentMark.W, k))
                        {
                            unsettledNodes.Add(new MultiLabelMarkQueue(edge.Key, currentMark.T + edge.Value, currentMark.W, k));
                        }
                    }
                }
            }
            return endNode;
        }

        private bool PathContainsNode(NodeGraphDTO nodeI, NodeGraphDTO nodePath, int kk)
        {
            NodeGraphDTO node = nodePath;
            int k = kk;
            int xk;
            while (node.MultiLabelMark[k - 1].Xk != 0)
            {
                xk = node.MultiLabelMark[k - 1].Xk;
                node = node.MultiLabelMark[k - 1].X;
                if (node.Equals(nodeI))
                {
                    return true;
                }
                k = xk;
            }
            return false;
        }

        private MultiLabelMarkQueue GetLowestDistanceNode(HashSet<MultiLabelMarkQueue> unsettledNodes)
        {
            MultiLabelMarkQueue lowestDistanceMark = null;
            double lowestDistance = Double.MaxValue;
            foreach (MultiLabelMarkQueue mark in unsettledNodes)
            {
                double nodeDistance = mark.T;
                if (nodeDistance < lowestDistance)
                {
                    lowestDistance = nodeDistance;
                    lowestDistanceMark = mark;
                }
            }
            return lowestDistanceMark;
        }
    }
}
