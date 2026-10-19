import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Especie, Raza, Servicio, Propietario, Veterinario } from '../models/catalogo.model';

@Injectable({
  providedIn: 'root'
})
export class CatalogoService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  obtenerEspecies(): Observable<Especie[]> {
    return this.http.get<Especie[]>(`${this.apiUrl}/Especie`);
  }

  obtenerRazasPorEspecie(idEspecie: number): Observable<Raza[]> {
    return this.http.get<Raza[]>(`${this.apiUrl}/Raza/especie/${idEspecie}`);
  }

  obtenerTodasRazas(): Observable<Raza[]> {
    return this.http.get<Raza[]>(`${this.apiUrl}/Raza`);
  }

  obtenerServicios(soloActivos: boolean = true): Observable<Servicio[]> {
    return this.http.get<Servicio[]>(`${this.apiUrl}/Servicio?soloActivos=${soloActivos}`);
  }

  obtenerPropietarios(): Observable<Propietario[]> {
    return this.http.get<Propietario[]>(`${this.apiUrl}/Propietario/todos`);
  }

  obtenerVeterinarios(): Observable<Veterinario[]> {
    return this.http.get<Veterinario[]>(`${this.apiUrl}/Usuario/todos`).pipe(
      map(usuarios =>
        usuarios.filter(
          u => u.activo && (u.idRol === 2 || u.idRol === 1 || u.rolNombre?.toLowerCase().includes('veterinario'))
        )
      )
    );
  }
}
