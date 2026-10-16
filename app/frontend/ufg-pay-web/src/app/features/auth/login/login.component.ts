import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);

  public readonly isSubmitting = signal<boolean>(false);

  public readonly loginForm: FormGroup = this.fb.group({
    matricula: ['', [Validators.required, Validators.minLength(5)]],
    senha: ['', [Validators.required]]
  });

  public onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.authService.login(this.loginForm.value).subscribe({
      next: (session) => {
        this.isSubmitting.set(false);
        if (session.sucesso) {
          this.notificationService.success(`Bem-vindo(a), ${session.nome || 'Usuário'}!`);
          this.router.navigate(['/dashboard']);
        }
      },
      error: () => {
        this.isSubmitting.set(false);
      }
    });
  }
}
