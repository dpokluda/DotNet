local max_value = tonumber(@maxValue)
local current_value = tonumber(redis.call("get", @key))
if current_value then
    if current_value == max_value then
        return max_value
    end
end

-- set the counter type
local type_key = @key .. ":type"
if redis.call("get", type_key) == "2" then
    return -1
end
redis.call("set", type_key, 1) -- 1:simple; 2:with-expiration
return redis.call("incr",@key)
