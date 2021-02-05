using Base.ThirdTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSUnitTest_RabbitMQ
{
    [TestClass]
    public class UnitTest1
    {
        private readonly RedisClient _redis;

        //public UnitTest1(RedisClient redis)
        //{
        //    _redis = redis;
        //}
        [TestMethod]
        public void TestMethod1()
        {
            //var a=_redis.StringGet("test");
            System.Console.WriteLine("asfasf");
        }
    }
}
