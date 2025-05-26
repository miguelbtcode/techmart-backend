#================================================================
# 📋 CONTENIDO DE start-infrastructure.sh
#================================================================

#!/bin/bash
echo "🚀 Starting TechMart Infrastructure..."

# Verificar si Docker está ejecutándose
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Crear directorios de datos si no existen
mkdir -p data/{postgres,redis,elasticsearch,grafana,prometheus}
mkdir -p logs

echo "📦 Pulling latest images..."
docker-compose pull

echo "🏗️ Starting infrastructure services..."
# Iniciar bases de datos PostgreSQL (una por cada servicio)
docker-compose up -d postgres-auth postgres-product postgres-order postgres-payment postgres-notification postgres-analytics

# Iniciar servicios de cache y message bus
docker-compose up -d redis rabbitmq elasticsearch

echo "⏳ Waiting for services to be ready..."
sleep 30

echo "📊 Starting monitoring stack..."
# Iniciar servicios de monitoreo
docker-compose up -d prometheus grafana seq jaeger

echo "🔧 Starting additional services..."
# Iniciar servicios adicionales
docker-compose up -d kibana pgadmin

echo "✅ Infrastructure started successfully!"
echo ""
echo "🌐 Available services:"
echo "  - PostgreSQL Auth: localhost:5432"
echo "  - PostgreSQL Product: localhost:5433"
echo "  - PostgreSQL Order: localhost:5434"
echo "  - PostgreSQL Payment: localhost:5435"
echo "  - PostgreSQL Notification: localhost:5436"
echo "  - PostgreSQL Analytics: localhost:5437"
echo "  - Redis: localhost:6379"
echo "  - RabbitMQ: localhost:15672 (admin/Password123!)"
echo "  - Elasticsearch: localhost:9200"
echo "  - Kibana: localhost:5601"
echo "  - Grafana: localhost:3000 (admin/admin123)"
echo "  - Seq: localhost:5340"
echo "  - Jaeger: localhost:16686"
echo "  - pgAdmin: localhost:8080 (admin@techmart.com/Password123!)"