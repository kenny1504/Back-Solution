import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MovimientosFormComponent } from './pages/movimientos-form/movimientos-form.component';
import {MovimientosListComponent} from './pages/movimientos-list/movimientos-list.component';

const routes: Routes = [
  { path: '', component: MovimientosListComponent },
  { path: 'nuevo', component: MovimientosFormComponent },
  { path: ':id/editar', component: MovimientosFormComponent }
];

@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class MovimientosRoutingModule {}
