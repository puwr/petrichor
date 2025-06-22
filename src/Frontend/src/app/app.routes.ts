import { Routes } from '@angular/router';
import { GalleryPageComponent } from './pages/gallery-page/gallery-page.component';
import { ImagePageComponent } from './pages/image-page/image-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { NotFoundPageComponent } from './pages/not-found-page/not-found-page.component';
import { UploadPageComponent } from './pages/upload-page/upload-page.component';

export const routes: Routes = [
  { path: '', component: GalleryPageComponent },
  { path: 'images/:id', component: ImagePageComponent },
  { path: 'upload', component: UploadPageComponent },
  { path: 'register', component: RegisterPageComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'not-found', component: NotFoundPageComponent },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
