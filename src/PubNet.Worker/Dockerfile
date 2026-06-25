FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ["src/PubNet.Worker/PubNet.Worker.csproj", "src/PubNet.Worker/"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
RUN dotnet restore "src/PubNet.Worker/PubNet.Worker.csproj"
COPY . .
WORKDIR "/build/src/PubNet.Worker"
RUN dotnet build "PubNet.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PubNet.Worker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final

RUN apt-get update && apt-get install \
    clang \
    cmake \
    curl \
    git \
    ninja-build \
    pkg-config \
    unzip \
    xz-utils \
    zip \
    libglu1-mesa \
    -y --no-install-recommends && \
    rm -rf /var/lib/apt/lists/* \

WORKDIR /flutter
RUN git clone https://github.com/flutter/flutter.git -b stable flutter
ENV PATH="/flutter/bin/:$PATH"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Worker.dll"]
