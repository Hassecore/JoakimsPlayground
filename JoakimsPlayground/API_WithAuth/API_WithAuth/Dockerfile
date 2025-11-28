FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

#COPY *.csproj ./
#RUN dotnet restore

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "API_WithAuth.dll"]

