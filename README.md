# boinc-manager
.NET Core implementation of a cross-platform BOINC Manager

## Docker
[![](https://images.microbadger.com/badges/version/adamradocz/boinc-manager-web.svg)](https://microbadger.com/images/adamradocz/boinc-manager-web "Get your own version badge on microbadger.com")
[![](https://images.microbadger.com/badges/image/adamradocz/boinc-manager-web.svg)](https://microbadger.com/images/adamradocz/boinc-manager-web "Get your own image badge on microbadger.com")
![Docker Pulls](https://img.shields.io/docker/pulls/adamradocz/boinc-manager-web.svg)
![Docker Stars](https://img.shields.io/docker/stars/adamradocz/boinc-manager-web.svg)
![Docker Cloud Build Status](https://img.shields.io/docker/cloud/build/adamradocz/boinc-manager-web.svg)

### BoincManagerWeb Usage

The following command runs the BoincManagerWeb Docker container:

```
docker run -d \
  --name boinc-manager-web \
  --net=host \
  -v /opt/appdata/boinc-manager-web:/app/BoincManager \
  adamradocz/boinc-manager-web
```

### Application Setup
Access the webui at `https://your-ip`

### Docker Compose
You can create the following `docker-compose.yml` file and from within the same directory run the client with `docker-compose up -d` to avoid the longer command from above. 
```
version: '2'
services:

  boinc-manager-web:
    image: adamradocz/boinc-manager-web
    container_name: boinc-manager-web
    restart: always
    network_mode: host
    volumes:
      - /opt/appdata/boinc-manager-web:/app/BoincManager
```

### More Info
- How to build it yourself: `docker build -t boinc-manager-web .`
- Shell access whilst the container is running: `docker exec -it boinc-manager-web /bin/bash`
- Monitor the logs of the container in realtime: `docker logs -f boinc-manager-web`

## Development requirements
- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/preview/)
- [.NET Core 3.0 Runtime and SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)
