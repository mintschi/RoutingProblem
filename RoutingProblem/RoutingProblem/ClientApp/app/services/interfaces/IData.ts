import { Injectable } from '@angular/core';
import { Http, HttpModule } from '@angular/http';
import { IRoute } from './IRoute';

export interface IData {
    pocetHranCesty: number;
    pocetNavstivenychHran: number;
    dlzkaCesty: number;
    casVypoctu: number;
    nodes: IRoute[];
}