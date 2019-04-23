using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public class MultiLabelMark
    {
        public MultiLabelMark(int k, double t, NodeGraphDTO x, int xk)
        {
            K = k;
            T = t;
            X = x;
            Xk = xk;
        }
        public int K { get; set; }
        public double T { get; set; }
        public NodeGraphDTO X { get; set; }
        public int Xk { get; set; }
    }

    public class MultiLabelMarkQueue
    {
        public MultiLabelMarkQueue(NodeGraphDTO w, double t, NodeGraphDTO x, int xk)
        {
            W = w;
            T = t;
            X = x;
            Xk = xk;
        }
        public NodeGraphDTO W { get; set; }
        public double T { get; set; }
        public NodeGraphDTO X { get; set; }
        public int Xk { get; set; }
    }
}
