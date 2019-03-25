using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public partial class Data
    {

        public decimal IdData { get; set; }
        public String Title { get; set; }
        public double MinLat { get; set; }
        public double MinLon { get; set; }
        public double MaxLat { get; set; }
        public double MaxLon { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<Node> NodeIdDataNavigation { get; set; }
        public virtual ICollection<Edge> EdgeIdDataNavigation { get; set; }
        public virtual ICollection<DisabledMovement> DisabledMovementIdDataNavigation { get; set; }
    }
}
