cp ../AuctionHouse.SnapshotService/Dockerfile ../Dockerfile
cp -r /secrets ../
docker build -t snapshot-service ../
rm -f ../Dockerfile
rm -rf ../secrets/
