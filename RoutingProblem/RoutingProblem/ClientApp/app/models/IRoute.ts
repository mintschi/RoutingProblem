import { Injectable } from '@angular/core';
import { Http, HttpModule } from '@angular/http';
import { INode } from './INode';

export interface IRoute {
    pocetHranCesty: number;
    pocetSpracovanychVrcholov: number;
    dlzkaCesty: number;
    casVypoctu: number;
    nodes: INode[];
}