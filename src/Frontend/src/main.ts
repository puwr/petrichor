import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { devTools } from '@ngneat/elf-devtools';
import { isDevMode } from '@angular/core';

bootstrapApplication(App, appConfig)
  .then(() => {
    if (isDevMode()) {
      devTools();
    }
  })
  .catch((err) => console.error(err));
