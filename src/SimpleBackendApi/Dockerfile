#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5012

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SimpleBackendApi/SimpleBackendApi.csproj", "src/SimpleBackendApi/"]
RUN dotnet restore "src/SimpleBackendApi/SimpleBackendApi.csproj"
COPY . .
WORKDIR "/src/src/SimpleBackendApi"
RUN dotnet build "SimpleBackendApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleBackendApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleBackendApi.dll"]
