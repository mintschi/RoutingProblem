using RoutingProblem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace RoutingProblem.Services.Data
{
    public class Utils
    {
        private const double RADIUS_EARTH_IN_METERS = 6369628.75;
        public static int PocetSpracovanychVrcholov { get; set; }
        public static long PocetVrcholov { get; set; }
        public static long GraphMemory { get; set; }
        public static long DisabledGraphMemory { get; set; }
        public static TimeSpan GraphTime { get; set; }
        public static TimeSpan DisabledGraphTime { get; set; }

        public static double Distance(double startLat, double startLon, double endLat, double endLon)
        {
            double dLat = ConvertDegreesToRadians(Math.Abs(endLat - startLat));
            double dLng = ConvertDegreesToRadians(Math.Abs(endLon - startLon));
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(ConvertDegreesToRadians(startLat)) * Math.Cos(ConvertDegreesToRadians(endLat))
                    * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return (RADIUS_EARTH_IN_METERS * c);
        }

        public static double Distance(Node startNode, Node endNode)
        {
            double dLat = ConvertDegreesToRadians(endNode.Lat - startNode.Lat);
            double dLng = ConvertDegreesToRadians(endNode.Lon - startNode.Lon);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(ConvertDegreesToRadians(startNode.Lat)) * Math.Cos(ConvertDegreesToRadians(endNode.Lat))
                    * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (RADIUS_EARTH_IN_METERS * c);
        }

        private static double ConvertDegreesToRadians(double degrees)
        {
            double radians = degrees / 180 * Math.PI;
            return (radians);
        }
    }
}
