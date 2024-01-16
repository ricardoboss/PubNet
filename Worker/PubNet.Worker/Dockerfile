FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PubNet.Worker/PubNet.Worker.csproj", "PubNet.Worker/"]
RUN dotnet restore "PubNet.Worker/PubNet.Worker.csproj"
COPY . .
WORKDIR "/src/PubNet.Worker"
RUN dotnet build "PubNet.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.Worker.csproj" -c Release -o /app/publish

FROM base AS final

RUN apt-get update && apt-get install git curl unzip -y
WORKDIR /flutter
RUN git clone https://github.com/flutter/flutter.git -b stable flutter
ENV PATH="/flutter/flutter/bin/:$PATH"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Worker.dll"]
