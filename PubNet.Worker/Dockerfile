FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PubNet.Worker/PubNet.Worker.csproj", "PubNet.Worker/"]
RUN dotnet restore "PubNet.Worker/PubNet.Worker.csproj"
COPY . .
WORKDIR "/src/PubNet.Worker"
RUN dotnet build "PubNet.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.Worker.csproj" -c Release -o /app/publish

FROM base AS final

COPY --from=dart:stable /usr/lib/dart/ /usr/lib/dart/
COPY --from=dart:stable /runtime/ /runtime/
ENV PATH="/usr/lib/dart/bin:$PATH"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Worker.dll"]
