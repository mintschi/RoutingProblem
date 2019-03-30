import { Injectable } from '@angular/core';
import { Http, HttpModule } from '@angular/http';
import { IRoute } from './IRoute';

export interface IRoutes {
    route: IRoute[];
}