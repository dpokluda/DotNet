local current_time = tonumber(@currentTime)

-- remove counter
redis.call("zrem", @key, @value)

-- remove expired items
redis.call("zremrangebyscore", @key, 0, current_time)
-- get current count
local current_count = redis.call("zcard", @key)

return current_count
