import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocorreu um erro inesperado.';

      if (error.status === 401) {
        errorMessage = 'Sessão expirada ou não autorizada. Faça login novamente.';
        authService.logout();
      } else if (error.error?.mensagem) {
        errorMessage = error.error.mensagem;
      } else if (error.error?.erros && Array.isArray(error.error.erros)) {
        errorMessage = error.error.erros.join(' | ');
      }

      notificationService.error(errorMessage);
      return throwError(() => error);
    })
  );
};
