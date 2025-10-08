import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class ScanQrService {
  private isAddedListener = false;
  private wheelHandler: any;
  private readString = '';

  // Quét QR
  addListener(callback = (maCheckIn: string) => {}, searchCallback = () => {}) {
    if (!this.isAddedListener) {
      this.isAddedListener = true;
      this.wheelHandler = this.listenQrCode.bind(this, callback, searchCallback);
      const node: any = window.document;

      node.addEventListener('keypress', this.wheelHandler, true);
    }
  }

  clearListener() {
    this.isAddedListener = false;
    const node = window.document;
    node.removeEventListener('keypress', this.wheelHandler, true);
  }

  listenQrCode(callback: any, searchCallback: any, event: any) {
    if (event.keyCode === 13) {
      const value = this.readString;
      this.readString = '';

      if (value !== '') {
        if (value.includes('http') && value.includes('mssv')) {
          console.log(value)
          const parsedUrl = new URL(value);
          const data = parsedUrl.searchParams.get('mssv');

          callback(data);
        } else {
          // bình thường
          searchCallback();  
        }
      } else {
        searchCallback();
      }
    } else {
      this.readString += event?.key || '';
    }
  }

}
