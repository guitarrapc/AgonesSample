#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV DOTNET_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SimpleFrontEnd/SimpleFrontEnd.csproj", "src/SimpleFrontEnd/"]
COPY ["src/SimpleShared/SimpleShared.csproj", "src/SimpleShared/"]
RUN dotnet restore "src/SimpleFrontEnd/SimpleFrontEnd.csproj"
COPY . .
WORKDIR "/src/src/SimpleFrontEnd"
RUN dotnet build "SimpleFrontEnd.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleFrontEnd.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleFrontEnd.dll"]