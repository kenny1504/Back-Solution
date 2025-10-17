import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportesRoutingModule } from './reportes-routing.module';
import { ReporteComponent } from './pages/reporte/reporte.component';


@NgModule({
  imports: [CommonModule, FormsModule, ReportesRoutingModule, ReporteComponent]
})
export class ReportesModule {}
