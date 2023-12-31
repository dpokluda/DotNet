local result = redis.call("ZREMRANGEBYSCORE", @key, 0, tonumber(@currentTime))
if result then
    local current_count = redis.call("ZCARD", @key)
    return current_count
end

return 0