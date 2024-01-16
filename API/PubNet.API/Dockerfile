FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PubNet.API/PubNet.API.csproj", "PubNet.API/"]
RUN dotnet restore "PubNet.API/PubNet.API.csproj"
COPY . .
WORKDIR "/src/PubNet.API"
RUN dotnet build "PubNet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.API.dll"]
