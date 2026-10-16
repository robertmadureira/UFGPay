import { Injectable, signal } from '@angular/core';

export interface ToastMessage {
  id: string;
  type: 'success' | 'error' | 'info' | 'warning';
  title?: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly _messages = signal<ToastMessage[]>([]);
  public readonly messages = this._messages.asReadonly();

  public show(message: string, type: 'success' | 'error' | 'info' | 'warning' = 'info', title?: string, durationMs = 4000): void {
    const id = Math.random().toString(36).substring(2, 9);
    const toast: ToastMessage = { id, type, title, message };

    this._messages.update(prev => [...prev, toast]);

    if (durationMs > 0) {
      setTimeout(() => this.remove(id), durationMs);
    }
  }

  public success(message: string, title = 'Sucesso'): void {
    this.show(message, 'success', title);
  }

  public error(message: string, title = 'Erro'): void {
    this.show(message, 'error', title, 6000);
  }

  public remove(id: string): void {
    this._messages.update(prev => prev.filter(m => m.id !== id));
  }
}
