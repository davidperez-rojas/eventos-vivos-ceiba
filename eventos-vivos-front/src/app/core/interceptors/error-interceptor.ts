import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'Ocurrió un error inesperado';
      if (error.error?.error) message = error.error.error;
      else if (error.message) message = error.message;
      return throwError(() => new Error(message));
    })
  );
};
