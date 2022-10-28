set version=0.7
docker build -t guitarrapc/agonessample-simplebackend:%version% -f src/SimpleBackend/Dockerfile .
docker build -t guitarrapc/agonessample-simplefrontend:%version% -f src/simplefrontend/Dockerfile .
docker push guitarrapc/agonessample-simplebackend:%version%
docker push guitarrapc/agonessample-simplefrontend:%version%
