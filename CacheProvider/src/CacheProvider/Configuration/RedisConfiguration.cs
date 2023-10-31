// -------------------------------------------------------------------------
//  <copyright file="RedisConfiguration.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
// -------------------------------------------------------------------------
// 

namespace CacheProvider.Configuration;

public class RedisConfiguration
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; set; }
}