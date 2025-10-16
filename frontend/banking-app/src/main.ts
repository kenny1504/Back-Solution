import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { AppComponent } from '@app/app.component';
import { routes } from '@app/app-routing.module';
import { errorInterceptor } from '@app/core/interceptors/error.interceptor';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(withInterceptors([errorInterceptor])),
    provideRouter(routes),
  ],
}).catch(err => console.error(err));
