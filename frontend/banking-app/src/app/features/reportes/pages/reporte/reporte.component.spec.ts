import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReporteComponent } from './reporte.component';
import {HttpClient} from '@angular/common/http';

describe('ReporteComponent', () => {
  let component: ReporteComponent;
  let fixture: ComponentFixture<ReporteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReporteComponent],
      providers: [
        {
          provide: HttpClient,
          useValue: {}
        }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ReporteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
