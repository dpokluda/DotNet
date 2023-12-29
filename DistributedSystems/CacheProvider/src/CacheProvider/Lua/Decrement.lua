redis.call("zrem", @key, @value)
return 0
