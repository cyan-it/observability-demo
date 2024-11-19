import { HttpEvent, HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { instrumentation } from '../utils/instrumentation';

export const HttpRequestTracingInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
  return new Observable(observer => {
    instrumentation.startActiveSpan('http-request', span => {
      span.setAttribute('url', req.url);

      next(req).pipe(
        tap({
          next: (event) => {
            if (event instanceof HttpResponse) {
              span.end();
            }
          },
          error: (error: HttpErrorResponse) => {
            span.end();
            observer.error(error);
          }
        })
      ).subscribe({
        next: (event) => {
          observer.next(event);
        },
        error: (error) => {
          observer.error(error);
        },
        complete: () => {
          observer.complete();
        }
      });
    });
  });
};
