#!/bin/bash

# Script para manejar el contenedor SQL Server

case "$1" in
    "up")
        echo "🚀 Levantando SQL Server..."
        docker-compose up -d
        echo "⏳ Esperando que SQL Server esté listo..."
        sleep 30
        echo "✅ SQL Server disponible en localhost:1433"
        echo "🔐 Usuario: sa | Password: YourStrong!Passw0rd"
        ;;
    "down")
        echo "🛑 Deteniendo SQL Server..."
        docker-compose down
        echo "✅ Contenedor detenido"
        ;;
    "restart")
        echo "🔄 Reiniciando SQL Server..."
        docker-compose down
        docker-compose up -d
        sleep 30
        echo "✅ SQL Server reiniciado"
        ;;
    "logs")
        echo "📋 Mostrando logs de SQL Server..."
        docker-compose logs -f sqlserver
        ;;
    "status")
        echo "📊 Estado del contenedor:"
        docker-compose ps
        ;;
    "clean")
        echo "🧹 Limpiando datos de SQL Server..."
        docker-compose down -v
        echo "⚠️  Todos los datos han sido eliminados"
        ;;
    *)
        echo "📖 Uso: $0 {up|down|restart|logs|status|clean}"
        echo ""
        echo "Comandos disponibles:"
        echo "  up      - Levantar SQL Server"
        echo "  down    - Detener SQL Server"
        echo "  restart - Reiniciar SQL Server"
        echo "  logs    - Ver logs del contenedor"
        echo "  status  - Ver estado del contenedor"
        echo "  clean   - Eliminar datos y contenedor"
        ;;
esac