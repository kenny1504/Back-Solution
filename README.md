# Banking Tech Challenge (.NET 8 + Angular 17)

<img width="774" height="897" alt="image" src="https://github.com/user-attachments/assets/3aab290e-0deb-42be-8d7b-a94ca89a9501" />

<img width="843" height="912" alt="image" src="https://github.com/user-attachments/assets/dcfba90d-1c49-4482-86e9-b5773dec732b" />

## Requisitos
- Docker (opcional) o
- .NET 8 SDK y Node 20 + Angular CLI 17

## Ejecutar con Docker
```bash
docker compose up -d
# API: http://localhost:8080/swagger
# Web: http://localhost:4200
```

## Ejecutar local (sin Docker)
### Backend
```bash
cd backend/src/Banking.Api
dotnet restore
dotnet run
# API: http://localhost:5080/swagger (o el puerto que indique)
```
### Frontend
```bash
cd frontend/banking-app
npm install
npm start
# App: http://localhost:4200
```

### Notas
- La base de datos usa `EnsureCreated`.
- Endpoints principales en `/api/v1`: clientes, cuentas, movimientos, transferencias.
- CORS permitido para `http://localhost:4200`.
