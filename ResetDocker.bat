@echo off
echo Stopping all running Docker containers...
for /f %%i in ('docker ps -q') do docker stop %%i

echo Removing all stopped containers...
for /f %%i in ('docker ps -a -q') do docker rm %%i

echo Removing all dangling images...
for /f %%i in ('docker images -f "dangling=true" -q') do docker rmi %%i

echo Restarting Docker service...
net stop com.docker.service
net start com.docker.service

echo Docker has been restarted.
pause
