import {Component, ElementRef, inject, ViewChild} from '@angular/core';
import {DatePipe, DecimalPipe, NgForOf, NgIf} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {ApiService} from '../../../../core/services/api.service';
import {exportElementToPdf} from '../../utils/pdf.util';

@Component({
  selector: 'app-reporte',
  standalone: true,
    imports: [
        DatePipe,
        DecimalPipe,
        FormsModule,
        NgForOf,
        NgIf
    ],
  templateUrl: './reporte.component.html'
})
export class ReporteComponent {
  private api = inject(ApiService);
  rows: any[] = [];
  total = 0;
  errorMsg = '';
  filtro: any = { desde: '', hasta: '', cuentaId: '' };

  @ViewChild('reporte') reporteRef!: ElementRef<HTMLDivElement>;

  buscar(){
    this.errorMsg = '';
    const params: any = {};
    if(this.filtro.desde) params.desde = this.filtro.desde;
    if(this.filtro.hasta) params.hasta = this.filtro.hasta;
    if(this.filtro.cuentaId) params.cuentaId = this.filtro.cuentaId;
    this.api.getResumenMovimientos(params).subscribe({
      next: (r: any) => {
        this.rows = r?.items || [];
        this.total = this.rows.reduce((acc: number, x: any) => acc + Number(x.monto || 0), 0);
      },
      error: (e) => this.errorMsg = e?.error?.message || 'No fue posible cargar el reporte.'
    });
  }

  async exportar(){
    if(!this.reporteRef?.nativeElement) return;
    const filename = `reporte_${new Date().toISOString().substring(0,10)}.pdf`;
    await exportElementToPdf(this.reporteRef.nativeElement, filename);
  }
}
