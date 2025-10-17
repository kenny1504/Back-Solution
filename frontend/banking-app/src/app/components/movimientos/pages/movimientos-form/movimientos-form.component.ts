import {Component, inject} from '@angular/core';
import {ApiService} from '../../../../core/services/api.service';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {Movimiento} from '../../../../shared/models/movimiento';
import {FormsModule} from '@angular/forms';
import {Cliente} from '@app/shared/models/cliente';
import {Cuenta} from '@app/shared/models/cuenta';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-movimientos-form',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    NgForOf
  ],
  templateUrl: './movimientos-form.component.html'
})
export class MovimientosFormComponent {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  id?: number;
  model: Movimiento = { cuentaId:0, fecha: new Date().toISOString().substring(0,10), tipo:0, valor:0 };
  cuentas: Cuenta[] = [];


  ngOnInit(){
    const idParam = this.route.snapshot.paramMap.get('id');
    if(idParam){
      this.id = Number(idParam);
      this.api.getById<Movimiento>(this.api.movimientos, this.id).subscribe(r => this.model = r);
    }

    this.api.getAll<Cuenta>(this.api.cuentas).subscribe((data) => {
      this.cuentas = data;
    });

  }
  save(){
    if(this.id){
      this.api.update(this.api.movimientos, this.id, this.model).subscribe(() => this.router.navigate(['/movimientos']));
    } else {
      this.api.create(this.api.movimientos, this.model).subscribe(() => this.router.navigate(['/movimientos']));
    }
  }
}
