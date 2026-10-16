import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'auth/register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'pix',
    canActivate: [authGuard],
    loadComponent: () => import('./features/pix/pix-topup/pix-topup.component').then(m => m.PixTopupComponent)
  },
  {
    path: 'campus',
    canActivate: [authGuard],
    loadComponent: () => import('./features/campus/campus-catalog/campus-catalog.component').then(m => m.CampusCatalogComponent)
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
