import { Injectable } from '@angular/core';
import { Http, HttpModule } from '@angular/http';
import { IData } from './IData';

export interface IRoutes {
    route: IData[];
}