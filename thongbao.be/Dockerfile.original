# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["thongbao.be/thongbao.be.csproj", "thongbao.be/"]
COPY ["thongbao.be.application/thongbao.be.application.csproj", "thongbao.be.application/"]
COPY ["thongbao.be.infrastructure.data/thongbao.be.infrastructure.data.csproj", "thongbao.be.infrastructure.data/"]
COPY ["thongbao.be.domain/thongbao.be.domain.csproj", "thongbao.be.domain/"]
COPY ["thongbao.be.shared/thongbao.be.shared.csproj", "thongbao.be.shared/"]
COPY ["thongbao.be.infrastructure.external/thongbao.be.infrastructure.external.csproj", "thongbao.be.infrastructure.external/"]
RUN dotnet restore "./thongbao.be/thongbao.be.csproj"
COPY . .
WORKDIR "/src/thongbao.be"
RUN dotnet build "./thongbao.be.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./thongbao.be.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "thongbao.be.dll"]