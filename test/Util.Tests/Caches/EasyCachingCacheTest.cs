﻿using EasyCaching.InMemory;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Util.Caches;
using Util.Caches.EasyCaching;
using Xunit;

namespace Util.Tests.Caches {
    /// <summary>
    /// EasyCaching缓存测试
    /// </summary>
    public class EasyCachingCacheTest {
        /// <summary>
        /// EasyCaching缓存
        /// </summary>
        private readonly ICache _cache;
        /// <summary>
        /// 测试服务
        /// </summary>
        private readonly IEasyCachingTestService _service;

        /// <summary>
        /// 测试初始化
        /// </summary>
        public EasyCachingCacheTest() {
            var services = new ServiceCollection();
            services.AddCache( options => options.UseInMemory() );
            var serviceProvider = services.BuildServiceProvider();
            _cache = serviceProvider.GetService<ICache>();
            _service = Substitute.For<IEasyCachingTestService>();
        }

        /// <summary>
        /// 测试并发获取缓存时，应该只有一个操作会访问数据源
        /// </summary>
        [Fact]
        public void Test_1() {
            var key = "Test_Parallel_1";
            Util.Helpers.Thread.ParallelExecute( () => {
                _cache.Get( key, () => _service.Get() );
            },20 );
            _service.Received( 1 ).Get();
        }
    }
}
