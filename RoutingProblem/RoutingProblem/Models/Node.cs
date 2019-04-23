using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoutingProblem.Models
{
    public partial class Node
    {
        public Node()
        {
            EdgeEndNodeNavigation = new HashSet<Edge>();
            EdgeStartNodeNavigation = new HashSet<Edge>();
        }

        public decimal IdNode { get; set; }
        public decimal IdData { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public virtual Data IdDataNavigation { get; set; }
        public virtual ICollection<Edge> EdgeEndNodeNavigation { get; set; }
        public virtual ICollection<Edge> EdgeStartNodeNavigation { get; set; }

        [NotMapped]
        public NodeGraphDTO NodeGraphDTO { get; set; }
    }
}
