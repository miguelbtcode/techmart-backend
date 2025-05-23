# TechMart E-commerce Backend

🚀 **Plataforma de E-commerce Moderna con Arquitectura de Microservicios**

## 🏗️ Arquitectura

TechMart utiliza una arquitectura de microservicios moderna construida con:

- **.NET 8** - Framework principal
- **PostgreSQL** - Base de datos principal
- **Redis** - Cache distribuido
- **RabbitMQ** - Message bus
- **Elasticsearch** - Motor de búsqueda
- **Docker** - Containerización

## 🛠️ Microservicios

| Servicio | Puerto | Descripción |
|----------|--------|-------------|
| **API Gateway** | 7000 | Punto de entrada único |
| **Auth Service** | 7001 | Autenticación y autorización |
| **Product Service** | 7002 | Catálogo de productos |
| **Order Service** | 7003 | Gestión de órdenes |
| **Payment Service** | 7004 | Procesamiento de pagos |
| **Notification Service** | 7005 | Notificaciones |
| **Analytics Service** | 7006 | Métricas y reportes |

## 🚀 Inicio Rápido

### Prerrequisitos
- Docker & Docker Compose
- .NET 8 SDK
- Git

### Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/tu-usuario/techmart-backend.git
cd techmart-backend
```

2. **Levantar infraestructura**
```bash
docker-compose up -d
```

3. **Ejecutar migraciones**
```bash
./scripts/run-migrations.sh
```

4. **Ejecutar servicios**
```bash
./scripts/start-services.sh
```

## 📋 Variables de Entorno

Copia `.env.example` a `.env` y configura:

```bash
# Database
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_PASSWORD=Password123!

# Redis
REDIS_HOST=localhost
REDIS_PORT=6379

# RabbitMQ
RABBITMQ_HOST=localhost
RABBITMQ_USER=admin
RABBITMQ_PASSWORD=Password123!

# JWT
JWT_SECRET=your-super-secret-key
JWT_ISSUER=TechMart
JWT_AUDIENCE=TechMart-API
```

## 🧪 Testing

```bash
# Unit Tests
dotnet test tests/UnitTests/

# Integration Tests
dotnet test tests/IntegrationTests/

# E2E Tests
dotnet test tests/E2ETests/
```

## 📊 Monitoreo

| Herramienta | URL | Credenciales |
|-------------|-----|-------------|
| **Grafana** | http://localhost:3000 | admin/admin123 |
| **Jaeger** | http://localhost:16686 | - |
| **Seq** | http://localhost:5340 | - |
| **RabbitMQ** | http://localhost:15672 | admin/Password123! |

## 🔄 GitFlow

Utilizamos GitFlow para el manejo de branches:

- `main` - Producción
- `develop` - Desarrollo
- `feature/*` - Nuevas características
- `release/*` - Preparación de releases
- `hotfix/*` - Fixes críticos

## 📚 Documentación

- [API Documentation](docs/API.md)
- [Architecture Guide](docs/Architecture.md)
- [Deployment Guide](docs/Deployment.md)

## 🤝 Contribución

1. Fork el proyecto
2. Crear branch de feature (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Crear Pull Request

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver [LICENSE](LICENSE) para detalles.

## 👥 Equipo

- **Senior .NET React Developer** - Desarrollador Principal

---

⭐ Si este proyecto te ayuda, ¡dale una estrella!
