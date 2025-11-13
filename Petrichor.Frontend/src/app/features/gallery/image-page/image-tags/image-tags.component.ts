import { Component, OnInit, inject, DestroyRef, input, output } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import {
  TextInputComponent,
  ValidationErrorsComponent,
  ButtonComponent,
  IconComponent,
} from '@app/shared/components';
import { ImageService } from '../../image.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-tags',
  imports: [
    ReactiveFormsModule,
    TextInputComponent,
    ValidationErrorsComponent,
    ButtonComponent,
    IconComponent,
    RouterLink,
  ],
  templateUrl: './image-tags.component.html',
  styleUrl: './image-tags.component.scss',
})
export class TagsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private imageService = inject(ImageService);
  private destroyRef = inject(DestroyRef);

  imageId = input.required<string>();
  tags = input.required<string[]>();
  isUploader = input.required<boolean>();

  tagsForm = this.fb.group({
    tags: [''],
  });
  tagsChanged = output<void>();
  validationErrors: string[] | null = null;
  showTagsInput = false;

  ngOnInit(): void {
    this.tagsForm.patchValue({ tags: this.tags().join(', ') });
  }

  onReset(): void {
    this.showTagsInput = false;
    this.validationErrors = null;
    this.tagsForm.patchValue({ tags: this.tags().join(', ') });
  }

  onSubmit(): void {
    const tagsString = this.tagsForm.value.tags?.trim();

    const tagsArray = tagsString
      ?.split(',')
      .map((tag) => tag.trim().toLocaleLowerCase())
      .filter((tag) => tag.length > 0);

    this.imageService
      .updateImageTags(this.imageId(), tagsArray ?? [])
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.tagsChanged.emit();
          this.tagsForm.patchValue({ tags: tagsArray?.join(', ') });
          this.showTagsInput = false;
        },
        error: (errors) => (this.validationErrors = errors),
      });
  }
}
