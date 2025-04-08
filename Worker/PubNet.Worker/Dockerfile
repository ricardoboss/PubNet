ARG SDK_VERSION=9.0

FROM mcr.microsoft.com/dotnet/runtime:${SDK_VERSION} AS base
USER $APP_UID
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ../ .

WORKDIR "/src/Worker/PubNet.Worker"

RUN dotnet tool restore
RUN dotnet publish "PubNet.Worker.csproj" --nologo -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final

USER root

RUN apt-get update && apt-get install git curl unzip -y

USER $APP_UID

WORKDIR /flutter
RUN git clone https://github.com/flutter/flutter.git -b stable flutter
ENV PATH="/flutter/flutter/bin/:$PATH"

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Worker.dll"]
