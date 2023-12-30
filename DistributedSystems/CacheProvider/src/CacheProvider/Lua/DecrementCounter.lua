local current_value = tonumber(redis.call("get", @key))
if current_value then
    if current_value > 0 then
        return redis.call("decr",@key)
    end
end

return 0
