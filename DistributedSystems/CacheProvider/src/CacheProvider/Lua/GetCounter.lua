local type_key = @key .. ":type"
local counter_type = redis.call("get", type_key)
if counter_type == "1" then
    -- simple counter
    local current_value = tonumber(redis.call("get", @key))
    if current_value then
        return current_value
    end
end
if counter_type == "2" then
    -- counter with expiration
    local result = redis.call("zremrangebyscore", @key, 0, tonumber(@currentTime))
    if result then
        local current_count = redis.call("zcard", @key)
        return current_count
    end
end

return 0