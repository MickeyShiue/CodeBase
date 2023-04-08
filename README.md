### Redis for docker
`docker run --name redis -d -p 6379:6379 redis:latest`

### Important Redis set config 
- `redis-cli：config set notify-keyspace-events KEA`

### Implement
- `webApi` - CreateLockAsync
- `webApi` - UnlockAsync 
- `BackgroundService` - RedisKeyExpiredEvent 
