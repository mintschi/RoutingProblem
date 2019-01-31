import { MapHttpService } from '../../services/maphttpservice.service';
import { Component, OnInit } from '@angular/core';
import { Event } from '@angular/router';
import { forEach } from '@angular/router/src/utils/collection';
import { Observable } from 'rxjs/Observable';
import { IRoute } from '../../services/interfaces/IRoute';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { style } from '@angular/animations';
import { IRoutes } from '../../services/interfaces/IRoutes';
import { IData } from '../../services/interfaces/IData';

declare var ol: any;

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.css'],
    providers: [MapHttpService]
})

export class MapComponent implements OnInit {
    public show: boolean = true;
    public startLatLon: string = "";
    public endLatLon: string = "";
    public disableFind: boolean = true;
    public vector: any;
    public routeLayer: any;
    public type: string = "dijkster";
    public routeType: string = "najkratsia";
    public map: any;
    public startPointFeature: any;
    public endPointFeature: any;
    public interactiveRoute: boolean = true;
    public kRoutes: string = "";
    private source: any;

    constructor(private service: MapHttpService) { }

    ngOnInit() {
        var latitude: number = 49.2122189;
        var longitude: number = 18.7486345;
        var name: number = 0;

        var raster = new ol.layer.Tile({
            source: new ol.source.OSM()
        });

        var source = new ol.source.Vector({ wrapX: false });
        this.source = source;

        this.vector = new ol.layer.Vector({
            source: source,
            zIndex: 1,
            style: (feature: any) => {
                return this.getStyle(feature.get('type'));
            }
        });

        this.map = new ol.Map({
            controls: ol.control.defaults().extend([new ol.control.ScaleLine]),
            layers: [raster, this.vector],
            target: 'map',
            view: new ol.View({
                center: ol.proj.fromLonLat([longitude, latitude]),
                zoom: 13
            }),
        });

        var startLonLat: any;
        var endLonLat: any;

        this.map.on("click", (evt: any) => {
            var style = "";
            if (source.getFeatures().length == 0) {
                style = "start";
            } else if (source.getFeatures().length == 1) {
                style = "end";
            } 
            if (source.getFeatures().length <= 1) {
                var feature = new ol.Feature({
                    geometry: new ol.geom.Point(evt.coordinate),
                    type: style
                })

                if (source.getFeatures().length == 0) {
                    this.startPointFeature = feature;
                    feature.name = 0;
                    startLonLat = ol.proj.transform(feature.getGeometry().getCoordinates(), 'EPSG:3857', 'EPSG:4326');
                    this.startLatLon = startLonLat[1] + "," + startLonLat[0]
                } else {
                    this.endPointFeature = feature;
                    feature.name = 1;
                    endLonLat = ol.proj.transform(feature.getGeometry().getCoordinates(), 'EPSG:3857', 'EPSG:4326');
                    this.endLatLon = endLonLat[1] + "," + endLonLat[0]
                    this.disableFind = false;
                    if (this.interactiveRoute) {
                        this.findRoute();
                    }
                }

                feature.on('change', (evt: any) => {
                    if (evt.target.name == 0) {
                        startLonLat = ol.proj.transform(evt.target.getGeometry().getCoordinates(), 'EPSG:3857', 'EPSG:4326');
                        this.startLatLon = startLonLat[1] + "," + startLonLat[0];
                        if (this.featureLineStart != null)
                            this.featureLineStart.getGeometry().setCoordinates([evt.target.getGeometry().getCoordinates(),
                                                                                this.featureLineStart.getGeometry().getCoordinates()[1]]);
                    } else {
                        endLonLat = ol.proj.transform(evt.target.getGeometry().getCoordinates(), 'EPSG:3857', 'EPSG:4326');
                        this.endLatLon = endLonLat[1] + "," + endLonLat[0];
                        if (this.featureLineEnd != null)
                            this.featureLineEnd.getGeometry().setCoordinates([evt.target.getGeometry().getCoordinates(),
                                                                              this.featureLineEnd.getGeometry().getCoordinates()[1]]);
                    }
                });

                source.addFeature(feature);
            }
        });

        var modify = new ol.interaction.Modify({ source: source });
        this.map.addInteraction(modify);

        modify.on("modifyend", (evt: any) => {
            this.modifyend();
        });
    }

    public points: Array<any> = [];
    public featureLineStart: any;
    public featureLineEnd: any;
    public routes: IRoutes = new Object() as IRoutes;
    public route: IData = new Object() as IData;
    public idRoute: any = 0;
    public routesCount: any = -1;
    findRoute() {
        try {
            this.kRoutes = (<HTMLInputElement>document.getElementById("k")).value;
        } catch(e) {

        }
        this.map.removeLayer(this.routeLayer);
        this.points = new Array();
        var points: Array<any> = new Array();
        var source = new ol.source.Vector({ wrapX: false });
        this.routeLayer = new ol.layer.Vector({
            source: source,
            style: (feature: any) => {
                return this.getStyle(feature.get('type'));
            }
        });
        this.service.findRoute(this.type, this.routeType, this.startLatLon, this.endLatLon, this.kRoutes)
            .then((route: IRoutes) => {
                this.route = route.route[this.idRoute];
                this.points = this.route.nodes;
                this.routes = route;
                this.routesCount = route.route.length - 1;
            })
            .then(() => {
                this.points.forEach((value: any) => {
                    var point = ol.proj.transform([value.lon, value.lat], 'EPSG:4326', 'EPSG:3857');
                    points.push(point);
                });

                this.featureLineStart = new ol.Feature({
                    geometry: new ol.geom.LineString([this.startPointFeature.getGeometry().getCoordinates(), points[points.length - 1]]),
                    type: 'line'
                });
                source.addFeature(this.featureLineStart);

                this.featureLineEnd = new ol.Feature({
                    geometry: new ol.geom.LineString([this.endPointFeature.getGeometry().getCoordinates(), points[0]]),
                    type: 'line'
                });
                source.addFeature(this.featureLineEnd);

                source.addFeature(new ol.Feature({ geometry: new ol.geom.LineString(points) }));
                source.addFeature(new ol.Feature({ geometry: new ol.geom.Point(points[0]) }));
                source.addFeature(new ol.Feature({ geometry: new ol.geom.Point(points[points.length - 1]) }));
                this.map.addLayer(this.routeLayer);
            })
    }

    modifyend() {
        if (!this.disableFind && this.routeLayer != null) {
            if (this.interactiveRoute) {
                this.map.removeLayer(this.routeLayer);
                this.featureLineStart = null;
                this.featureLineEnd = null;
                this.findRoute();
            }
        }
    }

    interactive(evt: any) {
        this.interactiveRoute = evt.target.checked;
        if (this.interactiveRoute == true) {
            this.modifyend();
        }
    }

    changeStartEnd() {
        if (this.source.getFeatures().length > 1) {
            let s = this.startLatLon.split(",");
            let e = this.endLatLon.split(",");
            let startLatLon = this.startLatLon;
            this.startLatLon = this.endLatLon;
            this.endLatLon = startLatLon;
            this.source.getFeatures()[0].getGeometry().setCoordinates(ol.proj.transform([+e[1], +e[0]], 'EPSG:4326', 'EPSG:3857'));
            this.source.getFeatures()[0].getGeometry().setCoordinates(ol.proj.transform([+s[1], +s[0]], 'EPSG:4326', 'EPSG:3857'));
            this.modifyend();
        }
    }

    betterRoute() {
        if (this.idRoute > 0) {
            this.idRoute--;
            this.findRoute();
        }
    }

    worseRoute() {
        if (this.idRoute < this.routes.route.length - 1) {
            this.idRoute++;
            this.findRoute();
        } 
    }

    openForm() {
        this.show = true;
    }

    closeForm() {
        this.show = false;
    }

    getStyle(style: any) {
        if (style === 'line') {
            return new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'rgba(122, 122, 122, 0.8)',
                    width: 6,
                    lineDash: [1.5, 10]
                })
            });
        } else if (style === 'start') {
            return new ol.style.Style({
                image: new ol.style.Icon({
                    anchor: [0.5045, 1],
                    scale: 0.1,
                    src: 'image/mapMarkerStart.png'
                })
            });
        } else if (style === 'end') {
            return new ol.style.Style({
                image: new ol.style.Icon({
                    anchor: [0.5045, 1],
                    scale: 0.1,
                    src: 'image/mapMarkerEnd.png'
                })
            });
        }
        return new ol.style.Style({
            image: new ol.style.Circle({
                radius: 6,
                fill: new ol.style.Fill({
                    color: 'rgba(122, 122, 122, 0.8)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'white',
                    width: 2
                })
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(122, 122, 122, 0.8)',
                width: 6
            })
        });
    }
}