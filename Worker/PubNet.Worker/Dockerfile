ARG SDK_VERSION=8.0

FROM mcr.microsoft.com/dotnet/runtime:${SDK_VERSION} AS base
USER $APP_UID
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Directory.Build.props", "/"]
COPY ["Directory.Packages.props", "/"]
COPY ["Worker/PubNet.Worker/PubNet.Worker.csproj", "Worker/PubNet.Worker/"]

RUN dotnet restore "Worker/PubNet.Worker/PubNet.Worker.csproj"

COPY ../ .

WORKDIR "/src/Worker/PubNet.Worker"

RUN dotnet build "PubNet.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PubNet.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final

USER root

RUN apt-get update && apt-get install git curl unzip -y

USER $APP_UID

WORKDIR /flutter
RUN git clone https://github.com/flutter/flutter.git -b stable flutter
ENV PATH="/flutter/flutter/bin/:$PATH"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Worker.dll"]
