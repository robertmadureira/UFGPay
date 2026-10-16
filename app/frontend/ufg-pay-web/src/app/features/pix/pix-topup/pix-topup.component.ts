import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { WalletService } from '../../../core/services/wallet.service';
import { NotificationService } from '../../../core/services/notification.service';
import { BrlCurrencyPipe } from '../../../shared/pipes/brl-currency.pipe';

@Component({
  selector: 'app-pix-topup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, BrlCurrencyPipe],
  templateUrl: './pix-topup.component.html',
  styleUrls: ['./pix-topup.component.scss']
})
export class PixTopupComponent {
  private readonly fb = inject(FormBuilder);
  private readonly walletService = inject(WalletService);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);

  public readonly isProcessing = signal<boolean>(false);
  public readonly copied = signal<boolean>(false);

  public readonly topupForm: FormGroup = this.fb.group({
    valorReais: [20, [Validators.required, Validators.min(1)]]
  });

  public readonly quickValues: number[] = [10, 20, 50, 100];

  // Simulação de Chave Copia e Cola Padrão BACEN BR Code
  public readonly pixCopiaECola = computed(() => {
    const val = this.topupForm.get('valorReais')?.value || 20;
    return `00020126580014br.gov.bcb.pix0136ufgpay-pix-${Math.round(val * 100)}520400005303986540${val}.005802BR5925UNIVERSIDADE FEDERAL GOIAS6007GOIANIA62070503***6304`;
  });

  public readonly qrCodePixUrl = computed(() => {
    return `https://api.qrserver.com/v1/create-qr-code/?size=220x220&margin=8&data=${encodeURIComponent(this.pixCopiaECola())}`;
  });

  public selectQuickValue(value: number): void {
    this.topupForm.patchValue({ valorReais: value });
  }

  public onCopyPix(): void {
    if (navigator?.clipboard) {
      navigator.clipboard.writeText(this.pixCopiaECola());
      this.copied.set(true);
      this.notificationService.success('Código PIX Copia e Cola copiado!');
      setTimeout(() => this.copied.set(false), 2500);
    }
  }

  public onSubmit(): void {
    if (this.topupForm.invalid) {
      this.topupForm.markAllAsTouched();
      return;
    }

    const valorReais = Number(this.topupForm.get('valorReais')?.value);
    const valorCentavos = Math.round(valorReais * 100);

    this.isProcessing.set(true);

    this.walletService.topupPix({ valorCentavos }).subscribe({
      next: (res) => {
        this.isProcessing.set(false);
        if (res.sucesso) {
          this.notificationService.success(`Recarga de R$ ${valorReais.toFixed(2)} confirmada!`);
          this.router.navigate(['/dashboard']);
        }
      },
      error: () => {
        this.isProcessing.set(false);
      }
    });
  }
}
