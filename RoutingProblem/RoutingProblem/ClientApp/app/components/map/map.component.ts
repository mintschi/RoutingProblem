﻿import { MapHttpService } from '../../services/maphttp.service';
import { Component, OnInit } from '@angular/core';
import { Event } from '@angular/router';
import { forEach, last } from '@angular/router/src/utils/collection';
import { Observable } from 'rxjs/Observable';
import { IRoute } from '../../models/IRoute';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { style } from '@angular/animations';
import { IRoutes } from '../../models/IRoutes';
import { INode } from '../../models/INode';
import { IField } from '../../models/IField';
import { Title } from '@angular/platform-browser';
import { assertNotNull } from '@angular/compiler/src/output/output_ast';
import { IStatistics } from '../../models/IStatistics';

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
    public disabled: string = "";
    public routeType: string = "najkratsia";
    public map: any;
    public startPointFeature: any;
    public endPointFeature: any;
    public interactiveRoute: boolean = true;
    public kRoutes: string = "";
    public idData: string = "5";
    public load: boolean = false;
    private source: any;
    private title: string = "";
    private box: any;
    private modify: any;

    constructor(private service: MapHttpService, private titleService: Title) { }

    ngOnInit() {
        this.titleService.setTitle("Problémy trasovania");
        var name: number = 0;

        var raster = new ol.layer.Tile({
            source: new ol.source.OSM(),
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

        var dragBoxInteraction = new ol.interaction.DragBox({
            condition: ol.events.condition.shiftKeyOnly,
        });

        dragBoxInteraction.on('boxend', (e: any) => {
            var format = new ol.format.GeoJSON();
            var geom = e.target.getGeometry();
            this.download = true;
            this.title = "";
            this.box = new ol.Feature({
                geometry: new ol.geom.LineString(geom.getCoordinates()[0]),
            });
            this.map.removeInteraction(this.modify);
            source.addFeature(this.box);
        });

        this.map = new ol.Map({
            controls: ol.control.defaults().extend([new ol.control.ScaleLine]),
            layers: [raster, this.vector],
            interactions: ol.interaction.defaults({
                shiftDragZoom: false,
                altShiftDragRotate: false
            }).extend([dragBoxInteraction]),
            target: 'map',
            view: new ol.View({
            }),
        });

        //this.map.on("change:size", (evt: any) => {
        //    this.map.updateSize();
        //});

        var startLonLat: any;
        var endLonLat: any;

        this.map.on("click", (evt: any) => {
            if (this.download) return;
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

        this.modify = new ol.interaction.Modify({ source: source });
        this.map.addInteraction(this.modify);

        this.modify.on("modifyend", (evt: any) => {
            this.modifyend(true);
        });

        this.loadFields();
    }

    public points: Array<any> = [];
    public featureLineStart: any;
    public featureLineEnd: any;
    public routes: IRoutes = new Object() as IRoutes;
    public route: IRoute = new Object() as IRoute;
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
        this.service.findRoute(this.type, this.disabled, this.routeType, this.startLatLon, this.endLatLon, this.kRoutes)
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

    modifyend(find: boolean = true) {
        if (!this.disableFind && this.routeLayer != null) {
            if (this.interactiveRoute) {
                this.map.removeLayer(this.routeLayer);
                this.featureLineStart = null;
                this.featureLineEnd = null;
                this.idRoute = 0;
                if (find) {
                    this.findRoute();
                } else {
                    this.source.removeFeature(this.startPointFeature);
                    this.source.removeFeature(this.endPointFeature);
                    this.startLatLon = "";
                    this.endLatLon = "";
                    this.route = <IRoute>new Object;
                }
            }
        }
    }

    interactive(evt: any) {
        this.interactiveRoute = evt.target.checked;
        if (this.interactiveRoute == true) {
            this.modifyend(true);
        }
    }

    disabledM(evt: any) {
        this.disabled = evt.target.checked ? 'disabled' : '';
        this.modifyend(true);
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
            this.modifyend(true);
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

    closeDownload() {
        this.download = false;
        this.required = false;
        this.map.addInteraction(this.modify);
        this.source.removeFeature(this.box);
    }

    public download = false;
    public required = false;
    downloadData() {
        try {
            this.title = (<HTMLInputElement>document.getElementById("title")).value;
        } catch (e) {

        }
        if (this.title == "") {
            this.required = true;
            return;
        }
        this.closeDownload();
        this.load = true;
        var minLonLat = ol.proj.transform(this.box.getGeometry().getCoordinates()[1], 'EPSG:3857', 'EPSG:4326');
        var maxLonLat = ol.proj.transform(this.box.getGeometry().getCoordinates()[3], 'EPSG:3857', 'EPSG:4326');
        this.service.downloadData(this.title, minLonLat[1], minLonLat[0], maxLonLat[1], maxLonLat[0])
            .then((response: Array<IField>) => {
                this.fields = response;
                this.selectedField = this.fields[0];
                this.load = false;
                this.modifyend(false);
                this.addExtent();
            });
    }

    setData(evt: any) {
        this.load = true;
        this.service.setData(evt.key)
            .then((response: boolean) => {
                if (response) {
                    this.selectedField = evt;
                    this.load = false;
                    this.modifyend(false);
                    this.addExtent();
                }
            });
    }

    public fields: Array<IField> = [];
    public selectedField: any;
    loadFields() {
        this.service.loadFields()
            .then((response: Array<IField>) => {
                this.fields = response;
                if (this.fields.length != 0) {
                    this.selectedField = this.fields[0];
                    this.addExtent();
                }
            });
    }

    public statistic: IStatistics = new Object() as IStatistics;
    statistics() {
        this.service.statistics()
            .then((response: IStatistics) => {
                this.statistic = response;
            });
    }

    openForm() {
        this.show = true;
    }

    closeForm() {
        this.show = false;
    }

    //extent
    private extentLayer: any;
    private zoomToExtentControl: any;
    addExtent() {
        this.map.removeLayer(this.extentLayer);
        var source = new ol.source.Vector({ wrapX: false });
        this.extentLayer = new ol.layer.Vector({
            source: source,
            style: (feature: any) => {
                return this.getStyle(feature.get('type'));
            }
        });

        var feature = new ol.Feature({
            geometry: new ol.geom.LineString([
                ol.proj.fromLonLat([+this.selectedField.value[4], +this.selectedField.value[1]]),
                ol.proj.fromLonLat([+this.selectedField.value[4], +this.selectedField.value[3]]),
                ol.proj.fromLonLat([+this.selectedField.value[2], +this.selectedField.value[3]]),
                ol.proj.fromLonLat([+this.selectedField.value[2], +this.selectedField.value[1]]),
                ol.proj.fromLonLat([+this.selectedField.value[4], +this.selectedField.value[1]])]),
            type: 'extent'
        });
        source.addFeature(feature);
        this.map.addLayer(this.extentLayer);

        this.map.removeControl(this.zoomToExtentControl);
        var extent = ol.proj.transformExtent([+this.selectedField.value[2], +this.selectedField.value[1],
        +this.selectedField.value[4], +this.selectedField.value[3]], 'EPSG:4326', 'EPSG:3857');
        this.zoomToExtentControl = new ol.control.ZoomToExtent({
            extent: extent
        });
        this.map.addControl(this.zoomToExtentControl);
        this.map.getView().fit(extent, this.map.getSize());
    }

    //styles
    getStyle(style: any) {
        if (style === 'line') {
            return new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'rgba(122, 122, 122, 0.8)',
                    width: 6,
                    lineDash: [1.5, 10]
                })
            });
        } else if (style === 'extent') {
            return new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 200, 0.8)',
                    width: 6,
                    lineDash: [1.5, 10]
                })
            });
        }else if (style === 'start') {
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
                    color: 'rgba(77, 77, 77, 0.8)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'white',
                    width: 2
                })
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(77, 77, 77, 0.8)',
                width: 6
            })
        });
    }

    //download data
    dragElement() {
        var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
        var elmnt: any;
        try {
            elmnt = <HTMLDivElement>document.getElementById("downloadform");
        } catch (e) {

        }
        if (document.getElementById("downloadheader")) {
            (<HTMLDivElement>document.getElementById("downloadheader")).onmousedown = dragMouseDown;
        } else {
            elmnt.onmousedown = dragMouseDown;
        }

        function dragMouseDown(e: any) {
            e = e || window.event;
            e.preventDefault();
            pos3 = e.clientX;
            pos4 = e.clientY;
            document.onmouseup = closeDragElement;
            document.onmousemove = elementDrag;
        }

        function elementDrag(e: any) {
            e = e || window.event;
            e.preventDefault();
            pos1 = pos3 - e.clientX;
            pos2 = pos4 - e.clientY;
            pos3 = e.clientX;
            pos4 = e.clientY;

            if (elmnt.offsetTop >= 0 && elmnt.offsetLeft >= 0 && elmnt.offsetTop + elmnt.offsetHeight <= window.innerHeight
                && elmnt.offsetLeft + elmnt.offsetWidth <= window.innerWidth) {
                elmnt.style.top = (elmnt.offsetTop - pos2) + "px";
                elmnt.style.left = (elmnt.offsetLeft - pos1) + "px";
            }
            if (elmnt.offsetTop < 0) {
                elmnt.style.top = 0 + "px";
            } else if (elmnt.offsetTop + elmnt.offsetHeight > window.innerHeight) {
                elmnt.style.top = window.innerHeight - elmnt.offsetHeight + "px";
            }
            if (elmnt.offsetLeft < 0) {
                elmnt.style.left = 0 + "px";
            } else if (elmnt.offsetLeft + elmnt.offsetWidth > window.innerWidth) {
                elmnt.style.left = window.innerWidth - elmnt.offsetWidth + "px";
            }
        }

        function closeDragElement() {
            document.onmouseup = nothing;
            document.onmousemove = nothing;
        }

        function nothing() {

        }
    }
}