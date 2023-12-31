local current_time = tonumber(@currentTime)

-- remove counter
redis.call("ZREM", @key, @value)

-- remove expired items
redis.call("ZREMRANGEBYSCORE", @key, 0, current_time)
-- get current count
local current_count = redis.call("ZCARD", @key)

return current_count
