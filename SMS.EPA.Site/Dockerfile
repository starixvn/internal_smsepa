# escape=`
FROM hub.nexdev.net/base/rocketor/buildservice AS builder

ARG TRANSFORM_CONFIG

SHELL ["cmd", "/S", "/C"]

WORKDIR /build

# Copy files *.sln, *.csproj, packages.config to restore nuget packages
COPY *.sln .
COPY SMS.EPA.Resources\*.csproj SMS.EPA.Resources\

COPY SMS.EPA.Core\*.csproj SMS.EPA.Core\
COPY SMS.EPA.Core\packages.config SMS.EPA.Core\

COPY SMS.EPA.Models\*.csproj SMS.EPA.Models\
COPY SMS.EPA.Models\packages.config SMS.EPA.Models\

COPY SMS.EPA.Criteria\*.csproj SMS.EPA.Criteria\
COPY SMS.EPA.Criteria\packages.config SMS.EPA.Criteria\

COPY SMS.EPA.Repositories\*.csproj SMS.EPA.Repositories\
COPY SMS.EPA.Repositories\packages.config SMS.EPA.Repositories\

COPY SMS.EPA.Services\*.csproj SMS.EPA.Services\
COPY SMS.EPA.Services\packages.config SMS.EPA.Services\

COPY SMS.EPA.Site\*.csproj SMS.EPA.Site\
COPY SMS.EPA.Site\packages.config SMS.EPA.Site\

RUN nuget.exe restore SMS.EPA.sln

# Copy source codes
COPY SMS.EPA.Resources\ SMS.EPA.Resources\
COPY SMS.EPA.Core\ SMS.EPA.Core\
COPY SMS.EPA.Models\ SMS.EPA.Models\
COPY SMS.EPA.Criteria\ SMS.EPA.Criteria\
COPY SMS.EPA.Repositories\ SMS.EPA.Repositories\
COPY SMS.EPA.Services\ SMS.EPA.Services\
COPY SMS.EPA.Site\ SMS.EPA.Site\
COPY SonarQube SonarQube\

RUN MSBuild.exe "SMS.EPA.Site\SMS.EPA.Site.csproj" `
    /t:Build;WebPublish /m `
    /p:DebugType=None `
    /p:Configuration=Release `
    /p:WebPublishMethod=FileSystem `
    /p:TransformWebConfigEnabled=False `
    /p:LocalizedBuild=true `
    /p:AutoParameterizationWebConfigConnectionStrings=False `
    /p:MarkWebConfigAssistFilesAsExclude=false `
    /p:publishUrl="C:\Compiled" `
    /consoleloggerparameters:ErrorsOnly

WORKDIR C:\Compiled

# Transform config
RUN echo "%TRANSFORM_CONFIG%" `
    && xcopy "WebConfigs\Web.%TRANSFORM_CONFIG%.config" "Web.config" /Y

FROM hub.nexdev.net/base/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019 AS base

WORKDIR C:\inetpub\wwwroot

COPY --from=builder C:\Compiled .

RUN dir .\bin\

ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]