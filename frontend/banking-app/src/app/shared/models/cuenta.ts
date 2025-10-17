export interface Cuenta {
  id?: number;
  clienteIdFk: number;
  clienteId: number;
  cliente: string;
  numero: string;
  tipo: 1 | 2;
  saldoInicial: number;
  activa: boolean;
}
