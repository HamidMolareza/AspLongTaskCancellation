FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AspLongTaskCancellation/AspLongTaskCancellation.csproj", "AspLongTaskCancellation/"]
RUN dotnet restore "AspLongTaskCancellation/AspLongTaskCancellation.csproj"
COPY . .
WORKDIR "/src/AspLongTaskCancellation"
RUN dotnet build "AspLongTaskCancellation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AspLongTaskCancellation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspLongTaskCancellation.dll"]
