using RoutingProblem.Models;
using RoutingProblem.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace RoutingProblem.Data
{
    public class SpracovanieOSMDat
    {
        
        public bool SpracovanieXMLDat(DopravnaSietContext db)
        {
            var xml = new XmlDocument();
            var nodesAll = new Dictionary<decimal, Node>();
            var nodesEdge = new Dictionary<decimal, Node>();
            var edges = new List<Edge>();
            var disabledMovements = new List<DisabledMovement>();
            xml.Load("C:\\Users\\Martin\\Downloads\\BasicOSMParser-master\\BasicOSMParser-master\\OSM_nove.osm");

            XmlNodeList documentNodeList = xml.DocumentElement.SelectNodes("/osm/node");
            XmlNodeList documentWayList = xml.DocumentElement.SelectNodes("/osm/way");

            if (documentNodeList.Count > 0)
            {
                foreach (XmlNode row in documentNodeList)
                {
                    Node node = new Node();

                    node.IdNode = Convert.ToDecimal(row.Attributes["id"].Value);
                    node.Lat = double.Parse(row.Attributes["lat"].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    node.Lon = double.Parse(row.Attributes["lon"].Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                    nodesAll.Add(node.IdNode, node);
                }
            }

            if (documentWayList.Count > 0)
            {
                decimal id_edge = 0;
                decimal id_disabledM = 0;

                foreach (XmlNode row in documentWayList)
                {
                    decimal end_node = -1;
                    bool oneway = false;

                    for (int i = row.ChildNodes.Count - 1; i >= 0; i--)
                    {
                        if (row.ChildNodes[i].Name.Equals("tag") && 
                            row.ChildNodes[i].Attributes["k"].Value.Equals("building") &&
                            row.ChildNodes[i].Attributes["v"].Value.Equals("yes"))
                        {
                            break;
                        }

                        if (row.ChildNodes[i].Name.Equals("tag") &&
                            row.ChildNodes[i].Attributes["k"].Value.Equals("highway") &&
                            (row.ChildNodes[i].Attributes["v"].Value.Equals("pedestrian") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("footway") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("bridleway") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("steps") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("path") ||
                            row.ChildNodes[i].Attributes["v"].Value.Equals("cycleway")))
                        {
                            break;
                        }

                        if (row.ChildNodes[i].Name.Equals("tag") &&
                            row.ChildNodes[i].Attributes["k"].Value.Equals("oneway") &&
                            row.ChildNodes[i].Attributes["v"].Value.Equals("yes"))
                        {
                            oneway = true;
                        }

                        if (row.ChildNodes[i].Name.Equals("nd"))
                        {
                            if (end_node == -1)
                            {
                                end_node = Convert.ToDecimal(row.ChildNodes[i].Attributes["ref"].Value);
                                if (!nodesEdge.ContainsKey(end_node))
                                {
                                    nodesEdge.Add(end_node, nodesAll[end_node]);
                                }
                            }
                            else
                            {
                                Edge edge = new Edge();
                                edge.IdEdge = id_edge++;
                                edge.StartNode = Convert.ToDecimal(row.ChildNodes[i].Attributes["ref"].Value);
                                edge.EndNode = end_node;
                                edge.DistanceInMeters = Utils.Vzdialenost(nodesAll[edge.StartNode], nodesAll[edge.EndNode]);
                                edges.Add(edge);

                                if (!oneway)
                                {
                                    Edge edge1 = new Edge();
                                    edge1.IdEdge = id_edge++;
                                    edge1.StartNode = edge.EndNode;
                                    edge1.EndNode = edge.StartNode;
                                    edge1.DistanceInMeters = edge.DistanceInMeters;
                                    edges.Add(edge1);

                                    disabledMovements.Add(new DisabledMovement()
                                    {
                                        IdDisabledMovement = id_disabledM++,
                                        StartEdge = edge.IdEdge,
                                        EndEdge = edge1.IdEdge
                                    });
                                    disabledMovements.Add(new DisabledMovement()
                                    {
                                        IdDisabledMovement = id_disabledM++,
                                        StartEdge = edge1.IdEdge,
                                        EndEdge = edge.IdEdge
                                    });
                                }

                                end_node = edge.StartNode;
                                if (!nodesEdge.ContainsKey(end_node))
                                {
                                    nodesEdge.Add(end_node, nodesAll[end_node]);
                                }
                            }
                        }
                    }
                }
            }

            NaplnenieDatabazy(nodesEdge, edges, disabledMovements);
            return true;
        }

        private void NaplnenieDatabazy(Dictionary<decimal, Node> nodes, List<Edge> edges, List<DisabledMovement> disabledMovements)
        {
            var dtNode = new DataTable();
            dtNode.Columns.Add("id_node");
            dtNode.Columns.Add("lat");
            dtNode.Columns.Add("lon");

            foreach (KeyValuePair<decimal, Node> n in nodes)
            {
                dtNode.Rows.Add(n.Value.IdNode, n.Value.Lat, n.Value.Lon);
            }

            var dtEdge = new DataTable();
            dtEdge.Columns.Add("id_edge");
            dtEdge.Columns.Add("start_node");
            dtEdge.Columns.Add("end_node");
            dtEdge.Columns.Add("distance_in_meters");

            edges.ForEach(n => dtEdge.Rows.Add(n.IdEdge, n.StartNode, n.EndNode, n.DistanceInMeters));

            var dtDisabledM = new DataTable();
            dtDisabledM.Columns.Add("id_disabled_movement");
            dtDisabledM.Columns.Add("start_edge");
            dtDisabledM.Columns.Add("end_edge");

            disabledMovements.ForEach(n => dtDisabledM.Rows.Add(n.IdDisabledMovement, n.StartEdge, n.EndEdge));

            using (var sqlBulk = new SqlBulkCopy("Server=mainpc\\sqlexpress;Database=DopravnaSiet;Trusted_Connection=True;"))
            {
                sqlBulk.DestinationTableName = "Node";
                sqlBulk.WriteToServer(dtNode);
                sqlBulk.DestinationTableName = "Edge";
                sqlBulk.WriteToServer(dtEdge);
                sqlBulk.DestinationTableName = "DisabledMovement";
                sqlBulk.WriteToServer(dtDisabledM);
            }
        }
    }
}