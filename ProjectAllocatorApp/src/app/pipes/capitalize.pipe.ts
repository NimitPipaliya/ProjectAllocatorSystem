import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'capitalize'
})
export class CapitalizePipe implements PipeTransform {

  transform(value: string | null | undefined): string | null | undefined {
    if(!value) return value;
    return value.replace(/\b\w/g, first => first.toLocaleUpperCase());
  };
}
