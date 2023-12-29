local max_value = tonumber(@maxValue)
local current_value = tonumber(redis.call("get", @key))
if current_value then
    if current_value == max_value then
        return max_value
    end
end

return redis.call("incr",@key)
