#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Operation-Service/src/Spirebyte.Services.Operations.API/Spirebyte.Services.Operations.API.csproj", "src/Operation-Service/src/Spirebyte.Services.Operations.API/"]
COPY ["src/Operation-Service/src/Spirebyte.Services.Operations.Infrastructure/Spirebyte.Services.Operations.Infrastructure.csproj", "src/Operation-Service/src/Spirebyte.Services.Operations.Infrastructure/"]
COPY ["src/Operation-Service/src/Spirebyte.Services.Operations.Application/Spirebyte.Services.Operations.Application.csproj", "src/Operation-Service/src/Spirebyte.Services.Operations.Application/"]
COPY ["src/Operation-Service/src/Spirebyte.Services.Operations.Core/Spirebyte.Services.Operations.Core.csproj", "src/Operation-Service/src/Spirebyte.Services.Operations.Core/"]
RUN dotnet restore "src/Operation-Service/src/Spirebyte.Services.Operations.API/Spirebyte.Services.Operations.API.csproj"
COPY . .
WORKDIR "/src/src/Operation-Service/src/Spirebyte.Services.Operations.API"
RUN dotnet build "Spirebyte.Services.Operations.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spirebyte.Services.Operations.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spirebyte.Services.Operations.API.dll"]
