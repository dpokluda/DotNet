local current_time = tonumber(@currentTime)

-- Fetch the expiration field from the hash
local expiration = tonumber(redis.call('HGET', @key, 'expiration'))

-- Check if the fetched expiration is greater than the provided expiration
if expiration and expiration > current_time then
    -- If true, return the value
    return redis.call('HGET', @key, 'value')
else
    -- Otherwise, return nil or an error message
    return nil
end

