docker compose -p cineworld stop && \
docker images --quiet | grep -v $(docker images mcr.microsoft.com/mssql/server --quiet) | xargs -r docker rmi -f && \
docker container prune -f && \
docker network prune -f && \
docker volume prune -f && \
docker builder prune -f
