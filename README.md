# AgonesSample

* [guitarrapc/agonessample-simpleserver](https://hub.docker.com/r/guitarrapc/agonessample-simpleserver) | [Dockerfile](https://github.com/guitarrapc/AgonesSample/blob/main/src/SimpleServer/Dockerfile)
* [guitarrapc/agonessample-simplefrontend](https://hub.docker.com/r/guitarrapc/agonessample-simplefrontend) | [Dockerfile](https://github.com/guitarrapc/AgonesSample/blob/main/src/SimpleFrontEnd/Dockerfile)

# Prerequisites

Install Agones. Options set Allocator non-TLS.

```shell
helm upgrade --install agones agones/agones --version 1.22.0 --namespace agones-system --create-namespace --set agones.allocator.service.http.port=8443 --set agones.allocator.service.grpc.enabled=false --set agones.allocator.disableTLS=true
```

# Install

Try on Kubernetes. Use [examples/k8s/deployment.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/examples/k8s/deployment.yaml).

```shell
kubectl apply -f ./examples/k8s/deployment.yaml
```

To allocate Gameserver, use [examples/k8s/allocation.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/examples/k8s/allocation.yaml).

```shell
kubectl apply -f ./examples/k8s/allocation.yaml
```

# Clean up

Clean kubernetes resources.

```shell
kubectl delete -f ./k8s/deployment.yaml
```

# Note

Run docker-compose to emulate AgonesSDK. Use [examples/docker/compose.yaml](https://github.com/guitarrapc/AgonesSample/blob/main/examples/docker/compose.yaml).

```yaml
services:
  frontend:
    image: agonessample-simplefrontend:v0.9.0
    ports:
      - 5104:80

  server:
    image: agonessample-simpleserver:v0.9.0
    ports:
      - 5157:80
```

Then run docker-compose.

```shell
docker compose up --build
```
