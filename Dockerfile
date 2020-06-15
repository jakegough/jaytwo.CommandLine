FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dotnet-sdk
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine AS dotnet-runtime

FROM dotnet-sdk AS base

RUN apt-get update \
  && apt-get install -y --no-install-recommends \
# Install Make for makefile support
    make \
# Install Mono (for .NET Framework build target)
    mono-devel \
# Install Node (for some unit test cases)
    nodejs \
  && apt-get clean \
  && apt-get autoremove \
  && rm -rf /var/lib/apt/lists/*

# TODO: figure out how to install .net core 1.1 for the tests
#       https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-1.1.14-linux-ubuntu-16.04-x64-binaries

ENV FrameworkPathOverride /usr/lib/mono/4.5/
ENV PATH="${PATH}:/root/.dotnet/tools"

FROM base AS builder
WORKDIR /build
COPY . /build
RUN make deps restore


FROM builder AS publisher
RUN make publish
