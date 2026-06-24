FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/PubNet.API/PubNet.API.csproj", "src/PubNet.API/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

RUN dotnet restore "src/PubNet.API/PubNet.API.csproj"

COPY . .
WORKDIR "/src/src/PubNet.API"
RUN dotnet build "PubNet.API.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
WORKDIR /src
COPY --from=build /src .
WORKDIR "/src/src/PubNet.API"
RUN dotnet publish "PubNet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.API.dll"]
