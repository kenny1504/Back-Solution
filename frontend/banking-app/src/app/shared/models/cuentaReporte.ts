// @ts-ignore
import {MovimientoItem} from '@app/shared/models/movimientoItem';

export interface CuentaReporte  {
  cuentaId: number;
  numeroCuenta: string;
  tipoCuenta: string;
  saldoActual: number;
  totalCreditos: number;
  totalDebitos: number;
  movimientos: MovimientoItem[];
}
