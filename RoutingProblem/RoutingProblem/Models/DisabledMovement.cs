using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public partial class DisabledMovement
    {
        public decimal IdDisabledMovement { get; set; }
        public decimal IdData { get; set; }
        public decimal StartEdge { get; set; }
        public decimal EndEdge { get; set; }

        public virtual Data IdDataNavigation { get; set; }
        public virtual Edge EndEdgeNavigation { get; set; }
        public virtual Edge StartEdgeNavigation { get; set; }
    }
}
