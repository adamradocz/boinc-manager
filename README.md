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

```sh
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
```yaml
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
```yaml
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


# Development

## Development requirements
- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/)
- [.NET Core 3.0 Runtime and SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)


## Project Structure
| Project | Info |
| :--- | :--- |
| BoincManager | .NET Standard Standard. This is the Core project, contains all of the Common code, Business logic, Models, DbContext, Interfaces. |
| BoincManagerMobile | Xamarin Core (.NET Standard Library) project. Contains the re-usable code in one place, to be shared across the mobile platforms. |
| BoincManagerMobile.Android | Xamarin Android-specific application project. Consume the re-usable code with as little coupling as possible. Platform-specific features are added at this level, built on components exposed in the Core project. |
| BoincManagerMobile.iOS | Xamarin iOS-specific application project. Consume the re-usable code with as little coupling as possible. Platform-specific features are added at this level, built on components exposed in the Core project. |
| BoincManagerWeb | ASP.NET Core implementation of the Boinc Manager. |
| BoincManagerWindows | .NET Core WPF implementation of the Boinc Manager for Windows. |
| BoincRpc | Boinc RPC .NET Standard Library. The BoincManager uses it for making connection with the BOINC Client. |



## BoincManagerWeb Development


### How to add HTTPS Certificate
Info about [Hosting ASP.NET Core Images with Docker over HTTPS](https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https.md)

Environment variables have to be set:
- `ASPNETCORE_URLS`
- `ASPNETCORE_Kestrel__Certificates__Default__Path`
- `ASPNETCORE_Kestrel__Certificates__Default__Password`
- `ASPNETCORE_HTTPS_PORT` (Optional)

Example:
```yaml
  boinc-manager-web:
    image: adamrdocz/boinc-manager-web
    container_name: boinc-manager-web
    restart: always
    environment:
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/privkey.pfx
      - ASPNETCORE_Kestrel__Certificates__Default__Password=password
    ports:
      - "8000:80"
      - "8001:443"
    volumes:
      - /opt/appdata/boinc-manager-web:/app/BoincManager
      - /privkeylocation:/https

```