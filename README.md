# boinc-manager
.NET Core implementation of a cross-platform BOINC Manager

## Docker
[![](https://images.microbadger.com/badges/version/adamradocz/boinc-manager-web.svg)](https://microbadger.com/images/adamradocz/boinc-manager-web "Get your own version badge on microbadger.com")
[![](https://images.microbadger.com/badges/image/adamradocz/boinc-manager-web.svg)](https://microbadger.com/images/adamradocz/boinc-manager-web "Get your own image badge on microbadger.com")
![Docker Pulls](https://img.shields.io/docker/pulls/adamradocz/boinc-manager-web.svg)
![Docker Stars](https://img.shields.io/docker/stars/adamradocz/boinc-manager-web.svg)
![Docker Cloud Build Status](https://img.shields.io/docker/cloud/build/adamradocz/boinc-manager-web.svg)

## Supported Architectures and Tags

You can specialize the `boinc/client` image with either of the following tags to use one of the specialized container version instead.

### x86-64
| Tag | Info |
| :--- | :--- |
| [`debian`](Dockerfile) | Debian based BOINC Manager. |
| [`latest`, `alpine`](Dockerfile.alpine) | Alpine based BOINC Manager. |


### ARMv7 32-bit
| Tag | Info |
| :--- | :--- |
| [`arm32v7`](Dockerfile) | Debian based BOINC Manager. |


### ARMv8 64-bit
| Tag | Info |
| :--- | :--- |
| [`arm64v8`](Dockerfile) | Debian based BOINC Manager. |


### BoincManagerWeb Usage

The following command runs the BoincManagerWeb Docker container:

```
docker run -d \
  --name boinc-manager-web \
  -p "8000:80" \
  -v /opt/appdata/boinc-manager-web:/app/BoincManager \
  adamradocz/boinc-manager-web
```

### Application Setup
Access the webui at `http://your-ip:8000`

### Docker Compose
You can create the following `docker-compose.yml` file and from within the same directory run the Manager with `docker-compose up -d` to avoid the longer command from above. 
```
version: "2"
services:

  boinc-manager-web:
    image: adamradocz/boinc-manager-web
    container_name: boinc-manager-web
    restart: always
    ports:
      - "8000:80"
    volumes:
      - /opt/appdata/boinc-manager-web:/app/BoincManager
```

You can run Boinc Client and Manager together with the following `docker-compose.yml` file. For more info about the Boinc Client Docker image check out its [official page](https://hub.docker.com/r/boinc/client)
```
version: "2"
services:

  boinc:
    image: boinc/client
    container_name: boinc
    restart: always
    environment:
      - BOINC_GUI_RPC_PASSWORD=123
      - BOINC_CMD_LINE_OPTIONS=--allow_remote_gui_rpc
    ports:
      - "31416:31416"
    volumes:
      - /opt/appdata/boinc:/var/lib/boinc

  boinc-manager-web:
    image: adamradocz/boinc-manager-web
    container_name: boinc-manager-web
    restart: always
    ports:
      - "8000:80"
    volumes:
     - /opt/appdata/boinc-manager-web:/app/BoincManager
```


### More Info
- How to build it yourself: `docker build -t boinc-manager-web .`
- Shell access whilst the container is running: `docker exec -it boinc-manager-web /bin/bash` or `docker exec -it boinc-manager-web /bin/sh`
- Monitor the logs of the container in realtime: `docker logs -f boinc-manager-web`

## Development

### Development requirements
- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/preview/)
- [.NET Core 3.0 Runtime and SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)

### Project Guide
| Project | Info |
| :--- | :--- |
| BoincManager | .NET Standard Library. Contains the Business logic, all of the Common code, Interfaces, Models, and DbContext |
| BoincManagerMobile | .NET Standard Libray. Contains the majority of the Mobile Code. |
| BoincManagerMobile.Android | Contains the Android specific code only. |
| BoincManagerMobile.iOS | Contains the iOS specific code only. |
| BoincManagerWeb | ASP.NET Core implementation of the Boinc Manager. |
| BoincManagerWindows | .NET Core WPF implementation of the Boinc Manager for Windows. |

### How to add HTTPS Certificate
Info about [Hosting ASP.NET Core Images with Docker over HTTPS](https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https.md)

Environment variables have to be set:
- `ASPNETCORE_URLS`
- `ASPNETCORE_Kestrel__Certificates__Default__Password`
- `ASPNETCORE_Kestrel__Certificates__Default__Path`
- `ASPNETCORE_HTTPS_PORT` (Optional)
