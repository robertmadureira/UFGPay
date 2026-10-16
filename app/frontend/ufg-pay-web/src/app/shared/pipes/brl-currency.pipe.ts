import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'brlCurrency',
  standalone: true
})
export class BrlCurrencyPipe implements PipeTransform {
  transform(value: number | null | undefined, isCents = false): string {
    if (value === null || value === undefined) return 'R$ 0,00';
    const amount = isCents ? value / 100 : value;
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(amount);
  }
}
