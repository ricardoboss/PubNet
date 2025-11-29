FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ["src/PubNet.API/PubNet.API.csproj", "src/PubNet.API/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
RUN dotnet restore "src/PubNet.API/PubNet.API.csproj"
COPY . .
WORKDIR "/build/src/PubNet.API"
RUN dotnet build "PubNet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.API.dll"]
