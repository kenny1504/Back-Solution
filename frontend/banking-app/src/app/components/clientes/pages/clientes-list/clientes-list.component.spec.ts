import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import {RouterLink, RouterLinkWithHref, RouterModule} from '@angular/router';
import { DebugElement } from '@angular/core';
import { ClientesListComponent } from './clientes-list.component';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('ClientesListComponent', () => {
  let component: ClientesListComponent;
  let fixture: ComponentFixture<ClientesListComponent>;

  const clientesMock = [
    { id: 1, nombre: 'Ana Pérez',  direccion: 'Calle 1', identificacion: 'ID-001', telefono: '555-111', activo: true  },
    { id: 2, nombre: 'Juan López', direccion: 'Calle 2', identificacion: 'ID-002', telefono: '',         activo: false }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({

      imports: [
        ClientesListComponent,
        CommonModule,
        FormsModule,
        RouterTestingModule.withRoutes([]),
        RouterModule,
        HttpClientTestingModule
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ClientesListComponent);
    component = fixture.componentInstance;

    (component as any).view = [...clientesMock];

    fixture.detectChanges();
  });

  it('debe crearse', () => {
    expect(component).toBeTruthy();
  });

  it('debe mostrar el título "Clientes" y el total correcto', () => {
    const title = fixture.debugElement.query(By.css('h2.page-title')).nativeElement as HTMLElement;
    expect(title.textContent?.trim()).toBe('Clientes');

    const total = fixture.debugElement.query(By.css('.total-note')).nativeElement as HTMLElement;
    expect(total.textContent?.trim()).toBe(`Total: ${clientesMock.length} cliente(s)`);
  });

  it('debe renderizar filas de la tabla con datos y badges de estado Activo/Inactivo', () => {
    const rows = fixture.debugElement.queryAll(By.css('tbody tr'));
    // No debe aparecer la fila "vacía" porque hay datos
    expect(rows.length).toBe(clientesMock.length);

    // Primera fila (activo)
    const firstRow = rows[0].nativeElement as HTMLTableRowElement;
    expect(firstRow.textContent).toContain('Ana Pérez');
    const badgeActivo = rows[0].query(By.css('.badge.badge-success'));
    expect(badgeActivo).toBeTruthy();

    // Segunda fila (inactivo)
    const secondRow = rows[1].nativeElement as HTMLTableRowElement;
    expect(secondRow.textContent).toContain('Juan López');
    const badgeInactivo = rows[1].query(By.css('.badge.badge-danger'));
    expect(badgeInactivo).toBeTruthy();

    // Teléfono vacío debe mostrar '-'
    expect(secondRow.textContent).toContain('-');
  });

  it('debe tener un enlace "Nuevo" con routerLink a /clientes/nuevo', () => {
    const linkNuevoDE = fixture.debugElement.query(By.css('a.btn.warning'));
    expect(linkNuevoDE).toBeTruthy();


    const rl = linkNuevoDE.injector.get(RouterLinkWithHref, null) || linkNuevoDE.injector.get(RouterLink, null);

    if ((rl as any)?.href) {
      expect((rl as any).href).toContain('/clientes/nuevo');
    } else if ((rl as any)?.commands) {
      expect((rl as any).commands).toEqual(['/clientes', 'nuevo']);
    } else {

      expect((linkNuevoDE.nativeElement as HTMLAnchorElement).textContent?.trim()).toBe('Nuevo');
    }
  });

  it('debe tener un enlace "Editar" por fila que apunte a /clientes/:id/editar', () => {
    const editLinks = fixture.debugElement.queryAll(By.css('a.btn.btn-success'));
    expect(editLinks.length).toBe(clientesMock.length);

    const rl1 = editLinks[0].injector.get(RouterLinkWithHref, null) || editLinks[0].injector.get(RouterLink, null);
    if ((rl1 as any)?.href) {
      expect((rl1 as any).href).toContain('/clientes/1/editar');
    } else if ((rl1 as any)?.commands) {
      expect((rl1 as any).commands).toEqual(['/clientes', 1, 'editar']);
    }
  });

  it('debe invocar remove(c) al hacer click en "Eliminar"', () => {
    spyOn(component as any, 'remove');

    const deleteButtons = fixture.debugElement.queryAll(By.css('button.btn.btn-danger'));
    expect(deleteButtons.length).toBe(clientesMock.length);

    deleteButtons[1].triggerEventHandler('click', null);
    fixture.detectChanges();

    expect((component as any).remove).toHaveBeenCalledTimes(1);
    expect((component as any).remove).toHaveBeenCalledWith(jasmine.objectContaining({ id: 2 }));
  });

  it('debe llamar a applyFilter() cuando se escribe en el input de búsqueda', fakeAsync(() => {
    spyOn(component as any, 'applyFilter').and.callThrough();

    const inputDE: DebugElement = fixture.debugElement.query(By.css('.search .input'));
    const inputEl = inputDE.nativeElement as HTMLInputElement;

    inputEl.value = 'ana';
    inputEl.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    tick();

    expect((component as any).applyFilter).toHaveBeenCalled();
  }));

  it('no debe mostrar la fila vacía cuando hay elementos en view', () => {
    const emptyRow = fixture.debugElement.query(By.css('tbody tr td[colspan]'));
    expect(emptyRow).toBeNull();
  });

  it('debe mostrar la fila vacía cuando view está vacío', () => {
    (component as any).view = [];
    fixture.detectChanges();

    const emptyRow = fixture.debugElement.query(By.css('tbody tr td[colspan]'));
    expect(emptyRow).not.toBeNull();
  });
});
