using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Base.ThirdTool
{
    public class RedisClient : IDisposable
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        public RedisClient(IConfiguration configuration)
        {
            var str = configuration.GetConnectionString("Redis");
            this._redis = ConnectionMultiplexer.Connect(str);
            //this._redis = ConnectionMultiplexer.Connect(ConfigHelper.GetConnStr("Redis"));
            this._db = _redis.GetDatabase();
        }

        #region String 字符串  key-value
        /// <summary>
        /// 单个K-V
        /// </summary>
        public bool StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return TryCatch<bool>(() => _db.StringSet(key, value, expiry));
        }
        /// <summary>
        /// 获取单个key的值
        /// </summary>
        public string StringGet(string key)
        {
            return TryCatch<string>(() => _db.StringGet(key));
        }
        /// <summary>
        /// 自增
        /// </summary>
        /// <param name="value">自增量</param>
        public double StringIncr(string key, double value = 1)
        {
            return TryCatch<double>(() => _db.StringIncrement(key, value));
        }
        /// <summary>
        /// 自减
        /// </summary>
        /// <param name="value">自减量</param>
        public double StringDecr(string key, double value = 1)
        {
            return TryCatch<double>(() => _db.StringDecrement(key, value));
        }
        /// <summary>
        /// 多个K-V
        /// </summary>
        public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            var newkeyValues = keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(p.Key, p.Value)).ToList();
            return TryCatch<bool>(() => _db.StringSet(newkeyValues.ToArray()));
        }
        /// <summary>
        /// 获取多个K的值
        /// </summary>
        public RedisValue[] StringGet(List<string> listKey)
        {
            var redisKeys = listKey.Select(redisKey => (RedisKey)redisKey).ToArray();
            return TryCatch<RedisValue[]>(() => _db.StringGet(redisKeys));
        }
        #endregion

        #region List 有序双向链表 可重复
        /// <summary>
        /// 获取指定key的List
        /// </summary>
        public List<T> ListRange<T>(string key)
        {
            return TryCatch<List<T>>(() =>
            {
                var values = _db.ListRange(key);
                return ConvetToList<T>(values);
            });
        }
        /// <summary>
        /// 删除列中指定元素
        /// </summary>
        public void ListRemove<T>(string key, T value)
        {
            TryCatch(() => _db.ListRemove(key, value.ToJson()));
        }
        /// <summary>
        /// 右入列(列尾)
        /// </summary>
        public void ListRightPush<T>(string key, T value)
        {
            TryCatch(() => _db.ListRightPush(key, value.ToJson()));
        }
        /// <summary>
        /// 右出列(列尾)
        /// </summary>
        public T ListRightPop<T>(string key)
        {
            return TryCatch<T>(() =>
            {
                var value = _db.ListRightPop(key);
                return ((string)value).ToObj<T>();
            });
        }
        /// <summary>
        /// 左入列(列头)
        /// </summary>
        public void ListLeftPush<T>(string key, T value)
        {
            TryCatch(() => _db.ListLeftPush(key, value.ToJson()));
        }
        /// <summary>
        /// 左出列(列头)
        /// </summary>
        public T ListLeftPop<T>(string key)
        {
            return TryCatch<T>(() =>
            {
                var value = _db.ListLeftPop(key);
                return ((string)value).ToObj<T>();
            });
        }
        #endregion

        #region Hash 哈希字典
        public bool HashExists(string key, string dataKey)
        {
            return TryCatch<bool>(() => _db.HashExists(key, dataKey));
        }
        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        public List<T> HashKeys<T>(string key)
        {
            return TryCatch(() =>
            {
                RedisValue[] values = _db.HashKeys(key);
                return ConvetToList<T>(values);
            });
        }
        /// <summary>
        /// 将对象插入哈希表
        /// </summary>
        public bool HashSet<T>(string key, string dataKey, T obj)
        {
            return TryCatch<bool>(() => _db.HashSet(key, dataKey, obj.ToJson()));
        }
        /// <summary>
        /// 从哈希表获取对象
        /// </summary>
        public T HashGet<T>(string key, string dataKey)
        {
            return TryCatch<T>(() => _db.HashGet(key, dataKey).ToObj<T>());
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        public bool HashDelete(string key, string dataKey)
        {
            return TryCatch(() => _db.HashDelete(key, dataKey));
        }
        /// <summary>
        /// 哈希自增
        /// </summary>
        public double HashIncr(string key, string dataKey, double val = 1)
        {
            return TryCatch(() => _db.HashIncrement(key, dataKey, val));
        }
        /// <summary>
        /// 哈希自减
        /// </summary>
        public double HashDecr(string key, string dataKey, double val = 1)
        {
            return TryCatch(() => _db.HashDecrement(key, dataKey, val));
        }
        #endregion

        #region Set 无序不重复集合
        /// <summary>
        /// 添加到无序集合(不重复)
        /// </summary>
        public bool SetAdd<T>(string key, T value)
        {
            return TryCatch(() => _db.SetAdd(key, value.ToJson()));
        }
        /// <summary>
        /// 获取无序集合全部元素
        /// </summary>
        public List<T> SetRangeByRank<T>(string key)
        {
            return TryCatch(() =>
            {
                var values = _db.SetMembers(key);
                return ConvetToList<T>(values);
            });
        }
        #endregion

        #region Sorted Set 有序不重复集合
        /// <summary>
        /// 添加到有序集合(不重复)
        /// </summary>
        /// <param name="score">得分</param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            return TryCatch(() => _db.SortedSetAdd(key, value.ToJson(), score));
        }
        /// <summary>
        /// 获取有序集合全部元素
        /// </summary>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            return TryCatch(() =>
            {
                var values = _db.SortedSetRangeByRank(key);
                return ConvetToList<T>(values);
            });
        }
        /// <summary>
        /// 删除有序集合中某个元素
        /// </summary>
        public bool SortedSetRemove<T>(string key, T value)
        {
            return TryCatch(() => _db.SortedSetRemove(key, value.ToJson()));
        }
        /// <summary>
        /// 获取有序集合中的数量
        /// </summary>
        public long SortedSetLength(string key)
        {
            return TryCatch(() => _db.SortedSetLength(key));
        }
        #endregion

        #region try catch 为将来全链路准备
        private T TryCatch<T>(Func<T> action)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Exception ex = null;
            bool isError = false;
            T result;
            try
            {
                result = action();
            }
            catch (Exception exinfo)
            {
                isError = true;
                throw exinfo;
                ex = exinfo;
            }
            finally
            {
                sw.Stop();
            }
            return result;
        }

        private void TryCatch(Action action)
        {

            Stopwatch sw = Stopwatch.StartNew();
            bool isError = false;
            Exception ex = null;
            try
            {
                action();
            }
            catch (Exception exinfo)
            {
                isError = true;
                throw exinfo;
            }
            finally
            {
                sw.Stop();

            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 分布式锁（非阻塞）
        /// 配合SpinWait.SpinUntil使用
        /// </summary>
        public bool LockTake(string key, string token, TimeSpan expiry = default(TimeSpan))
        {
            return TryCatch(() => _db.LockTake(key, token, expiry));
        }
        public bool HasLock(string key)
        {
            return TryCatch(() => _db.LockQuery(key).HasValue);
        }
        public bool KeyDelete(string key)
        {
            return TryCatch(() => _db.KeyDelete(key));
        }
        public bool KeyExists(string key)
        {
            return TryCatch(() => _db.KeyExists(key));
        }
        public IDatabase GetDb()
        {
            return _db;
        }
        #endregion

        #region 辅助
        private List<T> ConvetToList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ((string)item).ToObj<T>();
                result.Add(model);
            }
            return result;
        }
        public void Dispose()
        {
            this._redis.Dispose();
        }
        #endregion
    }
}
