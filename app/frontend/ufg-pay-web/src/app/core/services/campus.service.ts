import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { CampusServiceItem, CheckoutRequestPayload, CheckoutResponsePayload } from '../models/campus.model';
import { WalletService } from './wallet.service';

@Injectable({
  providedIn: 'root'
})
export class CampusService {
  private readonly http = inject(HttpClient);
  private readonly walletService = inject(WalletService);
  private readonly API_URL = '/api/v1/campus';

  private readonly _services = signal<CampusServiceItem[]>([]);
  public readonly services = this._services.asReadonly();
  public readonly isLoading = signal<boolean>(false);

  public loadServices(): Observable<CampusServiceItem[]> {
    this.isLoading.set(true);
    return this.http.get<CampusServiceItem[]>(`${this.API_URL}/services`).pipe(
      tap({
        next: (items) => {
          this._services.set(items);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      })
    );
  }

  public checkout(payload: CheckoutRequestPayload): Observable<CheckoutResponsePayload> {
    return this.http.post<CheckoutResponsePayload>(`${this.API_URL}/checkout`, payload).pipe(
      tap(res => {
        if (res.sucesso && res.novoSaldoCentavos !== undefined && res.novoSaldoReais !== undefined) {
          this.walletService.updateBalanceManually(res.novoSaldoCentavos, res.novoSaldoReais);
        }
      })
    );
  }
}
