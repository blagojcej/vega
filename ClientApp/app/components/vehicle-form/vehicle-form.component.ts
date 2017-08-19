import * as _ from 'underscore';
import { Vehicle } from './../../models/vehicle';
import { SaveVehicle } from '../../models/vehicle';
import { Observable } from 'rxjs/Observable';
import { ActivatedRoute, Router } from '@angular/router';
import { VehicleService } from '../../services/vehicle.service';
import { Component, OnInit } from '@angular/core';
import { ToastyService } from "ng2-toasty";
import 'rxjs/add/Observable/forkJoin';

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit {
  makes: any[];
  models: any[];
  features: any[];
  vehicle: SaveVehicle = {
    id: 0,
    makeId: 0,
    modelId: 0,
    isRegistered: false,
    features: [],
    contact: {
      name: '',
      email: '',
      phone:''
    }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private vehicleService: VehicleService,
    private toastyService: ToastyService) {
    route.params.subscribe(p => {
      // + to convert to a number
      this.vehicle.id = +p['id'];
    });
  }

  ngOnInit() {
    var sources = [
      this.vehicleService.getMakes(),
      this.vehicleService.getFeatures(),
    ];

    if (this.vehicle.id) {
      sources.push(this.vehicleService.getVehicle(this.vehicle.id));
    }

    //Send parallel requests
    Observable.forkJoin(sources).subscribe(data => {
      this.makes = data[0];
      this.features = data[1];
      if (this.vehicle.id) {
        this.setVehicle(data[2]);
        this.populateModels();
      }
    }, err => {
      if (err.status == 404) {
        this.router.navigate(['/home']);
      }
    });


    //   .subscribe(v => {
    //     this.vehicle=v;
    //   }, 
    //   //check for error if the page is firt time loaded throuh the address bar
    //   err => {
    //     if(err.status==404){
    //       this.router.navigate(['/']);
    //     }
    //   }
    // );


    // .subscribe(makes => {
    //   this.makes = makes;
    //   console.log("MAKES", this.makes);
    // });


    // .subscribe(features => this.features = features);
  }

  private setVehicle(v: Vehicle){
    this.vehicle.id = v.id;
    this.vehicle.makeId = v.make.id;
    this.vehicle.modelId = v.model.id;
    this.vehicle.isRegistered=v.isRegistered;
    this.vehicle.contact=v.contact;
    this.vehicle.features=_.pluck(v.features, 'id');
  }

  onMakeChange() {
    console.log("VEHICLE", this.vehicle); 
    this.populateModels();   
    delete this.vehicle.modelId;
  }

  private populateModels(){
    var selectedMake = this.makes.find(m => m.id == this.vehicle.makeId);
    this.models = selectedMake ? selectedMake.models : [];
  }

  onFeatureToggle(featureId, $event) {
    if ($event.target.checked) {
      this.vehicle.features.push(featureId);
    }
    else {
      var index = this.vehicle.features.indexOf(featureId);
      this.vehicle.features.splice(index, 1);
    }
  }

  submit() {
    if(this.vehicle.id){
      this.vehicleService.update(this.vehicle)
      .subscribe( x=> {
        this.toastyService.success({
          title: 'Success',
          msg: 'The vehicle was successfullt updated.' ,
          theme: 'bootstrap',
          showClose: true,
          timeout: 5000
        });
      });
    }
    else{
    this.vehicleService.create(this.vehicle)
      .subscribe(x => console.log(x));
    // .subscribe(
    //   x => console.log(x),
    //   err => {
    //             this.toastySerice.error({
    //               title: 'Error',
    //               msg: 'An unexpected error happened,',
    //               theme: 'bootstrap',
    //               showClose: true,
    //               timeout: 5000
    //             });
    //   }
    // );
    }
  }

  delete(){
    if(confirm("Are you sure?")){
      this.vehicleService.delete(this.vehicle.id)
      .subscribe(x=>{
        this.router.navigate(['/home']);
      });
    };
  }
}
