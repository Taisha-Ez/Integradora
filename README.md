# API Fenix Vales - Proyecto Integradora

Este proyecto es una API RESTful robusta desarrollada con **.NET 8** utilizando **Clean Architecture**. Está diseñada para gestionar la autenticación de usuarios en **MySQL** (Entity Framework Core) y el registro de logs de auditoría en **MongoDB**.

## Requisitos Previos

* **.NET 8 SDK**
* **MySQL** (Local o XAMPP)
* **MongoDB** (Local o Atlas)
* **EF Core Tools**: Instalar ejecutando `dotnet tool install --global dotnet-ef`

## Configuración de Secretos (IMPORTANTE)

Para que la API funcione correctamente, **NO** se deben poner las credenciales en el `appsettings.json`. Usamos **User Secrets** para evitar errores de formato y proteger los datos.

### Opción A: Desde Visual Studio (Recomendado)
1. Haz clic derecho en el proyecto **TaishaIntegradora**.
2. Selecciona **"Administrar secretos del usuario"** (Manage User Secrets).
3. Pega el siguiente JSON y ajusta tus credenciales (asegúrate de que no haya espacios extras al inicio):

```json
{
  "ConnectionStrings": {
    "MySql": "Server=localhost;Port=3306;Database=appfenixvales;User=root;Password=TU_CONTRASEÑA;",
    "MongoDb": "mongodb://localhost:27017/appfenixvales"
  },
  "Jwt": {
    "Key": "ESTA_ES_UNA_LLAVE_SUPER_SECRETA_Y_LARGA_DE_MAS_DE_32_CARACTERES"
  }
}
```

### Opción B: Desde la Terminal
Si prefieres comandos, ejecuta estos tres en la raíz del proyecto:
```bash
dotnet user-secrets set "ConnectionStrings:MySql" "Server=localhost;Port=3306;Database=appfenixvales;User=root;Password=TU_CONTRASEÑA;"
dotnet user-secrets set "ConnectionStrings:MongoDb" "mongodb://localhost:27017/appfenixvales"
dotnet user-secrets set "Jwt:Key" "ESTA_ES_UNA_LLAVE_SUPER_SECRETA_Y_LARGA_DE_MAS_DE_32_CARACTERES"
```

## Base de Datos (MySQL)

Una vez configurados los secretos, crea la tabla de usuarios automáticamente ejecutando:

```bash
dotnet ef database update
```

## Ejecución del Proyecto

* **Visual Studio**: Presiona `F5` o el botón de reproducir.
* **CLI**: Ejecuta `dotnet run`.

La documentación interactiva se abrirá en: `http://localhost:XXXX/swagger`

## Estructura del Código

* **Application**: Lógica de negocio (Servicios), Interfaces y DTOs.
* **Domain**: Entidades de MySQL y Documentos de MongoDB.
* **Infrastructure**: Repositorios y Contextos de persistencia.
* **Controllers**: Endpoints de la API.

## Endpoints de Autenticación

### Registro
* **POST** `/api/Auth/register`
```json
{
  "nombre": "Nombre Usuario",
  "apellidoPaterno": "Apellido",
  "apellidoMaterno": "Apellido",
  "usuario": "nick_name",
  "contrasenia": "password123",
  "tipoUsuario": "admin"
}
```

### Login
* **POST** `/api/Auth/login`
```json
{
  "usuario": "nick_name",
  "contrasenia": "password123"
}
```
