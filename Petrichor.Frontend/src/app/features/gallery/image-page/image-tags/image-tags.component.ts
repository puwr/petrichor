import { Component, OnInit, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ValidationErrorsComponent, ButtonComponent, IconComponent } from '@app/shared/components';
import { ImageService } from '../../image.service';
import { RouterLink } from '@angular/router';
import { Field, form, submit } from '@angular/forms/signals';
import { catchError, firstValueFrom, map, of } from 'rxjs';

@Component({
  selector: 'app-tags',
  imports: [
    ReactiveFormsModule,
    ValidationErrorsComponent,
    ButtonComponent,
    IconComponent,
    RouterLink,
    Field,
  ],
  templateUrl: './image-tags.component.html',
  styleUrl: './image-tags.component.scss',
})
export class TagsComponent implements OnInit {
  private imageService = inject(ImageService);

  imageId = input.required<string>();
  tags = input.required<string[]>();
  isUploader = input.required<boolean>();

  tagsForm = form(signal({ tags: '' }));
  tagsChanged = output<void>();
  showTagsInput = false;

  ngOnInit(): void {
    this.tagsForm().value.set({ tags: this.tags().join(', ') });
  }

  onReset(): void {
    this.showTagsInput = false;
    this.tagsForm().value.set({ tags: this.tags().join(', ') });
  }

  async onSubmit(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    await submit(this.tagsForm, async (form) => {
      const formValue = form().value();

      const tagsString = formValue.tags.trim();

      const tagsArray = tagsString
        .split(',')
        .map((tag) => tag.trim().toLocaleLowerCase())
        .filter((tag) => tag.length > 0);

      return await firstValueFrom(
        this.imageService.updateImageTags(this.imageId(), tagsArray ?? []).pipe(
          map(() => {
            this.tagsChanged.emit();
            form().value.set({ tags: tagsArray?.join(', ') });
            this.showTagsInput = false;
          }),
          catchError((errors) => of(errors)),
        ),
      );
    });
  }
}
