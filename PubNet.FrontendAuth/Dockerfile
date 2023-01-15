FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PubNet.FrontendAuth/PubNet.FrontendAuth.csproj", "PubNet.FrontendAuth/"]
RUN dotnet restore "PubNet.FrontendAuth/PubNet.FrontendAuth.csproj"
COPY . .
WORKDIR "/src/PubNet.FrontendAuth"
RUN dotnet build "PubNet.FrontendAuth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.FrontendAuth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.FrontendAuth.dll"]
