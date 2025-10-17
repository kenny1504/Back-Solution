import {Component, inject} from '@angular/core';
import {DatePipe, DecimalPipe, NgForOf} from '@angular/common';
import {ApiService} from '@app/core/services/api.service';
import {RouterLink} from "@angular/router";
import {FormsModule} from '@angular/forms';
import {MovimientoList} from '@app/shared/models/movimientoList';

@Component({
  selector: 'app-movimientos-list',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    NgForOf,
    RouterLink,
    FormsModule
  ],
  templateUrl: './movimientos-list.component.html'
})
export class MovimientosListComponent {
  private api = inject(ApiService);
  data: MovimientoList[] = [];
  view: MovimientoList[] = [];
  query = '';

  ngOnInit() {
    this.api.getAll<MovimientoList>(this.api.movimientos).subscribe(r => {
      this.data = r;
      this.view = r;
    });
  }

  remove(m: MovimientoList) {
    if (!m.id) return;
    if (confirm('¿Eliminar movimiento?')) {
      this.api.delete(this.api.movimientos, m.id).subscribe(() => {
        this.data = this.data.filter(x => x.id !== m.id);
        this.applyFilter();
      });
    }
  }

  applyFilter(){
    const texto = this.query.trim().toLowerCase();
    this.view = !texto ? this.data : this.data.filter(x =>
      (x.fecha||'').toLowerCase().includes(texto) ||
      (x.fecha||'').toLowerCase().includes(texto) ||
      String(x.id||'').includes(texto)
    );
  }
}
