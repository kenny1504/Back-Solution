export interface Movimiento {
  id?: number;
  cuentaId: number;
  fecha: string; // ISO
  concepto: string;
  monto: number; // positivo/negativo
}
