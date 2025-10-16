import {Component, inject} from '@angular/core';
import {DatePipe, DecimalPipe, NgForOf} from '@angular/common';
import {ApiService} from '../../../../core/services/api.service';
import {Movimiento} from '../../../../shared/models/movimiento';
import {RouterLink} from "@angular/router";

@Component({
  selector: 'app-movimientos-list',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    NgForOf,
    RouterLink
  ],
  templateUrl: './movimientos-list.component.html'
})
export class MovimientosListComponent {
  private api = inject(ApiService);
  data: Movimiento[] = [];
  ngOnInit(){ this.api.getAll<Movimiento>(this.api.movimientos).subscribe(r => this.data = r); }
  remove(m: Movimiento){
    if(!m.id) return;
    if(confirm('¿Eliminar movimiento?')){
      this.api.delete(this.api.movimientos, m.id).subscribe(() => this.data = this.data.filter(x => x.id !== m.id));
    }
  }
}
