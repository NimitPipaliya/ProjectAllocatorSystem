import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocalstorageService {

  constructor() { }

  setItem(key: string, value:  string): void{
    localStorage.setItem(key, value);
  }

  getItem(key: string): string | null | undefined { 
    return localStorage.getItem(key);
  }

  //to check any item exists or not
  hasItem(key: string): boolean{
    return localStorage.getItem(key) ? true: false;
  }

  //to remove item and call when we log out
  removeItem(key: string): void{
    localStorage.removeItem(key);
  }
}
