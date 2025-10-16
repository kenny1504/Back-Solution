import {Component, inject} from '@angular/core';
import {DecimalPipe, NgForOf} from '@angular/common';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {ApiService} from '../../../../core/services/api.service';
import {Cuenta} from '../../../../shared/models/cuenta';

@Component({
  selector: 'app-cuentas-list',
  standalone: true,
  imports: [
    DecimalPipe,
    NgForOf,
    RouterLink
  ],
  templateUrl: './cuentas-list.component.html'
})
export class CuentasListComponent {
  private api = inject(ApiService);
  data: Cuenta[] = [];
  ngOnInit(){ this.api.getAll<Cuenta>(this.api.cuentas).subscribe(r => this.data = r); }
  remove(c: Cuenta){
    if(!c.id) return;
    if(confirm('¿Eliminar cuenta?')){
      this.api.delete(this.api.cuentas, c.id).subscribe(() => this.data = this.data.filter(x => x.id !== c.id));
    }
  }
}
