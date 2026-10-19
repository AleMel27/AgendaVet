import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { UsuarioService } from './usuario.service';
import { Usuario, UsuarioCreateRequest, UsuarioUpdateRequest, Rol, PaginatedResponse } from '../models/usuario.model';
import { environment } from '../../../environments/environment';

describe('UsuarioService', () => {
  let service: UsuarioService;
  let httpTesting: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/Usuario`;
  const rolUrl = `${environment.apiUrl}/Rol`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        UsuarioService
      ]
    });

    service = TestBed.inject(UsuarioService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get paginated users', () => {
    const mockRespuesta: PaginatedResponse<Usuario> = {
      items: [
        {
          idUsuario: 1,
          idRol: 1,
          rolNombre: 'Administrador',
          nombre: 'Juan',
          apellido: 'Pérez',
          email: 'admin@vet.com',
          telefono: '123456',
          activo: true,
          fechaRegistro: '2026-01-01'
        }
      ],
      totalCount: 1,
      pageNumber: 1,
      pageSize: 10,
      totalPages: 1
    };

    service.obtenerPaginado(1, 10, 'Juan').subscribe(res => {
      expect(res).toEqual(mockRespuesta);
    });

    const req = httpTesting.expectOne(`${baseUrl}?page=1&pageSize=10&buscar=Juan`);
    expect(req.request.method).toBe('GET');
    req.flush(mockRespuesta);
  });

  it('should create user via POST', () => {
    const nuevo: UsuarioCreateRequest = {
      idRol: 2,
      nombre: 'Carlos',
      apellido: 'Ruiz',
      email: 'carlos@vet.com',
      password: 'password123',
      telefono: '555555',
      activo: true
    };

    const mockResp: Usuario = {
      idUsuario: 2,
      idRol: 2,
      rolNombre: 'Veterinario',
      nombre: nuevo.nombre,
      apellido: nuevo.apellido,
      email: nuevo.email,
      telefono: nuevo.telefono,
      activo: true,
      fechaRegistro: '2026-01-01'
    };

    service.crear(nuevo).subscribe(res => {
      expect(res).toEqual(mockResp);
    });

    const req = httpTesting.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(nuevo);
    req.flush(mockResp);
  });

  it('should update user via PUT', () => {
    const dto: UsuarioUpdateRequest = {
      idRol: 2,
      nombre: 'Carlos Modificado',
      apellido: 'Ruiz',
      telefono: '999999',
      activo: true
    };

    const mockResp: Usuario = {
      idUsuario: 2,
      idRol: 2,
      rolNombre: 'Veterinario',
      nombre: dto.nombre,
      apellido: dto.apellido,
      email: 'carlos@vet.com',
      telefono: dto.telefono,
      activo: true,
      fechaRegistro: '2026-01-01'
    };

    service.actualizar(2, dto).subscribe(res => {
      expect(res).toEqual(mockResp);
    });

    const req = httpTesting.expectOne(`${baseUrl}/2`);
    expect(req.request.method).toBe('PUT');
    req.flush(mockResp);
  });

  it('should delete user via DELETE', () => {
    service.eliminar(2).subscribe();

    const req = httpTesting.expectOne(`${baseUrl}/2`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('should fetch roles', () => {
    const mockRoles: Rol[] = [
      { idRol: 1, nombre: 'Administrador' },
      { idRol: 2, nombre: 'Veterinario' }
    ];

    service.obtenerRoles().subscribe(roles => {
      expect(roles).toEqual(mockRoles);
    });

    const req = httpTesting.expectOne(rolUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockRoles);
  });
});
