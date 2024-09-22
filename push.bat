set version=0.7
docker build -t guitarrapc/agonessample-BackendServer:%version% -f src/BackendServer/Dockerfile .
docker build -t guitarrapc/agonessample-FrontEnd:%version% -f src/FrontEnd/Dockerfile .
docker push guitarrapc/agonessample-BackendServer:%version%
docker push guitarrapc/agonessample-FrontEnd:%version%
