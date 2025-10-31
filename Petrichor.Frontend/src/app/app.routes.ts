import { Routes } from '@angular/router';
import { authGuard } from './core/auth';
import { RegisterPageComponent, LoginPageComponent } from './features/auth';
import { GalleryPageComponent, ImagePageComponent, UploadPageComponent } from './features/gallery';
import { NotFoundPageComponent } from './pages';

export const routes: Routes = [
  { path: '', component: GalleryPageComponent },
  { path: 'images/:id', component: ImagePageComponent },
  { path: 'upload', component: UploadPageComponent, canActivate: [authGuard] },
  { path: 'register', component: RegisterPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'not-found', component: NotFoundPageComponent },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
