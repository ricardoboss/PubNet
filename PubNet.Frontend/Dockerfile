FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PubNet.Frontend/PubNet.Frontend.csproj", "PubNet.Frontend/"]
RUN dotnet restore "PubNet.Frontend/PubNet.Frontend.csproj"
COPY . .
WORKDIR "/src/PubNet.Frontend"
RUN dotnet build "PubNet.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.Frontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Frontend.dll"]
