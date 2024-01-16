FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PubNet.Frontend/PubNet.Frontend.csproj", "PubNet.Frontend/"]
RUN dotnet restore "PubNet.Frontend/PubNet.Frontend.csproj"
COPY . .
WORKDIR "/src/PubNet.Frontend"
RUN dotnet build "PubNet.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.Frontend.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ./PubNet.Frontend/nginx.conf /etc/nginx/nginx.conf
