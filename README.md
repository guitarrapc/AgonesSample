# AgonesSample

* [Server image on DockerHub](https://hub.docker.com/repository/docker/guitarrapc/agonessample-simpleserver) | [Dockerfile](https://github.com/guitarrapc/AgonesSample/blob/main/src/SimpleServer/Dockerfile)
* [FrontEnd image on DockerHub](https://hub.docker.com/repository/docker/guitarrapc/agonessample-simplefrontend) | [Dockerfile](https://github.com/guitarrapc/AgonesSample/blob/main/src/SimpleFrontEnd/Dockerfile)

# Prerequisites

Install Agones, before running on Kubernetes.

```shell
helm upgrade --install agones agones/agones --version 1.19.0 --namespace agones-system --create-namespace
```

# Install

Try on Kubernetes. Use [k8s/deployment.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/k8s/deployment.yaml).

```shell
kubectl apply -f ./k8s/deployment.yaml
```

To allocate Gameserver, use [k8s/allocation.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/k8s/allocation.yaml).

```shell
kubectl apply -f ./k8s/allocation.yaml
```

# Clean up

Clean kubernetes resources.

```shell
kubectl delete -f ./k8s/deployment.yaml
```

# Note

Run on local via VisualStudio or docker-compose.

```
docker compose up --build
```
