#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5217

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/SimpleGrpc/SimpleGrpc.csproj", "src/SimpleGrpc/"]
COPY ["src/SimpleShared/SimpleShared.csproj", "src/SimpleShared/"]
RUN dotnet restore "src/SimpleGrpc/SimpleGrpc.csproj"
COPY . .
WORKDIR "/src/src/SimpleGrpc"
RUN dotnet build "SimpleGrpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleGrpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleGrpc.dll"]