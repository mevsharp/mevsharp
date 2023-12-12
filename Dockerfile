FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 as build

ARG TARGETPLATFORM
ARG TARGETOS
ARG TARGETARCH
ARG BUILDPLATFORM
ARG BUILD_CONFIG=release
ARG BUILD_TIMESTAMP
ARG COMMIT_HASH

COPY . .

RUN if [ "$TARGETARCH" = "amd64" ]; \
    then dotnet publish src -c $BUILD_CONFIG -r $TARGETOS-x64 -o build --sc false \
      -p:BuildTimestamp=$BUILD_TIMESTAMP -p:Commit=$COMMIT_HASH -p:Deterministic=true ; \
    else dotnet publish src -c $BUILD_CONFIG -r $TARGETOS-$TARGETARCH -o build --sc false \
      -p:BuildTimestamp=$BUILD_TIMESTAMP -p:Commit=$COMMIT_HASH -p:Deterministic=true ; \
    fi

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0

ARG TARGETPLATFORM
ARG TARGETOS
ARG TARGETARCH
ARG BUILDPLATFORM

WORKDIR /app
COPY --from=build /build/ .
LABEL git_commit=$COMMIT_HASH
EXPOSE 18550
ENTRYPOINT ["./mevsharp"]
