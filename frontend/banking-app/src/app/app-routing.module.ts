import {Routes} from '@angular/router';

export const routes: Routes = [
  {path: '', pathMatch: 'full', redirectTo: 'clientes'},
  {
    path: 'clientes',
    loadChildren: () => import('./features/clientes/clientes.module').then(m => m.ClientesModule)
  },
  {
    path: 'cuentas',
    loadChildren: () => import('./features/cuentas/cuentas.module').then(m => m.CuentasModule)
  },
  {
    path: 'movimientos',
    loadChildren: () => import('./features/movimientos/movimientos.module').then(m => m.MovimientosModule)
  },
  {
    path: 'reportes',
    loadChildren: () => import('./features/reportes/reportes.module').then(m => m.ReportesModule)
  },
  {path: '**', redirectTo: 'clientes'}
];
