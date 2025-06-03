import { Component, input } from '@angular/core';
import { Tag } from '../../../shared/models/tag';

@Component({
  selector: 'app-tags',
  imports: [],
  templateUrl: './tags.component.html',
  styleUrl: './tags.component.scss',
})
export class TagsComponent {
  tags = input.required<Tag[]>();
}
