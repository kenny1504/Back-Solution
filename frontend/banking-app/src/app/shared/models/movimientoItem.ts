export interface MovimientoItem {
  fecha: string;
  tipo: string;
  movimiento: number;
  saldoDisponible: number;
  descripcion?: string | null;
}
