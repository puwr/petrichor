import { Routes } from '@angular/router';
import { GalleryPageComponent } from './pages/gallery-page/gallery-page.component';
import { ImagePageComponent } from './pages/image-page/image-page.component';

export const routes: Routes = [
  { path: '', component: GalleryPageComponent },
  { path: 'images/:id', component: ImagePageComponent },
];
