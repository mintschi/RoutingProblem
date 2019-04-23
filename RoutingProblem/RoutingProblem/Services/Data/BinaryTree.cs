using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Services.Data
{
    public class BinaryTree
    {
        private NodeTree root;
        private int comparer;

        public BinaryTree(Dictionary<Node, NodeGraphDTO> nodes, int compare = 0)
        {
            comparer = compare;
            foreach (NodeGraphDTO node in nodes.Values)
            {
                Add(node);
            }
        }

        public BinaryTree(int compare = 0)
        {
            comparer = compare;
        }

        public bool IsEmpty()
        {
            return root == null;
        }

        public void Clear()
        {
            root = null;
        }

        public void Add(NodeGraphDTO node)
        {
            if (root == null)
            {
                root = new NodeTree(node);
            }
            else
            {
                AddPrivate(root, node);
            }
        }

        private void AddPrivate(NodeTree nodeTree, NodeGraphDTO nodeNew)
        {
            if ((comparer == 0 && nodeNew.Node.IdNode <= nodeTree.node.Node.IdNode)
                || (comparer == 1 && nodeNew.CurrentDistance <= nodeTree.node.CurrentDistance)
                || (comparer == 2 && nodeNew.FScore <= nodeTree.node.FScore)
                || (comparer == 3 && nodeNew.CurrentDistanceR <= nodeTree.node.CurrentDistanceR))
            {
                if (nodeTree.left == null)
                {
                    nodeTree.left = new NodeTree(nodeNew);
                    return;
                }
                else
                {
                    AddPrivate(nodeTree.left, nodeNew);
                }
            }
            else
            {
                if (nodeTree.right == null)
                {
                    nodeTree.right = new NodeTree(nodeNew);
                    return;
                }
                else
                {
                    AddPrivate(nodeTree.right, nodeNew);
                }
            }
        }

        public void Add(MultiLabelMarkQueue node)
        {
            if (root == null)
            {
                root = new NodeTree(node);
            }
            else
            {
                AddPrivate(root, node);
            }
        }

        private void AddPrivate(NodeTree nodeTree, MultiLabelMarkQueue nodeNew)
        {
            if (nodeNew.T <= nodeTree.nodeMultiLabel.T)
            {
                if (nodeTree.left == null)
                {
                    nodeTree.left = new NodeTree(nodeNew);
                    return;
                }
                else
                {
                    AddPrivate(nodeTree.left, nodeNew);
                }
            }
            else
            {
                if (nodeTree.right == null)
                {
                    nodeTree.right = new NodeTree(nodeNew);
                    return;
                }
                else
                {
                    AddPrivate(nodeTree.right, nodeNew);
                }
            }
        }

        public bool Remove(NodeGraphDTO node)
        {
            if (root == null)
            {
                return false;
            }
            NodeTree currentNode = root;
            NodeTree previousNode = null;
            bool left = true;
            while (currentNode != null)
            {
                if (currentNode.node.Equals(node))
                {
                    NodeTree n = null, rightNode;
                    if (previousNode == null)
                    {
                        rightNode = root.right;
                        if (root.left == null)
                            root = root.right;
                        else
                        {
                            root = root.left;
                            n = root;
                        }
                    }
                    else if (left)
                    {
                        rightNode = currentNode.right;
                        if (currentNode.left == null)
                            previousNode.left = currentNode.right;
                        else
                        {
                            previousNode.left = currentNode.left;
                            n = previousNode.left;
                        }
                    }
                    else
                    {
                        rightNode = currentNode.right;
                        if (currentNode.left == null)
                            previousNode.right = currentNode.right;
                        else
                        {
                            previousNode.right = currentNode.left;
                            n = previousNode.right;
                        }
                    }

                    if (n != null && rightNode != null)
                    {
                        while (n.right != null)
                        {
                            n = n.right;
                        }
                        n.right = rightNode;
                    }
                    return true;
                }
                else if ((comparer == 0 && node.Node.IdNode <= currentNode.node.Node.IdNode)
                || (comparer == 1 && node.CurrentDistance <= currentNode.node.CurrentDistance)
                || (comparer == 2 && node.FScore <= currentNode.node.FScore)
                || (comparer == 3 && node.CurrentDistanceR <= currentNode.node.CurrentDistanceR))
                {
                    left = true;
                    previousNode = currentNode;
                    currentNode = currentNode.left;
                }
                else
                {
                    left = false;
                    previousNode = currentNode;
                    currentNode = currentNode.right;
                }
            }
            return false;
        }

        public NodeGraphDTO GetMin()
        {
            if (root == null)
            {
                return null;
            }
            NodeTree currentNode = root;
            NodeTree previousNode = null;
            while (true)
            {
                if (currentNode.left != null)
                {
                    previousNode = currentNode;
                    currentNode = currentNode.left;
                }
                else if (previousNode == null)
                {
                    var rootMin = root;
                    root = root.right;
                    return rootMin.node;
                }
                else
                {
                    previousNode.left = currentNode.right;
                    return currentNode.node;
                }
            }
        }

        public MultiLabelMarkQueue GetMinMultiLabel()
        {
            if (root == null)
            {
                return null;
            }
            NodeTree currentNode = root;
            NodeTree previousNode = null;
            while (true)
            {
                if (currentNode.left != null)
                {
                    previousNode = currentNode;
                    currentNode = currentNode.left;
                }
                else if (previousNode == null)
                {
                    var rootMin = root;
                    root = root.right;
                    return rootMin.nodeMultiLabel;
                }
                else
                {
                    previousNode.left = currentNode.right;
                    return currentNode.nodeMultiLabel;
                }
            }
        }

        public NodeGraphDTO Find(Node node)
        {
            if (root == null)
            {
                return null;
            }
            NodeTree currentNode = root;
            while (true)
            {
                if (currentNode.node.Node.Equals(node))
                {
                    return currentNode.node;
                }
                if (node.IdNode <= currentNode.node.Node.IdNode)
                {
                    if (currentNode.left != null)
                    {
                        currentNode = currentNode.left;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (currentNode.right != null)
                    {
                        currentNode = currentNode.right;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private class NodeTree
        {
            public NodeTree(NodeGraphDTO node)
            {
                this.node = node;
            }

            public NodeTree(MultiLabelMarkQueue nodeMultiLabel)
            {
                this.nodeMultiLabel = nodeMultiLabel;
            }

            public MultiLabelMarkQueue nodeMultiLabel;
            public NodeGraphDTO node;
            public NodeTree left;
            public NodeTree right;
        }
    }
}
