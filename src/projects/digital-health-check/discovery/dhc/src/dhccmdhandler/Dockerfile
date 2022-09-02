FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS buildenv
WORKDIR /src

FROM buildenv as restorenuget
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
#RUN find . -type f -name *.csproj
RUN for file in $(find . -type f -name *.csproj); do dotnet restore $file; done

FROM restorenuget as build
ARG project="dhccmdhandler"
COPY . .
RUN dotnet build ${project}/${project}.csproj -c Release -o /app/build

FROM build AS publish
ARG project
RUN dotnet publish ${project}/${project}.csproj -c Release -o /app/publish  --no-restore /p:UseAppHost=false

FROM base AS final
ARG project="dhccmdhandler"
ENV project_env=${project}
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet $(echo ${project_env}).dll