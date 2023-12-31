local max_value = tonumber(@maxValue)
local current_time = tonumber(@currentTime)
local expiration_time = tonumber(@expirationTime)

-- remove expired items
redis.call("ZREMRANGEBYSCORE", @key, 0, current_time)
-- get current count
local current_count = redis.call("ZCARD", @key)
if current_count >= max_value then
    return 0
end

-- add new item
redis.call("ZADD", @key, current_time + expiration_time, @value)
return redis.call("ZCARD", @key)

