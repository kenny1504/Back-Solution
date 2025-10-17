import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { CuentasFormComponent } from './cuentas-form.component';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('CuentasFormComponent', () => {
  let component: any;
  let fixture: ComponentFixture<any>;

  const clientesMock = [
    { id: 10, nombre: 'Cliente A' },
    { id: 20, nombre: 'Cliente B' },
    { id: 30, nombre: 'Cliente C' }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CuentasFormComponent,
        CommonModule,
        FormsModule,
        RouterTestingModule.withRoutes([]) ,
        HttpClientTestingModule
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CuentasFormComponent);
    component = fixture.componentInstance;

    component.id = undefined;
    component.model = {
      clienteIdFk: null,
      numero: '',
      tipo: '1',
      saldoInicial: null,
      activa: false
    };
    component.clientes = [...clientesMock];

    fixture.detectChanges();
  });

  it('debe crearse', () => {
    expect(component).toBeTruthy();
  });

  it('debe mostrar "Nueva Cuenta" cuando id es undefined/null', () => {
    const titleEl: HTMLElement = fixture.debugElement.query(By.css('.card.modern-card h3')).nativeElement;
    expect(titleEl.textContent?.trim()).toBe('Nueva Cuenta');
  });

  it('debe mostrar "Editar Cuenta" cuando id tiene valor', () => {
    component.id = 99;
    fixture.detectChanges();
    const titleEl: HTMLElement = fixture.debugElement.query(By.css('.card.modern-card h3')).nativeElement;
    expect(titleEl.textContent?.trim()).toBe('Editar Cuenta');
  });


  it('debe bindear el input "Número" a model.numero (two-way binding)', () => {
    const numeroDE = fixture.debugElement.query(By.css('input[type="text"].modern-input'));
    const numeroEl = numeroDE.nativeElement as HTMLInputElement;

    numeroEl.value = 'CTA-001-ABC';
    numeroEl.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    expect(component.model.numero).toBe('CTA-001-ABC');
  });

  it('debe bindear el select "Tipo" a model.tipo (1=Ahorros, 2=Corriente)', () => {
    const selects = fixture.debugElement.queryAll(By.css('select.input.modern-input'));
    const tipoDE = selects.find(de => {
      const el = de.nativeElement as HTMLSelectElement;
      return Array.from(el.options).some(o => o.value === '1') &&
        Array.from(el.options).some(o => o.value === '2');
    })!;
    const tipoEl = tipoDE.nativeElement as HTMLSelectElement;

    tipoEl.value = '2';
    tipoEl.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    expect(component.model.tipo).toBe('2');
  });

  it('debe bindear el input "Saldo Inicial" (number) a model.saldoInicial', () => {
    const saldoDE = fixture.debugElement.queryAll(By.css('input.input.modern-input[type="number"]'))[0];
    const saldoEl = saldoDE.nativeElement as HTMLInputElement;

    saldoEl.value = '1234.56';
    saldoEl.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    expect(component.model.saldoInicial).toBe(1234.56);
  });

  it('debe togglear el checkbox "Estado" y actualizar model.activa (two-way binding)', () => {
    const checkDE = fixture.debugElement.query(By.css('input[type="checkbox"]'));
    const checkEl = checkDE.nativeElement as HTMLInputElement;

    expect(component.model.activa).toBeFalse();

    checkEl.checked = true;
    checkEl.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    expect(component.model.activa).toBeTrue();

    checkEl.checked = false;
    checkEl.dispatchEvent(new Event('change'));
    fixture.detectChanges();

    expect(component.model.activa).toBeFalse();
  });

  it('el enlace "Volver" debe apuntar a /cuentas', () => {
    const volverDE = fixture.debugElement.query(By.css('a.btn.btn-primary'));
    const anchor = volverDE.nativeElement as HTMLAnchorElement;

    expect(anchor.getAttribute('href') || '').toContain('/cuentas');
  });

  it('debe invocar save() al hacer click en "Guardar"', () => {
    spyOn(component, 'save').and.callFake(() => {});
    const btnGuardarDE = fixture.debugElement.query(By.css('button.btn.btn-success'));
    btnGuardarDE.triggerEventHandler('click', {});
    fixture.detectChanges();

    expect(component.save).toHaveBeenCalledTimes(1);
  });
});
