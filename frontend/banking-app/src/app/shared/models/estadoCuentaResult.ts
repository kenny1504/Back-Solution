// @ts-ignore
import {CuentaReporte} from '@app/shared/models/cuentaReporte';


export interface EstadoCuentaResult {
  clienteId: number;
  cliente: string;
  desde: string;
  hasta: string;
  cuentas: CuentaReporte[];
  pdfBase64?: string | null;
}
