import { Component, DestroyRef, inject, input, OnDestroy, OnInit, output } from '@angular/core';
import { Tag } from '../../../shared/models/image';
import { ImageService } from '../../../core/services/image.service';
import { exhaustMap, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { ValidationErrorsComponent } from '../../../shared/components/validation-errors/validation-errors.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-tags',
  imports: [
    ReactiveFormsModule,
    TextInputComponent,
    ValidationErrorsComponent,
    ButtonComponent,
    IconComponent,
  ],
  templateUrl: './tags.component.html',
  styleUrl: './tags.component.scss',
})
export class TagsComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private imageService = inject(ImageService);
  private snackbar = inject(SnackbarService);
  private destroyRef = inject(DestroyRef);

  imageId = input.required<string>();
  tags = input.required<Tag[]>();
  isUploader = input.required<boolean>();

  tagsChanged = output<void>();

  private addTags$ = new Subject<string[]>();
  private deleteTag$ = new Subject<string>();

  validationErrors: string[] | null = null;

  showAddTagInput = false;

  addTagsForm = this.fb.group({
    tags: ['', Validators.required],
  });

  ngOnInit(): void {
    this.addTags$
      .pipe(
        exhaustMap((tags) => this.imageService.addImageTags(this.imageId(), tags)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.tagsChanged.emit();
          this.showAddTagInput = false;
        },
        error: (errors) => (this.validationErrors = errors),
      });

    this.deleteTag$
      .pipe(
        exhaustMap((tagId) => this.imageService.deleteImageTag(this.imageId(), tagId)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => this.tagsChanged.emit());
  }

  deleteTag(tagId: string) {
    this.deleteTag$.next(tagId);
  }

  onReset(): void {
    this.addTagsForm.reset();
    this.showAddTagInput = false;
  }

  onSubmit(): void {
    const tagsString = this.addTagsForm.value.tags?.trim();

    if (!tagsString) {
      this.snackbar.error('Please enter at least one tag.');
      return;
    }

    const tagsArray = tagsString
      ?.split(',')
      .map((tag) => tag.trim())
      .filter((tag) => tag.length > 0);

    if (tagsArray?.length === 0) {
      this.snackbar.error('Please enter valid tags.');
      return;
    }

    this.addTags$.next(tagsArray);
  }

  ngOnDestroy(): void {
    this.addTags$.complete();
    this.deleteTag$.complete();
  }
}
