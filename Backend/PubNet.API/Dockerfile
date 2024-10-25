ARG SDK_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${SDK_VERSION} AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Directory.Build.props", "/"]
COPY ["Directory.Packages.props", "/"]
COPY ["Backend/PubNet.API/PubNet.API.csproj", "Backend/PubNet.API/"]

RUN dotnet restore "Backend/PubNet.API/PubNet.API.csproj"

COPY ../ .

WORKDIR "/src/Backend/PubNet.API"

RUN dotnet tool restore
RUN dotnet build "PubNet.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# git is only required for building
RUN rm -rf /src/.git && rm -rf /src/.gitignore

RUN dotnet publish "PubNet.API.csproj" --no-build --nologo -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PubNet.API.dll"]
