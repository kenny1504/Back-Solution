import {Component, inject} from '@angular/core';
import {FormsModule} from "@angular/forms";
import {NgForOf, NgIf} from "@angular/common";
import {RouterLink} from '@angular/router';
import {ApiService} from '../../../../core/services/api.service';
import {Cliente} from '../../../../shared/models/cliente';

@Component({
  selector: 'app-clientes-list',
  standalone: true,
  imports: [
    FormsModule,
    NgForOf,
    NgIf,
    RouterLink
  ],
  templateUrl: './clientes-list.component.html'
})
export class ClientesListComponent {
  private api = inject(ApiService);
  data: Cliente[] = [];
  view: Cliente[] = [];
  query = '';

  ngOnInit() {
    this.api.getAll<Cliente>(this.api.clientes).subscribe(r => {
      this.data = r;
      this.view = r;
    });
  }
  applyFilter(){
    const texto = this.query.trim().toLowerCase();
    this.view = !texto ? this.data : this.data.filter(x =>
      (x.nombre||'').toLowerCase().includes(texto) ||
      (x.identificacion||'').toLowerCase().includes(texto) ||
      (x.telefono||'').toLowerCase().includes(texto) ||
      String(x.id||'').includes(texto)
    );
  }
  remove(c: Cliente){
    if(!c.id) return;
    if(confirm('¿Eliminar cliente?')){
      this.api.delete(this.api.clientes, c.id).subscribe(() => {
        this.data = this.data.filter(x => x.id !== c.id);
        this.applyFilter();
      });
    }
  }
}
