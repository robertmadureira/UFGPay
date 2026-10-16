import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { TopupPixRequestPayload, TopupPixResponsePayload, WalletBalance } from '../models/wallet.model';

@Injectable({
  providedIn: 'root'
})
export class WalletService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = '/api/v1/wallet';

  private readonly _balance = signal<WalletBalance | null>(null);
  public readonly balance = this._balance.asReadonly();
  public readonly isLoading = signal<boolean>(false);

  public loadBalance(): Observable<WalletBalance> {
    this.isLoading.set(true);
    return this.http.get<WalletBalance>(`${this.API_URL}/balance`).pipe(
      tap({
        next: (data) => {
          this._balance.set(data);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      })
    );
  }

  public topupPix(payload: TopupPixRequestPayload): Observable<TopupPixResponsePayload> {
    return this.http.post<TopupPixResponsePayload>(`${this.API_URL}/topup-pix`, payload).pipe(
      tap(res => {
        if (res.sucesso) {
          this._balance.update(current => current ? {
            ...current,
            saldoCentavos: res.novoSaldoCentavos,
            saldoReais: res.novoSaldoReais
          } : null);
        }
      })
    );
  }

  public updateBalanceManually(novoSaldoCentavos: number, novoSaldoReais: number): void {
    this._balance.update(current => current ? {
      ...current,
      saldoCentavos: novoSaldoCentavos,
      saldoReais: novoSaldoReais
    } : null);
  }
}
