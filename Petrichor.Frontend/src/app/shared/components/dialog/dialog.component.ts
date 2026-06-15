import { DialogRef, DIALOG_DATA } from '@angular/cdk/dialog';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ButtonComponent } from '../button/button.component';
import { DialogData } from './dialog.models';

@Component({
  selector: 'app-dialog',
  imports: [ButtonComponent],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DialogComponent {
  dialogRef = inject(DialogRef);
  data: DialogData = inject(DIALOG_DATA);
}
