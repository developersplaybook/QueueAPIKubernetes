@echo off
echo Stopping all running Docker containers...
docker stop $(docker ps -q)

echo Removing all stopped containers...
docker rm $(docker ps -a -q)

echo Removing all dangling images...
docker rmi $(docker images -f "dangling=true" -q)

echo Restarting Docker service...
net stop com.docker.service
net start com.docker.service

echo Docker has been restarted.
pause
