#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5012
EXPOSE 5017

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SimpleBackend/SimpleBackend.csproj", "src/SimpleBackend/"]
COPY ["src/SimpleBackendApi/SimpleBackendApi.csproj", "src/SimpleBackendApi/"]
COPY ["src/SimpleBackendGrpc/SimpleBackendGrpc.csproj", "src/SimpleBackendGrpc/"]
COPY ["src/SimpleShared/SimpleShared.csproj", "src/SimpleShared/"]
RUN dotnet restore "src/SimpleBackend/SimpleBackend.csproj"
COPY . .
WORKDIR "/src/src/SimpleBackend"
RUN dotnet build "SimpleBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleBackend.dll"]
