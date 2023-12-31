local current_time = tonumber(@currentTime)
local expiration_time = tonumber(@expirationTime)

if tonumber(@ifNew) == 1 and redis.call("HGET", @key, "value") then
    return -1
end
 
return redis.call("HSET", @key, "value", @value, "expiration", current_time + expiration_time)
