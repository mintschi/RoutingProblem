using System;
using System.Collections.Generic;

namespace RoutingProblem.Models
{
    public partial class Edge
    {
        public decimal IdEdge { get; set; }
        public decimal StartNode { get; set; }
        public decimal EndNode { get; set; }
        public double DistanceInMeters { get; set; }
        public int? MaxSpeed { get; set; }

        public virtual Node EndNodeNavigation { get; set; }
        public virtual Node StartNodeNavigation { get; set; }
    }
}
