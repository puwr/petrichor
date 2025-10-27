import { DIALOG_DATA, DialogRef } from '@angular/cdk/dialog';
import { Component, inject } from '@angular/core';
import { DialogData } from '../../models/dialog';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-dialog',
  imports: [ButtonComponent],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.scss',
})
export class DialogComponent {
  dialogRef = inject(DialogRef);
  data: DialogData = inject(DIALOG_DATA);
}
