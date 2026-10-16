import { Component, EventEmitter, Input, Output, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-voucher-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './voucher-card.component.html',
  styleUrls: ['./voucher-card.component.scss']
})
export class VoucherCardComponent {
  @Input({ required: true }) voucherCode!: string;
  @Input() serviceName: string = '';
  @Output() close = new EventEmitter<void>();

  public readonly copied = signal<boolean>(false);
  public readonly imageError = signal<boolean>(false);

  public readonly qrCodeUrl = computed(() => {
    if (!this.voucherCode) return '';
    return `https://api.qrserver.com/v1/create-qr-code/?size=220x220&margin=8&data=${encodeURIComponent(this.voucherCode)}`;
  });

  public onCopyCode(): void {
    if (navigator?.clipboard && this.voucherCode) {
      navigator.clipboard.writeText(this.voucherCode);
      this.copied.set(true);
      setTimeout(() => this.copied.set(false), 2500);
    }
  }

  public onImageError(): void {
    this.imageError.set(true);
  }

  public onClose(): void {
    this.close.emit();
  }
}
