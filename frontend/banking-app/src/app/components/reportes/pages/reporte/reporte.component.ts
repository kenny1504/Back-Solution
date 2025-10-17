import {Component, ElementRef, inject, ViewChild} from '@angular/core';
import {DatePipe, DecimalPipe, NgForOf, NgIf} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {ApiService} from '@app/core/services/api.service';
import {Cliente} from '@app/shared/models/cliente';
import {EstadoCuentaResult} from '@app/shared/models/estadoCuentaResult';
import {Row} from '@app/shared/models/row';

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
  rows: Row[] = [];
  total = 0;
  errorMsg = '';
  loading = false;
  lastPayload?: EstadoCuentaResult;
  filtro = {desde: '', hasta: '', clienteId: ''};
  clientes: Cliente[] = [];

  @ViewChild('reporte') reporteRef!: ElementRef<HTMLDivElement>;

  ngOnInit() {
    this.api.getAll<Cliente>(this.api.clientes).subscribe(data => this.clientes = data);
  }

  buscar(): void {
    this.resetFeedback();

    if (this.areFiltersIncomplete()) {
      this.errorMsg = 'Complete los filtros.';
      return;
    }

    const params = this.buildParams();
    this.loading = true;

    this.api.getResumenMovimientos(params).subscribe({
      next: r => this.handleBuscarSuccess(r),
      error: e => this.handleError(e),
      complete: () => this.loading = false
    });
  }

  exportar(): void {
    if (this.lastPayload?.pdfBase64) {
      this.downloadPdf(this.lastPayload.pdfBase64, this.fileNameFromPayload(this.lastPayload));
      return;
    }

    if (this.areFiltersIncomplete()) {
      this.errorMsg = 'Complete los filtros.';
      return;
    }

    const params = this.buildParams();
    this.loading = true;

    this.api.getResumenMovimientos(params).subscribe({
      next: r => this.handleExportarSuccess(r),
      error: e => this.handleError(e),
      complete: () => this.loading = false
    });
  }

  private resetFeedback(): void {
    this.errorMsg = '';
    this.rows = [];
    this.total = 0;
  }

  private areFiltersIncomplete(): boolean {
    return !this.filtro.desde || !this.filtro.hasta || !this.filtro.clienteId;
  }

  private buildParams(): any {
    const {desde, hasta, clienteId} = this.filtro;
    return {...(desde && {desde}), ...(hasta && {hasta}), ...(clienteId && {clienteId})};
  }

  handleBuscarSuccess(r: any): void {
    this.lastPayload = r;
    this.rows = this.flatten(r);
    this.total = this.rows.reduce((acc, x) => acc + x.monto, 0);
  }

  private handleExportarSuccess(r: any): void {
    if (!r.pdfBase64) {
      this.errorMsg = 'El servidor no devolvió el PDF.';
      return;
    }
    this.downloadPdf(r.pdfBase64, this.fileNameFromPayload(r));
  }

  handleError(e: any): void {
    this.errorMsg = e?.error?.message || 'No fue posible procesar la solicitud.';
  }

  private flatten(r: EstadoCuentaResult): Row[] {
    return (r.cuentas ?? []).flatMap(c =>
      (c.movimientos ?? []).map(m => ({
        fecha: new Date(m.fecha),
        cuentaId: c.numeroCuenta ?? c.cuentaId,
        concepto: m.tipo ?? '',
        monto: m.movimiento
      }))
    ).sort((a, b) => a.fecha.getTime() - b.fecha.getTime());
  }

  private downloadPdf(base64: string, fileName: string): void {
    const blob = new Blob([Uint8Array.from(atob(base64), c => c.charCodeAt(0))], {type: 'application/pdf'});
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    URL.revokeObjectURL(url);
  }

  private fileNameFromPayload(r: EstadoCuentaResult): string {
    const {desde = 'desde', hasta = 'hasta', cliente = 'cliente'} = r;
    return `estado_cuenta_${cliente.replace(/\s+/g, '_').toLowerCase()}_${desde.substring(0, 10)}_a_${hasta.substring(0, 10)}.pdf`;
  }
}
