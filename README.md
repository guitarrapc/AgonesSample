# AgonesSample

# Prerequisites

Install Agones, before running on Kubernetes.

```shell
helm upgrade --install agones agones/agones --version 1.19.0 --namespace agones-system --create-namespace
```

# Install

Now try on Kubernetes.

```shell
kubectl apply -f ./k8s/deployment.yaml
```

# Clean up

Clean kubernetes resources.

```shell
kubectl delete -f ./k8s/deployment.yaml
```

# Note

Build Docker image and push.

```shell
push.bat
```

Run on local.

```
docker compose up --build
```
