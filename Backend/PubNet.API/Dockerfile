ARG SDK_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${SDK_VERSION} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
WORKDIR /src
COPY ["Web/PubNet.API/PubNet.API.csproj", "Web/PubNet.API/"]
RUN dotnet restore "Web/PubNet.API/PubNet.API.csproj"
COPY ../ .
WORKDIR "/src/Web/PubNet.API"
RUN dotnet build "PubNet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.API.dll"]
