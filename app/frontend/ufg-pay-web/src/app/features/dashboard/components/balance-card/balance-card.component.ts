import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BrlCurrencyPipe } from '../../../../shared/pipes/brl-currency.pipe';

@Component({
  selector: 'app-balance-card',
  standalone: true,
  imports: [CommonModule, RouterModule, BrlCurrencyPipe],
  templateUrl: './balance-card.component.html',
  styleUrls: ['./balance-card.component.scss']
})
export class BalanceCardComponent {
  @Input() saldoReais: number = 0;
  @Input() saldoCentavos: number = 0;
  @Input() bloqueada: boolean = false;
  @Input() userName: string = '';
}
