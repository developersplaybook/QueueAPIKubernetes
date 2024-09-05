@echo off
echo Building Docker image for Server...
docker build -t server:latest-k8s -f Server/Dockerfile .

echo Building Docker image for Client...
docker build -t client:latest-k8s -f Client/Dockerfile .

echo Building Docker image for CronJobManager...
docker build -t cronjobmanager:latest-k8s -f CronJobManager/Dockerfile .

echo Docker images have been built.
pause
