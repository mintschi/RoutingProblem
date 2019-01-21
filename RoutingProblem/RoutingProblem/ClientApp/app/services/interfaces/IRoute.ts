import { Injectable } from '@angular/core';
import { Http, HttpModule } from '@angular/http';

export interface IRoute {
    Lat: number;
    Lon: number;
}