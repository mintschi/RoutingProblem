﻿import { Injectable } from '@angular/core';
import { Http, HttpModule, Response } from '@angular/http';
import { IRoute } from './interfaces/IRoute';
import 'rxjs/add/operator/toPromise';
import { IRoutes } from './interfaces/IRoutes';

@Injectable()
export class MapHttpService {
    constructor(private _http: Http) { }

    findRoute(type: string, routeType: string, startLatLon: string, endLatLon: string): Promise<any> {
        return this._http.get('/api/Route/' + type + '/' + startLatLon + '/' + endLatLon + '/')
            .map((response: Response) => response.json() as IRoutes)
            .toPromise();
    }
}