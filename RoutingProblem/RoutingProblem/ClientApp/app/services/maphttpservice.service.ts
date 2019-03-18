import { Injectable } from '@angular/core';
import { Http, HttpModule, Response } from '@angular/http';
import { IRoute } from './interfaces/IRoute';
import 'rxjs/add/operator/toPromise';
import { IRoutes } from './interfaces/IRoutes';

@Injectable()
export class MapHttpService {
    constructor(private _http: Http) { }

    findRoute(type: string, disabled: string, routeType: string, startLatLon: string, endLatLon: string, kRoutes: string): Promise<any> {
        if (type === "multilabel") {
            if (disabled === "disabled") {
                return this._http.get('/api/Route/' + type + '/' + disabled + '/' + startLatLon + '/' + endLatLon + '/' + kRoutes + '/')
                    .map((response: Response) => response.json() as IRoutes)
                    .toPromise();
            } else {
                return this._http.get('/api/Route/' + type + '/' + startLatLon + '/' + endLatLon + '/' + kRoutes + '/')
                    .map((response: Response) => response.json() as IRoutes)
                    .toPromise();
            }
        } else if (disabled === "disabled") {
            return this._http.get('/api/Route/' + type + '/' + disabled + '/' + startLatLon + '/' + endLatLon + '/')
                .map((response: Response) => response.json() as IRoutes)
                .toPromise();
        } else {
            return this._http.get('/api/Route/' + type + '/' + startLatLon + '/' + endLatLon + '/')
                .map((response: Response) => response.json() as IRoutes)
                .toPromise();
        }
    }
}