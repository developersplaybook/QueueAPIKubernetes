# QueueAPIKubernetes

QueueAPIKubernetes is a Kubernetes-based implementation of a Queue API. This project orchestrates several services including SQL Server, a client API, and a cron job manager using Kubernetes.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Kubernetes Setup](#kubernetes-setup)
- [Configuration](#configuration)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Prerequisites

Before starting, ensure you have the following installed:

- [Docker Desktop with Kubernetes enabled](https://www.docker.com/products/docker-desktop)
- [kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/)
- [Kubernetes-kompose](https://kompose.io/)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/gunnarsireus/QueueAPIKubernetes.git
   cd QueueAPIKubernetes

2. Build Docker images:

    ```bash
    docker-compose build

## Kubernetes Setup

1. Deploy SQL Server:

    ```bash
    kubectl apply -f k8s/sqlserver-deployment.yaml
    kubectl apply -f k8s/sqlserver-service.yaml

2. Deploy the client API and cron job manager:

    ```bash
    kubectl apply -f k8s/client-deployment.yaml
    kubectl apply -f k8s/client-service.yaml
    kubectl apply -f k8s/cronjobmanager-deployment.yaml

## Configuration

- Update the connection strings in the Kubernetes YAML files (e.g., `sqlserver-deployment.yaml`, `client-deployment.yaml`) to reflect your actual SQL Server instance name.
- Ensure all Kubernetes resources like `configmaps` and `secrets` are correctly set up to match your environment.
- Modify any necessary environment variables within the YAML files to fit your deployment needs.
## Usage

To check the status of your deployments:

    ```bash
    kubectl get pods
    kubectl get services

To access the API, use the service's external IP or node port, depending on your Kubernetes setup.

For example, if using a NodePort service:

    ```bash
    kubectl get svc <service-name>

Then navigate to the exposed IP and port in your browser or API client.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork this repository.
2. Create a new branch for your feature or bugfix.
3. Commit your changes and push the branch to your fork.
4. Open a Pull Request on the original repository.

Please ensure your code adheres to the project's coding standards and includes relevant tests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
