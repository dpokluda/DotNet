# Simple Service

Simple background service demonstrating:
- configure dependency injection (standard)
- configure console logging
- configure program argument parsing
- use color console output

The project contains simple background worker that will print **Name** periodically. 
The **Name** is retrieved from a separate component `ITest` that is injected to the background worker when it is instantiated.
The background worker runs in a loop. The frequency of running is controlled by program argument which is again injected
to the background worker when it is instantiated.

The following command will print **Name** every 5 seconds and the output will contain additional debug information.
```sh
dotnet run -- --interval 5 --debug
# display help information 
dotnet run -- --help
```

# Docker 

## Simple Tutorial
Build the docker image from commandline:

```sh
docker build .
# optionally you can give your image a name (to refer to when you want to run it, etc.)
docker build . -t simpleservice
```

To see the list of all available images, use:

```sh
docker images
docker image ls
```

To run a specific image, use:

```sh
# detached
docker run -dti simpleservice
# attached (you would see the output as it runs)
docker run -ti simpleservice
```

This will start the docker at the background. If you want to see what's going on, you can attach to the container by running 
`docker attach {id}`. Because we used `-t -i` flags, our container has support for TTY and stdin. Because of that when we attach 
to it, we are able to detach using `Ctrl`+`P`, `Ctrl`+`Q` or stop the container using `Ctrl`+`C` keyboard shortcuts.

To see the list of running containers, use:

```sh
docker ps
# to see all (even non-running) containers
docker rm $(docker ps -a -q)
```

When the container is deleted, you can delete the corresponding image as well:

``` sh
docker image rm simpleservice
```

## Other useful Docker commands

- List running containers: `docker ps`
- List all existing containers (running and not running): `docker ps -a`
- Stop a specific container: `docker stop {id}`
- Stop all running containers: `docker stop $(docker ps -a -q)`
- Delete a specific container: `docker rm {id}`
- Delete all containers (only if stopped): `docker rm $(docker ps -a -q)`
- List your images: `docker image ls`
- Delete a specific image: `docker image rm {id}`
- Delete all existing images: `docker image rm $(docker images -a -q)`

# Resources
- [A beginner’s guide to Docker — how to create your first Docker application](https://www.freecodecamp.org/news/a-beginners-guide-to-docker-how-to-create-your-first-docker-application-cc03de9b639f/)