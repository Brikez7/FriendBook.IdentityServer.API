﻿using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using NpgsqlTypes;
using NSubstitute.Extensions;
using Testcontainers.Redis;

namespace FriendBook.IdentityServer.Tests.WebAppFactories.ContainerBuilders
{
    public class ContainerBuilderRedis
    {
        public const string ImageRedis = "redis:latest";
        public const string ExposedPort = "6377";
        public const string PortBinding = "6378";
        public const string Password = "TestRedis54321!";
        public static RedisContainer CreateRedisContainer()
        {
            var redisBuilder = new RedisBuilder();
            var r = $"Redis.Identity.{Guid.NewGuid():N}";
            return redisBuilder
                .WithName(r)
                .WithHostname($"RedisHost.Identity.{Guid.NewGuid():N}")
                .WithImage(ImageRedis)
                .WithPortBinding(PortBinding, true)
                .WithExposedPort(ExposedPort)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted($"redis-cli CONFIG SET requirepass \"{Password}\""))
                .Build();
        }
    }
}
