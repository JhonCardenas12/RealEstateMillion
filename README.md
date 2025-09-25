# RealEstateMillion

Plataforma de gestión inmobiliaria construida con **.NET 8** siguiendo principios de **Clean Architecture** y buenas prácticas de desarrollo.

---

## Tecnologías utilizadas

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core / Dapper**
- **SQL Server 2022** (base de datos)
- **AutoMapper**
- **Serilog** (logging estructurado)
- **NUnit + Moq** (pruebas unitarias)
- **Docker & Docker Compose**

---

## Estructura del proyecto

```bash
RealEstateMillion/
│── RealEstate.Application/      # Capa de aplicación (casos de uso, DTOs, interfaces)
│── RealEstate.Domain/           # Entidades de dominio
│── RealEstate.Infrastructure/   # Persistencia (Dapper/repositorios, servicios)
│── RealEstate.DataBase/         # Scripts SQL, Store Procedures, Backup
│── RealEstate.WebApi/           # API principal (controllers, middleware, startup)
│── RealEstate.Tests/            # Proyecto de pruebas unitarias e integración
│── RealEstate.sln               # Solución principal
│── Dockerfile                   # Imagen de la API
