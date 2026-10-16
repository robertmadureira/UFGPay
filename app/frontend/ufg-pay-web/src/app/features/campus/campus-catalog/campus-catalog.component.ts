import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CampusService } from '../../../core/services/campus.service';
import { CampusServiceItem } from '../../../core/models/campus.model';
import { ServiceItemComponent } from '../components/service-item/service-item.component';
import { VoucherCardComponent } from '../components/voucher-card/voucher-card.component';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-campus-catalog',
  standalone: true,
  imports: [CommonModule, ServiceItemComponent, VoucherCardComponent],
  templateUrl: './campus-catalog.component.html',
  styleUrls: ['./campus-catalog.component.scss']
})
export class CampusCatalogComponent implements OnInit {
  public readonly campusService = inject(CampusService);
  private readonly notificationService = inject(NotificationService);

  public readonly buyingServiceId = signal<string | null>(null);
  public readonly issuedVoucher = signal<{ code: string; serviceName: string } | null>(null);

  ngOnInit(): void {
    this.campusService.loadServices().subscribe();
  }

  public onBuyService(service: CampusServiceItem): void {
    this.buyingServiceId.set(service.id);

    this.campusService.checkout({
      servicoId: service.id,
      valorCentavos: service.precoCentavos
    }).subscribe({
      next: (res) => {
        this.buyingServiceId.set(null);
        if (res.sucesso && res.codigoVoucher) {
          this.issuedVoucher.set({
            code: res.codigoVoucher,
            serviceName: service.nome
          });
          this.notificationService.success('Voucher emitido com sucesso!');
        }
      },
      error: () => {
        this.buyingServiceId.set(null);
      }
    });
  }

  public onCloseVoucher(): void {
    this.issuedVoucher.set(null);
  }
}
