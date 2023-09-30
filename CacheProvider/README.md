# Cache Provider

This project implements basic component to access cache. The project implements a simple local cache as well as 
component to access distributed cache implemented in Redis. The Redis implementation uses StackExchange.Redis 
library and uses the best-practice pattern for accessing cache objects.

## Resources

The projects is built using the following sources:

- [Distributed Locks Pattern with Redis](https://redis.io/docs/manual/patterns/distributed-locks/)
- [Distributed Lock in .NET](https://github.com/madelson/DistributedLock)