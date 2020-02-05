FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.10 AS runtime

ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

ADD ./secrets /secrets
WORKDIR /app
COPY out/ ./


# RUN addgroup snapshot-service -g 2000
# RUN adduser -D -u 2000 snapshot-service -G snapshot-service

# USER 2000

ENTRYPOINT ["dotnet", "AuctionHouse.SnapshotService.dll"]