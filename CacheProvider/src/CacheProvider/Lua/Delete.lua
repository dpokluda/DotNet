if redis.call("get",@key) == @value then
    return redis.call("del",@key)
else
    return 0
end