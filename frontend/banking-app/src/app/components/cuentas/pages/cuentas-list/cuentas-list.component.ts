import {Component, inject} from '@angular/core';
import {DecimalPipe, NgForOf, NgIf} from '@angular/common';
import {RouterLink} from '@angular/router';
import {ApiService} from '@app/core/services/api.service';
import {cuentaList} from '@app/shared/models/cuentaList';
import {FormsModule} from '@angular/forms';
import {Cliente} from '@app/shared/models/cliente';

@Component({
  selector: 'app-cuentas-list',
  standalone: true,
  imports: [
    DecimalPipe,
    NgForOf,
    RouterLink,
    NgIf,
    FormsModule
  ],
  templateUrl: './cuentas-list.component.html'
})
export class CuentasListComponent {
  private api = inject(ApiService);
  query = '';
  data: cuentaList[] = [];
  view: cuentaList[] = [];

  ngOnInit() {
    this.api.getAll<cuentaList>(this.api.cuentas).subscribe(r => {
      this.data = r;
      this.view = r;
    });
  }

  applyFilter(){
    const texto = this.query.trim().toLowerCase();
    this.view = !texto ? this.data : this.data.filter(x =>
      (x.numero||'').toLowerCase().includes(texto) ||
      (x.cliente||'').toLowerCase().includes(texto) ||
      String(x.id||'').includes(texto)
    );
  }


  remove(c: cuentaList) {
    if (!c.id) return;
    if (confirm('¿Eliminar cuenta?')) {
      this.api.delete(this.api.cuentas, c.id).subscribe(() => {
        this.data = this.data.filter(x => x.id !== c.id);
        this.applyFilter();
      });
    }
  }
}
