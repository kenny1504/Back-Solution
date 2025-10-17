import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CuentasRoutingModule } from './cuentas-routing.module';
import {CuentasListComponent} from './pages/cuentas-list/cuentas-list.component';
import {CuentasFormComponent} from './pages/cuentas-form/cuentas-form.component';

@NgModule({
  imports: [CommonModule, FormsModule, CuentasRoutingModule, CuentasListComponent, CuentasFormComponent]
})
export class CuentasModule {}
