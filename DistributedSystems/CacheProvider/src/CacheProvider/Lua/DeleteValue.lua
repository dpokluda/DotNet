if redis.call("HGET", @key, "value") == @value then
    return redis.call("DEL",@key)
else
    return 0
end