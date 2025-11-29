FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ["src/PubNet.Frontend/PubNet.Frontend.csproj", "src/PubNet.Frontend/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
RUN dotnet restore "src/PubNet.Frontend/PubNet.Frontend.csproj"
COPY . .
WORKDIR "/build/src/PubNet.Frontend"
RUN dotnet build "PubNet.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.Frontend.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ./src/PubNet.Frontend/nginx.conf /etc/nginx/nginx.conf
