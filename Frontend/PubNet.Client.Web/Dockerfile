ARG SDK_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${SDK_VERSION} AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Directory.Build.props", "/"]
COPY ["Directory.Packages.props", "/"]
COPY ["Frontend/PubNet.Client.Web/PubNet.Client.Web.csproj", "Frontend/PubNet.Client.Web/"]

RUN dotnet restore "Frontend/PubNet.Client.Web/PubNet.Client.Web.csproj"

COPY ../ .

WORKDIR "/src/Frontend/PubNet.Client.Web"

RUN dotnet tool restore
RUN dotnet build "PubNet.Client.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# git is only required for building
RUN rm -rf /src/.git && rm -rf /src/.gitignore

RUN dotnet publish "PubNet.Client.Web.csproj" --no-build --nologo -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Client.Web.dll"]
