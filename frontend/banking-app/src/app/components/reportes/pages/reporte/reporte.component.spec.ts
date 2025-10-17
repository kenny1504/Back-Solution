import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReporteComponent} from './reporte.component';
import {of} from 'rxjs';
import {ApiService} from '@app/core/services/api.service';

describe('ReporteComponent', () => {
  let component: ReporteComponent;
  let fixture: ComponentFixture<ReporteComponent>;
  let mockHttpClient: jasmine.SpyObj<ApiService>;

  beforeEach(async () => {
    mockHttpClient = jasmine.createSpyObj('ApiService', ['getResumenMovimientos', 'getAll']);


    mockHttpClient.getAll.and.returnValue(of([]));
    mockHttpClient.getResumenMovimientos.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [ReporteComponent],
      providers: [
        {
          provide: ApiService,
          useValue: mockHttpClient
        }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ReporteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse', () => {
    expect(component).toBeTruthy();
  });

});
