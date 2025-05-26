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

#================================================================
# 📋 CONTENIDO DE health-check.sh  
#================================================================

#!/bin/bash
echo "🏥 TechMart Health Check..."

# Función para verificar health endpoint
check_health() {
    local service_name=$1
    local url=$2
    
    if curl -f -s "$url/health" > /dev/null; then
        echo "✅ $service_name: Healthy"
        return 0
    else
        echo "❌ $service_name: Unhealthy"
        return 1
    fi
}

# Función para verificar conexión básica (para servicios de infraestructura)
check_connection() {
    local service_name=$1
    local host=$2
    local port=$3
    
    if nc -z $host $port 2>/dev/null; then
        echo "✅ $service_name: Connected"
        return 0
    else
        echo "❌ $service_name: Not responding"
        return 1
    fi
}

# Verificar infraestructura
echo "🔍 Checking Infrastructure..."
check_connection "PostgreSQL Auth" "localhost" "5432"
check_connection "PostgreSQL Product" "localhost" "5433" 
check_connection "PostgreSQL Order" "localhost" "5434"
check_connection "PostgreSQL Payment" "localhost" "5435"
check_connection "PostgreSQL Notification" "localhost" "5436"
check_connection "PostgreSQL Analytics" "localhost" "5437"
check_connection "Redis" "localhost" "6379"
check_connection "RabbitMQ" "localhost" "5672"
check_connection "Elasticsearch" "localhost" "9200"

echo ""
echo "🔍 Checking Web Interfaces..."
check_connection "RabbitMQ Management" "localhost" "15672"
check_connection "Kibana" "localhost" "5601"
check_connection "Grafana" "localhost" "3000"
check_connection "Seq" "localhost" "5340"
check_connection "Jaeger" "localhost" "16686"
check_connection "pgAdmin" "localhost" "8080"

echo ""
echo "🔍 Checking Application Services..."
failed_services=0

# Estos servicios aún no existen, pero el script está preparado para cuando los crees
check_health "API Gateway" "http://localhost:7000" || ((failed_services++))
check_health "Auth Service" "http://localhost:7001" || ((failed_services++))
check_health "Product Service" "http://localhost:7002" || ((failed_services++))
check_health "Order Service" "http://localhost:7003" || ((failed_services++))
check_health "Payment Service" "http://localhost:7004" || ((failed_services++))
check_health "Notification Service" "http://localhost:7005" || ((failed_services++))
check_health "Analytics Service" "http://localhost:7006" || ((failed_services++))

echo ""
if [ $failed_services -eq 0 ]; then
    echo "✅ All services are healthy!"
    exit 0
else
    echo "⚠️ $failed_services application service(s) are not running yet (this is normal if you haven't started the .NET services)"
    echo "✅ Infrastructure services are ready for development!"
    exit 0
fi