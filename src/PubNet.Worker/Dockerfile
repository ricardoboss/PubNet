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

RUN apt-get update && apt-get install -y \
    wget \
    curl \
    gnupg \
    ca-certificates \
    unzip \
    fonts-liberation \
    libappindicator3-1 \
    libasound2 \
    libatk-bridge2.0-0 \
    libatk1.0-0 \
    libcups2 \
    libdbus-1-3 \
    libgdk-pixbuf2.0-0 \
    libnspr4 \
    libnss3 \
    libx11-xcb1 \
    libxcomposite1 \
    libxdamage1 \
    libxrandr2 \
    xdg-utils \
    clang \
    cmake \
    ninja-build \
    pkg-config \
    git \
    xz-utils \
    zip \
    libglu1-mesa \
    --no-install-recommends -y && \
    rm -rf /var/lib/apt/lists/*

# Install Google Chrome
RUN apt-get update && \
    curl -fsSL https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor -o /usr/share/keyrings/google-linux-keyring.gpg && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google-linux-keyring.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list && \
    apt-get update && apt-get install -y google-chrome-stable && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /flutter
RUN git clone https://github.com/flutter/flutter.git -b stable flutter
ENV PATH="/flutter/flutter/bin/:$PATH"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PubNet.Worker.dll"]
