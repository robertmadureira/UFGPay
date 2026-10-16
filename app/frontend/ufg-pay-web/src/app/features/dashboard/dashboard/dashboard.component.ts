import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { WalletService } from '../../../core/services/wallet.service';
import { CampusService } from '../../../core/services/campus.service';
import { BalanceCardComponent } from '../components/balance-card/balance-card.component';
import { BrlCurrencyPipe } from '../../../shared/pipes/brl-currency.pipe';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, BalanceCardComponent, BrlCurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  public readonly authService = inject(AuthService);
  public readonly walletService = inject(WalletService);
  public readonly campusService = inject(CampusService);

  ngOnInit(): void {
    this.walletService.loadBalance().subscribe();
    this.campusService.loadServices().subscribe();
  }
}
