# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
# Нужно будет убрать в финальной сборке
ENV TZ=Europe/Moscow
# Нужно будет убрать в финальной сборке
RUN apt-get update && \
    apt-get install -y tzdata && \
    ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && \
    echo $TZ > /etc/timezone && \
    apt-get clean
USER app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Renoza.Backend/Renoza.Backend.csproj", "Renoza.Backend/"]
COPY ["Renoza.Domain/Renoza.Domain.csproj", "Renoza.Domain/"]
COPY ["Renoza.DbMigration/Renoza.DbMigration.csproj", "Renoza.DbMigration/"]
RUN dotnet restore "Renoza.Backend/Renoza.Backend.csproj"
COPY . .
WORKDIR "/src/Renoza.Backend"
RUN dotnet build "Renoza.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Renoza.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Renoza.Backend.dll"]