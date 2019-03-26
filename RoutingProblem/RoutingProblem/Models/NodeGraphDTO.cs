using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingProblem.Models
{
    public class NodeGraphDTO
    {
        public NodeGraphDTO()
        {
            EdgeNavigation = new HashSet<Edge>();
            EdgeNavigationR = new HashSet<Edge>();
            NeighborNodeNavigation = new Dictionary<NodeGraphDTO, double>();
            NeighborNodeNavigationR = new Dictionary<NodeGraphDTO, double>();
            MultiLabelMark = new List<MultiLabelMark>();
        }

        public Node Node { get; set; }
        public NodeGraphDTO PreviousNode { get; set; }
        public double CurrentDistance { get; set; }

        //astar
        public double EstimateDistanceToEnd { get; set; }
        public double FScore { get; set; }

        //bothside dijkster
        public NodeGraphDTO PreviousNodeR { get; set; }
        public double CurrentDistanceR { get; set; }

        //multi label
        public List<MultiLabelMark> MultiLabelMark { get; set; }

        public ICollection<Edge> EdgeNavigation { get; set; }
        public ICollection<Edge> EdgeNavigationR { get; set; }
        public Dictionary<NodeGraphDTO, double> NeighborNodeNavigation { get; set; }
        public Dictionary<NodeGraphDTO, double> NeighborNodeNavigationR { get; set; }
    }
}
