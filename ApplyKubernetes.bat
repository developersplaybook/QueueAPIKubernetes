kubectl apply -f sql-server-deployment.yaml
kubectl apply -f sql-server-service.yaml
kubectl apply -f client-deployment.yaml
kubectl apply -f client-service.yaml
kubectl apply -f cronjobmanager-deployment.yaml
kubectl apply -f cronjobmanager-service.yaml
kubectl apply -f server-service.yaml
kubectl apply -f server-deployment.yaml
