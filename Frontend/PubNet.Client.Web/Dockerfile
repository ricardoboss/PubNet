ARG SDK_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${SDK_VERSION} AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ../ .

WORKDIR "/src/Frontend/PubNet.Client.Web"

RUN dotnet tool restore
RUN dotnet publish "PubNet.Client.Web.csproj" --nologo -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Client.Web.dll"]
