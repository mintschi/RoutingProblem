import { Injectable } from '@angular/core';
import { Http, HttpModule, Response } from '@angular/http';
import { IRoute } from './interfaces/IRoute';
import 'rxjs/add/operator/toPromise';
import { IRoutes } from './interfaces/IRoutes';
import { IField } from './interfaces/IField';

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

    downloadData(title: string, minLat: string, minLon: string, maxLat: string, maxLon: string): Promise<any> {
        return this._http.get('/api/Route/data/' + title + '/' + minLat + '/' + minLon + '/' + maxLat + '/' + maxLon)
            .map((response: Response) => response.json() as boolean)
            .toPromise();
    }

    setData(id: string): Promise<any> {
        return this._http.post('/api/Route/data/' + id, null)
            .map((response: Response) => response.json() as boolean)
            .toPromise();
    }

    loadFields(): Promise<any> {
        return this._http.get('/api/Route/fields')
            .map((response: Response) => response.json() as Array<IField>)
            .toPromise();
    }
}