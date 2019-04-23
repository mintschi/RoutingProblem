using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoutingProblem.Models;
using RoutingProblem.Services.Data;

namespace RoutingProblem.Services
{
    public class MultiLabel
    {
        public int K { get; set; }

        public NodeGraphDTO CalculateShortestPath(NodeGraphDTO startNode, NodeGraphDTO endNode)
        {
            startNode.MultiLabelMark.Add(new MultiLabelMark(0, 0, null, 0));

            MultiLabelMarkQueue currentMark = null;
            BinaryTree unsettledTree = new BinaryTree();

            foreach (Edge edge in startNode.Node.EdgeStartNodeNavigation)
            {
                unsettledTree.Add(new MultiLabelMarkQueue(edge.EndNodeNavigation.NodeGraphDTO, edge.DistanceInMeters, startNode, 0));
            }

            while (!unsettledTree.IsEmpty() && endNode.MultiLabelMark.Count < K)
            {
                currentMark = GetLowestDistanceNode(unsettledTree);

                int k = currentMark.W.MultiLabelMark.Count;
                if (k < K)
                {
                    k++;
                    currentMark.W.MultiLabelMark.Add(new MultiLabelMark(k, currentMark.T, currentMark.X, currentMark.Xk));

                    foreach (Edge edge in currentMark.W.Node.EdgeStartNodeNavigation)
                    {
                        if (!PathContainsNode(edge.EndNodeNavigation.NodeGraphDTO, currentMark.W, k))
                        {
                            unsettledTree.Add(new MultiLabelMarkQueue(edge.EndNodeNavigation.NodeGraphDTO, currentMark.T + edge.DistanceInMeters, currentMark.W, k));
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

        private MultiLabelMarkQueue GetLowestDistanceNode(BinaryTree unsettledTree)
        {
            return unsettledTree.GetMinMultiLabel();
        }
    }
}
