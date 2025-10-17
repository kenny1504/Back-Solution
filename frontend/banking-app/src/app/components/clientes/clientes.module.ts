import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientesRoutingModule } from './clientes-routing.module';
import { ClientesListComponent } from './pages/clientes-list/clientes-list.component';
import { ClientesFormComponent } from './pages/clientes-form/clientes-form.component';

@NgModule({
  imports: [CommonModule, FormsModule, ClientesRoutingModule, ClientesListComponent, ClientesFormComponent]
})
export class ClientesModule {}
