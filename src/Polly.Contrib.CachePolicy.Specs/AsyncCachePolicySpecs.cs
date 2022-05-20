// -----------------------------------------------------------------------
// <copyright file="AsyncCachePolicyTests.cs" company="Microsoft">
// Copyright © Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Polly;
using Polly.Contrib.CachePolicy.Builder.AgingStrategy;
using Polly.Contrib.CachePolicy.Providers.Cache;
using Polly.Contrib.CachePolicy.Providers.Logging;
using Polly.Contrib.CachePolicy.Specs;
using Polly.Contrib.CachePolicy.Utilities;
using Polly.Utilities;
using Xunit;

namespace Polly.Contrib.CachePolicy.Tests
{
    public class AsyncCachePolicyTests
    {
        private const string CacheKey = "CacheKey";

        private const string OperationName = "OperationName";

        private readonly ClassToCache freshClassToCacheValue = new ClassToCache()
        {
            Property = "cacheValue",
            Freshness = true,
        };

        private readonly ClassToCache staleClassToCacheValue = new ClassToCache()
        {
            Property = "cacheValue",
            Freshness = false,
        };

        private readonly ClassToCache errorBackendValue = new ClassToCache()
        {
            Property = "errorBackendValue",
        };

        private readonly ClassToCache cacheableSuccessBackendValue = new ClassToCache()
        {
            Property = "cacheableSuccessBackendValue",
        };

        private readonly ClassToCache uncacheableSuccessBackendValue = new ClassToCache()
        {
            Property = "uncacheableSuccessBackendValue",
        };

        private readonly Mock<ICacheProvider> cacheProvider = new Mock<ICacheProvider>();

        private readonly Mock<ILoggingProvider> loggingProvider = new Mock<ILoggingProvider>();

        private readonly Mock<IAgingStrategy<ClassToCache>> agingStrategy = new Mock<IAgingStrategy<ClassToCache>>();

        [Fact]
        public async Task UT_PolicyDisabled()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(false, this.freshClassToCacheValue);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.cacheableSuccessBackendValue;
                }, new Context().WithCacheKey(CacheKey)
                            .WithOperationName(OperationName));

            Assert.Equal(this.cacheableSuccessBackendValue.Property, result.Property);
        }

        [Fact]
        public async Task UT_CacheMiss_BackendThrows_ExpectedException()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, null);

            try
            {
                var result = await asyncCachePolicy.ExecuteAsync(
                                                        async ctx =>
                                                        {
                                                            await TaskHelper.EmptyTask.ConfigureAwait(false);
                                                            throw new TimeoutException("time out exception");
                                                        },
                                                        new Context().WithCacheKey(CacheKey)
                                                                     .WithOperationName(OperationName));
            }
            catch (TimeoutException timeoutException)
            {
                Assert.Equal("time out exception", timeoutException.Message);
            }
        }

        [Fact]
        public async Task UT_CacheMiss_BackendReturns_SuccessResult()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, null);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.cacheableSuccessBackendValue;
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(this.cacheableSuccessBackendValue.Property, result.Property);
        }

        [Fact]
        public async Task UT_CacheMiss_BackendThrows_UnexpectedException()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, null);

            try
            {
                var result = await asyncCachePolicy.ExecuteAsync(
                    async ctx =>
                    {
                        await TaskHelper.EmptyTask.ConfigureAwait(false);
                        throw new InvalidOperationException("error message");
                    },
                    new Context().WithCacheKey(CacheKey)
                                 .WithOperationName(OperationName));
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Assert.Equal("error message", invalidOperationException.Message);
            }
        }

        [Fact]
        public async Task UT_CacheMiss_BackendReturns_ErrorResult()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, null);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.errorBackendValue;
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(result.Property, this.errorBackendValue.Property);
        }

        [Fact]
        public async Task UT_CacheHitFresh()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, this.freshClassToCacheValue);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.cacheableSuccessBackendValue;
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(this.freshClassToCacheValue.Property, result.Property);
        }

        [Fact]
        public async Task UT_CacheHitStale_BackendReturns_ErrorResult()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, this.staleClassToCacheValue);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.errorBackendValue;
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(this.staleClassToCacheValue.Property, result.Property);
        }

        [Fact]
        public async Task UT_CacheHitStale_BackendReturns_CacheableSuccessResult()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, this.staleClassToCacheValue);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.cacheableSuccessBackendValue;
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(cacheableSuccessBackendValue.Property, result.Property);
        }

        [Fact]
        public async Task UT_CacheHitStale_BackendThrows_ExpectedException()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, this.staleClassToCacheValue);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    throw new TimeoutException();
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(this.staleClassToCacheValue.Property, result.Property);
        }

        [Fact]
        public async Task UT_CacheHitStale_BackendReturns_ExpectedErrorResults()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, this.staleClassToCacheValue);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.cacheableSuccessBackendValue;
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(this.cacheableSuccessBackendValue.Property, result.Property);

            Thread.Sleep(2000);
            this.cacheProvider.Verify(
                provider => provider.SetAsync<ClassToCache>(It.IsAny<string>(), It.IsAny<ClassToCache>(), It.IsAny<TimeSpan>(), It.IsAny<Context>()),
                Times.Once);
        }

        [Fact]
        public async Task UT_CacheHitStale_BackendThrows_UnexpectedException()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, this.staleClassToCacheValue);

            try
            {
                var result = await asyncCachePolicy.ExecuteAsync(
                    async ctx =>
                    {
                        await TaskHelper.EmptyTask.ConfigureAwait(false);
                        throw new InvalidOperationException("invalid operation exception");
                    },
                    new Context().WithCacheKey(CacheKey)
                                 .WithOperationName(OperationName));
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Assert.Equal("invalid operation exception", invalidOperationException.Message);                
            }
        }

        [Fact]
        public async Task UT_CacheHitStale_BackendReturns_UncacheableSuccessResult()
        {
            var asyncCachePolicy = this.SetupAsyncCachePolicy(true, this.staleClassToCacheValue);

            var result = await asyncCachePolicy.ExecuteAsync(
                async ctx =>
                {
                    await TaskHelper.EmptyTask.ConfigureAwait(false);
                    return this.uncacheableSuccessBackendValue;
                },
                new Context().WithCacheKey(CacheKey)
                             .WithOperationName(OperationName));

            Assert.Equal(this.uncacheableSuccessBackendValue.Property, result.Property);
        }

        private AsyncPolicy<ClassToCache> SetupAsyncCachePolicy(bool isPolicyEnabled, ClassToCache valueToReturnFromClassToCache)
        {
            this.cacheProvider.Invocations.Clear();
            this.agingStrategy.Invocations.Clear();

            this.cacheProvider.Setup(provider => provider.GetAsync<ClassToCache>(It.IsAny<string>(), It.IsAny<Context>()))
                         .ReturnsAsync(valueToReturnFromClassToCache);
            this.cacheProvider.Setup(provider => provider.SetAsync<ClassToCache>(It.IsAny<string>(), It.IsAny<ClassToCache>(), It.IsAny<TimeSpan>(), It.IsAny<Context>()))
                         .Returns(Task.CompletedTask);
            this.agingStrategy.Setup(agingStrategy => agingStrategy.GetGraceRelativeToNow(It.IsAny<ClassToCache>(), It.IsAny<Context>()))
                              .Returns(TimeSpan.FromDays(1));
            this.agingStrategy.Setup(agingStrategy => agingStrategy.GetExpirationRelativeToNow(It.IsAny<ClassToCache>(), It.IsAny<Context>()))
                              .Returns(TimeSpan.FromDays(5));

            var asyncCachePolicy = AsyncCachePolicy<ClassToCache>
                                .CreateBuilder(isPolicyEnabled, this.agingStrategy.Object, this.cacheProvider.Object, this.loggingProvider.Object)
                                .FallbackToCacheWhenThrows<TimeoutException>()
                                .OrFallbackToCacheWhenReturns(returnedValue => returnedValue.Property.Equals(this.errorBackendValue.Property))
                                .Build();

            return asyncCachePolicy;
        }
    }
}
