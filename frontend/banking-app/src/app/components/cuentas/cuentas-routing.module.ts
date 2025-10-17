import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {CuentasListComponent} from './pages/cuentas-list/cuentas-list.component';
import {CuentasFormComponent} from './pages/cuentas-form/cuentas-form.component';

const routes: Routes = [
  { path: '', component: CuentasListComponent },
  { path: 'nuevo', component: CuentasFormComponent },
  { path: ':id/editar', component: CuentasFormComponent }
];

@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class CuentasRoutingModule {}
