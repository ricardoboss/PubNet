ARG SDK_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${SDK_VERSION} AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
WORKDIR /src
COPY ["Web/PubNet.Frontend/PubNet.Frontend.csproj", "Web/PubNet.Frontend/"]
RUN dotnet restore "Web/PubNet.Frontend/PubNet.Frontend.csproj"
COPY . .
WORKDIR "/src/Web/PubNet.Frontend"
RUN dotnet build "PubNet.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.Frontend.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ./Web/PubNet.Frontend/nginx.conf /etc/nginx/nginx.conf
