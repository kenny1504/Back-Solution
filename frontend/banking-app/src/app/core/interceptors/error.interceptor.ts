import { HttpInterceptorFn } from '@angular/common/http';
import { tap, catchError, throwError } from 'rxjs';

const createToast = (message: string, isSuccess: boolean = false) => {
  const toast = document.createElement('div');
  toast.textContent = message;
  Object.assign(toast.style, {
    position: 'fixed',
    bottom: '20px',
    left: '50%',
    transform: 'translateX(-50%)',
    backgroundColor: isSuccess ? '#4caf50' : '#f44336',
    color: 'white',
    padding: '10px 20px',
    borderRadius: '5px',
    boxShadow: '0px 0px 10px rgba(0, 0, 0, 0.1)',
    zIndex: '1000'
  });
  document.body.appendChild(toast);
  setTimeout(() => {
    document.body.removeChild(toast);
  }, 4000);
};

const getMessage = (error: any) => {
  if (Array.isArray(error)) {
    return error.map((e: any) =>
      typeof e === 'object' ? JSON.stringify(e, null, 2) : e
    ).join('\n');
  }
  if (typeof error === 'object') {
    return JSON.stringify(error, null, 2);
  }
  return error || '';
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    tap(() => {
      if (req.method !== 'GET') {
        createToast('Solicitud completada con éxito', true);
      }
    }),
    catchError(err => {
      const msg = getMessage(  err?.error?.detalle || err?.error?.detail || err?.error?.errors || err?.mensaje || err?.errors || err?.message);
      createToast(`Ocurrió un error en la solicitud:\n${msg}`);
      return throwError(() => err);
    })
  );
};
