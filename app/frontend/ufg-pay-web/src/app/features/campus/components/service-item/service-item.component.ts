import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CampusServiceItem } from '../../../../core/models/campus.model';
import { BrlCurrencyPipe } from '../../../../shared/pipes/brl-currency.pipe';

@Component({
  selector: 'app-service-item',
  standalone: true,
  imports: [CommonModule, BrlCurrencyPipe],
  templateUrl: './service-item.component.html',
  styleUrls: ['./service-item.component.scss']
})
export class ServiceItemComponent {
  @Input({ required: true }) item!: CampusServiceItem;
  @Input() isBuying = false;
  @Output() buy = new EventEmitter<CampusServiceItem>();

  public onBuy(): void {
    this.buy.emit(this.item);
  }
}
