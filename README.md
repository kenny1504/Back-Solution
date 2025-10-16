# Banking Tech Challenge (.NET 8 + Angular 17)

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
- La base de datos usa `EnsureCreated` + **seed** de ejemplo.
- Endpoints principales en `/api/v1`: clientes, cuentas, movimientos, transferencias.
- CORS permitido para `http://localhost:4200`.
