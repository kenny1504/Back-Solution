import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {ClientesListComponent} from './pages/clientes-list/clientes-list.component';
import {ClientesFormComponent} from './pages/clientes-form/clientes-form.component';

const routes: Routes = [
  { path: '', component: ClientesListComponent },
  { path: 'nuevo', component: ClientesFormComponent },
  { path: ':id/editar', component: ClientesFormComponent }
];

@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class ClientesRoutingModule {}
