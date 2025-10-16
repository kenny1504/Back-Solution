export interface Cuenta {
  id?: number;
  clienteId: number;
  numero: string;
  tipo: 'ahorros' | 'corriente';
  saldo: number;
}
