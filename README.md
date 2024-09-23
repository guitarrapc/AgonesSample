# AgonesSample

* [guitarrapc/agonessample-BackendServer](https://hub.docker.com/r/guitarrapc/agonessample-BackendServer) | [Dockerfile](https://github.com/guitarrapc/AgonesSample/blob/main/src/BackendServer/Dockerfile)
* [guitarrapc/agonessample-FrontEnd](https://hub.docker.com/r/guitarrapc/agonessample-FrontEnd) | [Dockerfile](https://github.com/guitarrapc/AgonesSample/blob/main/src/FrontEnd/Dockerfile)

# Prerequisites

Install Agones. Options set Allocator non-TLS.

```shell
helm repo add agones https://agones.dev/chart/stable
helm repo update
helm upgrade --install agones agones/agones --version 1.43.0 --namespace agones-system --create-namespace --set "gameservers.namespaces={default}" --set agones.allocator.service.http.port=8443 --set agones.allocator.service.grpc.enabled=false --set agones.allocator.disableTLS=true
```

# Install

Try on Kubernetes. Use [examples/k8s/deployment.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/examples/k8s/deployment.yaml).

```shell
kubectl apply -f ./examples/k8s/deployment.yaml
```

Now server and up and running on http://localhost

To allocate Gameserver, you have 2 way.

1. Use allocate through Kubernetes API. Use [examples/k8s/allocation.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/examples/k8s/allocation.yaml).

```shell
kubectl apply -f ./examples/k8s/allocation.yaml
```

2. Use allocation API.

```shell
http://localhost:5104/allocate
```

# Clean up

Clean kubernetes resources.

```shell
kubectl delete -f ./examples/k8s/deployment.yaml
helm uninstall agones -n agones-system
```

# Note

Run docker-compose to emulate AgonesSDK. Use [examples/docker/compose.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/examples/docker/compose.yaml).

```yaml
services:
  frontend:
    image: agonessample-frontendpage:v3.0.0
    ports:
      - 5104:8080

  server:
    image: agonessample-backendserver:v3.0.0
    ports:
      - 5157:5157
    restart: on-failure:1 # emulate new pod when shutdown
```

Then run docker-compose.

```shell
docker compose up --build
```
