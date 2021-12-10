set version=0.7
docker build -t guitarrapc/agonessample-simpleserver:%version% -f src/SimpleServer/Dockerfile .
docker build -t guitarrapc/agonessample-simplefrontend:%version% -f src/simplefrontend/Dockerfile .
docker push guitarrapc/agonessample-simpleserver:%version%
docker push guitarrapc/agonessample-simplefrontend:%version%
