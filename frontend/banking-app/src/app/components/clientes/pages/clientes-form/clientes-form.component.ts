import {Component, inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {ApiService} from '@app/core/services/api.service';
import {Cliente} from '@app/shared/models/cliente';

@Component({
  selector: 'app-clientes-form',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink
  ],
  templateUrl: './clientes-form.component.html'
})
export class ClientesFormComponent {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  id?: number;
  model: Cliente = {
    nombre: '',
    identificacion: '',
    telefono: '',
    clienteId: '',
    edad: 0,
    direccion: '',
    genero: '',
    Contrasena: '',
    activo: true
  };

  ngOnInit() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.id = Number(idParam);
      this.api.getById<Cliente>(this.api.clientes, this.id).subscribe(r => this.model = r);
    }
  }

  save() {
    if (this.id) {
      this.api.update(this.api.clientes, this.id, this.model).subscribe(() => this.router.navigate(['/clientes']));
    } else {
      this.api.create(this.api.clientes, this.model).subscribe(() => this.router.navigate(['/clientes']));
    }
  }

  isFormValid(): boolean {
    return !!this.model.nombre &&
      !!this.model.identificacion &&
      !!this.model.telefono &&
      !!this.model.direccion &&
      !!this.model.edad &&
      !!this.model.genero &&
      !!this.model.clienteId
  }
}
