import {Component, inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {ApiService} from '@app/core/services/api.service';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {Cuenta} from '@app/shared/models/cuenta';
import {Cliente} from '@app/shared/models/cliente';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-cuentas-form',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    NgForOf
  ],
  templateUrl: './cuentas-form.component.html'
})
export class CuentasFormComponent {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  id?: number;
  model: Cuenta = { clienteId:0 , clienteIdFk: 0, cliente: '', numero: '', tipo: 1, saldoInicial: 0, activa: true};
  clientes: Cliente[] = [];


  ngOnInit(){
    const idParam = this.route.snapshot.paramMap.get('id');
    if(idParam){
      this.id = Number(idParam);
      this.api.getById<Cuenta>(this.api.cuentas, this.id).subscribe(r => this.model = r);
    }

    this.api.getAll<Cliente>(this.api.clientes).subscribe((data) => {
      this.clientes = data;
    });

  }
  save(){
    if(this.id){
      this.api.update(this.api.cuentas, this.id, this.model).subscribe(() => this.router.navigate(['/cuentas']));
    } else {
      this.model.clienteId = this.model.clienteIdFk;
      this.api.create(this.api.cuentas, this.model).subscribe(() => this.router.navigate(['/cuentas']));
    }
  }
}
