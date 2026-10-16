import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);

  public readonly isSubmitting = signal<boolean>(false);

  public readonly registerForm: FormGroup = this.fb.group({
    matricula: ['', [Validators.required, Validators.maxLength(20)]],
    nome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email]],
    senha: ['', [Validators.required, Validators.minLength(6)]],
    tipoVinculo: ['discente', [Validators.required]]
  });

  public onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const email = this.registerForm.get('email')?.value || '';
    if (!email.toLowerCase().endsWith('@ufg.br') && !email.toLowerCase().endsWith('@discente.ufg.br')) {
      this.notificationService.error('O e-mail deve ser institucional (@ufg.br ou @discente.ufg.br).');
      return;
    }

    this.isSubmitting.set(true);

    this.authService.register(this.registerForm.value).subscribe({
      next: (session) => {
        this.isSubmitting.set(false);
        if (session.sucesso) {
          this.notificationService.success('Cadastro e carteira acadêmica criados com sucesso!');
          this.router.navigate(['/dashboard']);
        }
      },
      error: () => {
        this.isSubmitting.set(false);
      }
    });
  }
}
