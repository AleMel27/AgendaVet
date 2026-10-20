using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AgendaVet.Api
{
    public static class DbInitializer
    {
        public static void Initialize(AgendaVetDbContext context)
        {
            // 1. Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Rol { IdRol = 1, Nombre = "Administrador", Descripcion = "Acceso total a la plataforma" },
                    new Rol { IdRol = 2, Nombre = "Veterinario", Descripcion = "Atención médica y expedientes" },
                    new Rol { IdRol = 3, Nombre = "Recepcionista", Descripcion = "Gestión de citas y clientes" }
                );
                context.SaveChanges();
            }

            // 2. Usuarios
            if (!context.Usuarios.Any())
            {
                context.Usuarios.AddRange(
                    new Usuario
                    {
                        IdRol = 1,
                        Nombre = "Carlos",
                        Apellido = "Administrador",
                        Email = "admin@agendavet.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                        Telefono = "999888777",
                        Activo = true,
                        FechaRegistro = DateTime.Now
                    },
                    new Usuario
                    {
                        IdRol = 2,
                        Nombre = "Alejandro",
                        Apellido = "Morales",
                        Email = "vet@agendavet.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Vet123*"),
                        Telefono = "988777666",
                        Activo = true,
                        FechaRegistro = DateTime.Now
                    }
                );
                context.SaveChanges();
            }

            // 3. Especies
            if (!context.Especies.Any())
            {
                context.Especies.AddRange(
                    new Especie { IdEspecie = 1, Nombre = "Canino" },
                    new Especie { IdEspecie = 2, Nombre = "Felino" },
                    new Especie { IdEspecie = 3, Nombre = "Ave" },
                    new Especie { IdEspecie = 4, Nombre = "Roedor" }
                );
                context.SaveChanges();
            }

            // 4. Razas
            if (!context.Razas.Any())
            {
                context.Razas.AddRange(
                    new Raza { IdEspecie = 1, Nombre = "Golden Retriever" },
                    new Raza { IdEspecie = 1, Nombre = "Bulldog Francés" },
                    new Raza { IdEspecie = 1, Nombre = "Poodle / Caniche" },
                    new Raza { IdEspecie = 1, Nombre = "Pastor Alemán" },
                    new Raza { IdEspecie = 2, Nombre = "Siamés" },
                    new Raza { IdEspecie = 2, Nombre = "Persa" },
                    new Raza { IdEspecie = 2, Nombre = "Común Europeo" }
                );
                context.SaveChanges();
            }

            // 5. Servicios
            if (!context.Servicios.Any())
            {
                context.Servicios.AddRange(
                    new Servicio
                    {
                        Nombre = "Consulta General",
                        Descripcion = "Examen clínico integral de rutina",
                        Precio = 50.00m,
                        DuracionEstimadaMin = 30,
                        Activo = true
                    },
                    new Servicio
                    {
                        Nombre = "Vacunación Séxtuple / Antirrábica",
                        Descripcion = "Inmunización preventiva",
                        Precio = 75.00m,
                        DuracionEstimadaMin = 20,
                        Activo = true
                    },
                    new Servicio
                    {
                        Nombre = "Desparasitación Interna / Externa",
                        Descripcion = "Tratamiento antiparasitario completo",
                        Precio = 35.00m,
                        DuracionEstimadaMin = 15,
                        Activo = true
                    },
                    new Servicio
                    {
                        Nombre = "Profilaxis Dental",
                        Descripcion = "Limpieza dental por ultrasonido",
                        Precio = 120.00m,
                        DuracionEstimadaMin = 45,
                        Activo = true
                    }
                );
                context.SaveChanges();
            }

            // 6. Propietario inicial
            if (!context.Propietarios.Any())
            {
                context.Propietarios.AddRange(
                    new Propietario
                    {
                        Nombre = "Carlos",
                        Apellido = "Mendoza",
                        DniCe = "74859612",
                        Telefono = "987654321",
                        Email = "carlos.mendoza@email.com",
                        Direccion = "Av. Las Palmeras 123, Miraflores",
                        FechaRegistro = DateTime.Now
                    },
                    new Propietario
                    {
                        Nombre = "Ana",
                        Apellido = "Torres",
                        DniCe = "45879632",
                        Telefono = "912345678",
                        Email = "ana.torres@email.com",
                        Direccion = "Calle Los Pinos 456, San Isidro",
                        FechaRegistro = DateTime.Now
                    }
                );
                context.SaveChanges();
            }

            // 7. Paciente inicial
            if (!context.Pacientes.Any())
            {
                var propCarlos = context.Propietarios.First();
                var espCanino = context.Especies.First(e => e.Nombre == "Canino");
                var espFelino = context.Especies.First(e => e.Nombre == "Felino");
                var razaGolden = context.Razas.FirstOrDefault(r => r.Nombre == "Golden Retriever");
                var razaSiames = context.Razas.FirstOrDefault(r => r.Nombre == "Siamés");

                context.Pacientes.AddRange(
                    new Paciente
                    {
                        IdPropietario = propCarlos.IdPropietario,
                        IdEspecie = espCanino.IdEspecie,
                        IdRaza = razaGolden?.IdRaza,
                        Nombre = "Max",
                        Sexo = "Macho",
                        FechaNacimiento = DateTime.Now.AddYears(-2).AddMonths(-4),
                        Color = "Dorado",
                        Esterilizado = true,
                        Observaciones = "Alérgico a medicamentos a base de penicilina",
                        FechaRegistro = DateTime.Now
                    },
                    new Paciente
                    {
                        IdPropietario = propCarlos.IdPropietario,
                        IdEspecie = espFelino.IdEspecie,
                        IdRaza = razaSiames?.IdRaza,
                        Nombre = "Michi",
                        Sexo = "Hembra",
                        FechaNacimiento = DateTime.Now.AddMonths(-10),
                        Color = "Blanco y café",
                        Esterilizado = false,
                        Observaciones = "Dócil en consulta",
                        FechaRegistro = DateTime.Now
                    }
                );
                context.SaveChanges();
            }

            // 8. Cita inicial de prueba para hoy
            if (!context.Citas.Any())
            {
                var pacienteMax = context.Pacientes.FirstOrDefault();
                var vet = context.Usuarios.FirstOrDefault(u => u.IdRol == 2) ?? context.Usuarios.First();
                var servicio = context.Servicios.FirstOrDefault();

                if (pacienteMax != null && servicio != null)
                {
                    var hoy10Am = DateTime.Today.AddHours(10);
                    var hoy15Pm = DateTime.Today.AddHours(15).AddMinutes(30);

                    context.Citas.AddRange(
                        new Cita
                        {
                            IdPaciente = pacienteMax.IdPaciente,
                            IdVeterinario = vet.IdUsuario,
                            IdServicio = servicio.IdServicio,
                            FechaHora = hoy10Am,
                            Motivo = "Control médico de rutina y vacunas",
                            NotasAgendamiento = "El paciente llega puntual",
                            Estado = "Confirmada",
                            FechaCreacion = DateTime.Now
                        },
                        new Cita
                        {
                            IdPaciente = pacienteMax.IdPaciente,
                            IdVeterinario = vet.IdUsuario,
                            IdServicio = servicio.IdServicio,
                            FechaHora = hoy15Pm,
                            Motivo = "Revisión dental y limpieza",
                            NotasAgendamiento = "Requiere evaluación previa",
                            Estado = "Pendiente",
                            FechaCreacion = DateTime.Now
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
