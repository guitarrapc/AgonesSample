#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# install libc-dev to avoid gRPC Execution Error.
# Unhandled exception. System.DllNotFoundException:
# Unable to load shared library 'libdl.so' or one of its dependencies.
# In order to help diagnose loading problems, consider setting the LD_DEBUG environment variable:
# liblibdl.so: cannot open shared object file: No such file or directory
RUN apt update && apt install -y --no-install-recommends libc-dev

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SimpleServer/SimpleServer.csproj", "src/SimpleServer/"]
COPY ["src/SimpleShared/SimpleShared.csproj", "src/SimpleShared/"]
RUN dotnet restore "src/SimpleServer/SimpleServer.csproj"
COPY . .
WORKDIR "/src/src/SimpleServer"
RUN dotnet build "SimpleServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleServer.dll"]
