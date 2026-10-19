import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ServicioService } from './servicio.service';
import { Servicio, ServicioRequest } from '../models/servicio.model';
import { environment } from '../../../environments/environment';

describe('ServicioService', () => {
  let service: ServicioService;
  let httpTesting: HttpTestingController;
  const baseUrl = `${environment.apiUrl}/Servicio`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        ServicioService
      ]
    });

    service = TestBed.inject(ServicioService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all services with soloActivos query param', () => {
    const mockServicios: Servicio[] = [
      { idServicio: 1, nombre: 'Consulta', precio: 50, duracionEstimadaMin: 30, activo: true }
    ];

    service.obtenerTodos(true).subscribe(data => {
      expect(data).toEqual(mockServicios);
    });

    const req = httpTesting.expectOne(`${baseUrl}?soloActivos=true`);
    expect(req.request.method).toBe('GET');
    req.flush(mockServicios);
  });

  it('should create a service via POST', () => {
    const nuevo: ServicioRequest = {
      nombre: 'Vacuna',
      precio: 30,
      duracionEstimadaMin: 15,
      activo: true
    };
    const mockRespuesta: Servicio = { idServicio: 2, ...nuevo };

    service.crear(nuevo).subscribe(data => {
      expect(data).toEqual(mockRespuesta);
    });

    const req = httpTesting.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(nuevo);
    req.flush(mockRespuesta);
  });

  it('should update a service via PUT', () => {
    const dto: ServicioRequest = {
      nombre: 'Consulta Avanzada',
      precio: 75,
      duracionEstimadaMin: 45,
      activo: true
    };
    const mockRespuesta: Servicio = { idServicio: 1, ...dto };

    service.actualizar(1, dto).subscribe(data => {
      expect(data).toEqual(mockRespuesta);
    });

    const req = httpTesting.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush(mockRespuesta);
  });
});
