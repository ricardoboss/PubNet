# PubNet

Private pub.dev alternative.

## Hosting

### Using `docker-compose.yml`

Create a `docker-compose.yml` with the following contents:

```yaml
version: '3.9'

volumes:
  postgres_data:
  pubnet_packages:
  caddy_data:
  caddy_config:

services:
  database:
    image: postgres:latest
    restart: always
    environment:
      POSTGRES_USER: "pubnet"
      POSTGRES_PASSWORD: "pubnet"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  backend:
    build:
      dockerfile: ./PubNet.API/Dockerfile
      target: final
    restart: always
    volumes:
      - "./backend-appsettings.json:/app/appsettings.Production.json"
      - "pubnet_packages:/app/packages"
    depends_on:
      - database
      - caddy

  worker:
    build:
      dockerfile: ./PubNet.Worker/Dockerfile
      target: final
    restart: always
    volumes:
      - "./worker-appsettings.json:/app/appsettings.Production.json"
      - "pubnet_packages:/app/packages"
    depends_on:
      - database

  frontend:
    build:
      dockerfile: ./PubNet.Frontend/Dockerfile
    restart: always
    depends_on:
      - backend
      - caddy

  # you can choose any reverse proxy you want, Caddy is not required
  caddy:
    image: caddy
    restart: always
    volumes:
      - "./Caddyfile:/etc/caddy/Caddyfile"
      - "caddy_data:/data"
      - "caddy_config:/config"
    ports:
      - "80:80"
      - "443:443"
```

You can also host the backend and frontend on different ports, and publish them directly, removing the need to configure a reverse proxy.

In case you want a reverse proxy, configure it appropriately (in this case using a Caddyfile):

```Caddyfile
*:80, *:443 {
    reverse_proxy /api/* backend:80
    reverse_proxy /* frontend:80
}
```

Add a `backend-appsettings.json`:

```json
{
  "AllowedOrigins": [
    "http://localhost"
  ],
  "ConnectionStrings": {
    "PubNet": "Host=database;Database=pubnet;Username=pubnet;Password=pubnet"
  },
  "Jwt": {
    "Issuer": "http://localhost",
    "Audience": "http://localhost",
    "SecretKey": "GenerateASecureKey!"
  },
  "PackageStorage": {
    "Path": "./packages"
  }
}

```

Add a `worker-appsettings.json`:

> You could use the same appsettings for the worker and the backend as their options shouldn't interfere.

```json
{
  "ConnectionStrings": {
    "PubNet": "Host=database;Database=pubnet;Username=pubnet;Password=pubnet"
  },
  "PackageStorage": {
    "Path": "./packages",
    "PendingMaxAge": "00:05:00"
  }
}

```

Finally, start your own PubNet using

```bash
docker-compose up -d
```

and access it at `https://localhost`.
