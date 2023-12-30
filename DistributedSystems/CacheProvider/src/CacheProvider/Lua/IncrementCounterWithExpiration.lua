local max_value = tonumber(@maxValue)
local current_time = tonumber(@currentTime)
local expiration_time = tonumber(@expirationTime)

-- remove expired items
redis.call("zremrangebyscore", @key, 0, current_time)
-- get current count
local current_count = redis.call("zcard", @key)
if current_count >= max_value then
    return current_count
end

-- add new item
local type_key = @key .. ":type"
if redis.call("get", type_key) == "1" then
    return -1
end
redis.call("set", type_key, 2) -- 1:simple; 2:with-expiration
redis.call("zadd", @key, current_time + expiration_time, @value)
return redis.call("zcard", @key)

