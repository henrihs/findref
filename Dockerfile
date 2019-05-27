FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine

RUN dotnet tool install -g findref

ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /assemblies

ENTRYPOINT ["findref"]
