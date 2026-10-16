import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthSession, LoginCredentials, RegisterData } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly API_URL = '/api/v1/auth';
  private readonly TOKEN_KEY = 'ufgpay_access_token';
  private readonly USER_KEY = 'ufgpay_user_session';

  private readonly _session = signal<AuthSession | null>(this.loadStoredSession());
  public readonly session = this._session.asReadonly();
  public readonly isAuthenticated = computed(() => !!this._session()?.token);
  public readonly currentUser = computed(() => this._session());

  public login(credentials: LoginCredentials): Observable<AuthSession> {
    return this.http.post<AuthSession>(`${this.API_URL}/login`, credentials).pipe(
      tap(response => {
        if (response.sucesso && response.token) {
          this.setSession(response);
        }
      })
    );
  }

  public register(data: RegisterData): Observable<AuthSession> {
    return this.http.post<AuthSession>(`${this.API_URL}/register`, data).pipe(
      tap(response => {
        if (response.sucesso && response.token) {
          this.setSession(response);
        }
      })
    );
  }

  public logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this._session.set(null);
    this.router.navigate(['/auth/login']);
  }

  public getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private setSession(session: AuthSession): void {
    localStorage.setItem(this.TOKEN_KEY, session.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(session));
    this._session.set(session);
  }

  private loadStoredSession(): AuthSession | null {
    try {
      const stored = localStorage.getItem(this.USER_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }
}
