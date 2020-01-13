import { Component, OnInit } from '@angular/core';
import {Plant} from "../interfaces/plant";
import {Observable} from "rxjs";
import {PlantService} from "../services/plant.service";
import * as signalR from '@aspnet/signalr';
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-cards',
  templateUrl: './cards.component.html',
  styleUrls: ['./cards.component.css']
})
export class CardsComponent implements OnInit {

  plants$ : Observable<Plant[]>;
  plants : Plant[] = [];

  constructor(private plantService: PlantService, private toastr: ToastrService) { }

  ngOnInit() {
    this.loadPlantList();

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/quartzHub")
      .build();

      connection.start().then(function () {
        console.log('SignalR Connected!');
      }).catch(function (err) {
        return console.error(err.toString());
      });

    connection.on("JobEndMessage", (message, message2) => {
      this.toastr.info(message2);
      this.loadPlantList();

    });
  }

  loadPlantList()
  {
    this.plants$ = this.plantService.getListOfPlants();
    this.plants$.subscribe(plantList => {
      this.plants = plantList;
    })
  }
}
