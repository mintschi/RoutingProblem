import { Injectable } from '@angular/core';
import { Http, HttpModule } from '@angular/http';

export interface IStatistics {
    GraphMemory: any;
    DisabledGraphMemory: any;
    GraphTime: any;
    DisabledGraphTime: any;
}