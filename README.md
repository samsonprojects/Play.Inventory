# Play.Inventory

play around with microservices

## Create and publish package

```
version="1.0.2"
owner="samsonprojects"
gh_pat="[PAT HERE]"

dotnet pack src/Play.Inventory.Contracts/ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/Play.Inventory -o ../packages


dotnet nuget push ../packages/Play.Inventory.$version.nupkg --api-key $gh_pat --source "github"

```

## Build docker image

```linux
version="1.0.2"
export GH_OWNER="samsonprojects"
export GH_PAT="[PAT HERE]"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.inventory:$version .
```

## Run the docker image and connect it with the same network as the mongodb and rabbitmq

```linux
docker run -it --rm -p 5004:5004 --name Inventory -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq --network src_default play.Inventory:$version
```
