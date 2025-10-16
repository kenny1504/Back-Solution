import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MovimientosRoutingModule } from './movimientos-routing.module';
import { MovimientosFormComponent } from './pages/movimientos-form/movimientos-form.component';
import {MovimientosListComponent} from './pages/movimientos-list/movimientos-list.component';

@NgModule({
  imports: [CommonModule, FormsModule, MovimientosRoutingModule, MovimientosListComponent, MovimientosFormComponent]
})
export class MovimientosModule {}
