local max_count = tonumber(@maxCound)
local current_time = tonumber(@currentTime)
local expiration_time = tonumber(@expirationTime)

-- remove expired items
redis.call("zremrangebyscore", @key, 0, current_time - expiration_time)
-- get current count
local current_count = redis.call("zcard", @key)
if current_count >= max_count then
    return 0
end

-- add new item
redis.call("zadd", @key, @value, current_time)
return 1
