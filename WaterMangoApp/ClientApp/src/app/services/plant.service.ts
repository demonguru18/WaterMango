import {Injectable} from '@angular/core';
import {Plant} from "../interfaces/plant";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {shareReplay} from "rxjs/operators";

@Injectable({

  providedIn: 'root'
})
export class PlantService {
  private getPlantList : string = "/api/v1/plant/GetListOfPlants";
  private plants$: Observable<Plant[]>;

  constructor(private http: HttpClient)
  {

  }

  getListOfPlants()
  {
    if (!this.plants$)
    {
      return this.http.get<any>(this.getPlantList).pipe(shareReplay());
    }
    return this.plants$;
  }
}
