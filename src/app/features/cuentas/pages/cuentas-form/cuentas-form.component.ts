import {Component, inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {ApiService} from '../../../../core/services/api.service';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {Cuenta} from '../../../../shared/models/cuenta';

@Component({
  selector: 'app-cuentas-form',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink
  ],
  templateUrl: './cuentas-form.component.html'
})
export class CuentasFormComponent {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  id?: number;
  model: Cuenta = { clienteId:0, numero:'', tipo:'ahorros', saldo:0 };

  ngOnInit(){
    const idParam = this.route.snapshot.paramMap.get('id');
    if(idParam){
      this.id = Number(idParam);
      this.api.getById<Cuenta>(this.api.cuentas, this.id).subscribe(r => this.model = r);
    }
  }
  save(){
    if(this.id){
      this.api.update(this.api.cuentas, this.id, this.model).subscribe(() => this.router.navigate(['/cuentas']));
    } else {
      this.api.create(this.api.cuentas, this.model).subscribe(() => this.router.navigate(['/cuentas']));
    }
  }
}
