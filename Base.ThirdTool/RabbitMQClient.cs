using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.ThirdTool
{
    /// <summary>
    /// 一个Exchange----多个Queue-----弄个缓存映射关系，初始化+支持全新绑定
    /// 全局单例使用
    /// 关系应该是直接配置到RabbitMQ了---程序只是向某个位置写入即可
    /// 全量更新--耗时---阻塞实时更新---换不同的exchange？
    /// </summary>
    public class RabbitMQClient
    {
        private readonly string _hostName = null;
        private readonly string _userName = null;
        private readonly string _password = null;

        public RabbitMQClient(IConfiguration configuration)
        {
            _hostName = configuration.GetSection("RabbitMQ:Host").Value;
            _userName = configuration.GetSection("RabbitMQ:UserName").Value;
            _password = configuration.GetSection("RabbitMQ:Password").Value;
        }

        /// <summary>
        /// 只管exchange---
        /// 4种路由类型？
        /// Send前完成交换机初始化
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="message">建议Json格式</param>
        public void Send(string exchangeName, string message)
        {
            this.InitExchange(exchangeName);

            if (_CurrentConnection == null || !_CurrentConnection.IsOpen)
            {
                this.InitConnection();
            }
            using (var channel = _CurrentConnection.CreateModel())//开辟新的信道通信
            {
                try
                {
                    channel.TxSelect();//开启Tx事务---RabbitMQ协议级的事务-----强事务

                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: exchangeName,
                                         routingKey: string.Empty,
                                         basicProperties: null,
                                         body: body);
                    channel.TxCommit();//提交
                    Console.WriteLine($" [x] Sent {body.Length}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"【{message}】发送到Broker失败！{ex.Message}");
                    channel.TxRollback(); //事务回滚--前面的所有操作就全部作废了。。。。
                }
            }
        }
        #region Receive
        /// <summary>
        /// 注册处理动作
        /// </summary>
        public void RegistReciveAction(string exchangeName,string queueName, Func<string, bool> func)
        {
            this.InitBindQueue(exchangeName,queueName);

            Task.Run(() =>
            {
                using (var channel = _CurrentConnection.CreateModel())//开辟新的信道通信
                {
                    //创建消费者对象
                    var consumer = new EventingBasicConsumer(channel);
                    channel.BasicQos(0, 0, true);
                    //绑定监听回调函数
                    consumer.Received += (sender, ea) =>
                    {
                        string str = Encoding.UTF8.GetString(ea.Body.ToArray());
                        if (func(str))
                        {
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);//确认已消费（自主ACK）
                        }
                        else
                        {
                            //channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: true);//放回队列--重新包装信息，放入其他队列
                        }
                    };
                    //消费者开始监听
                    channel.BasicConsume(queue: queueName,
                                         autoAck: false,//不自动ACK
                                         consumer: consumer);
                    Console.WriteLine($" Register Consumer To {exchangeName}-{queueName}");
                    Console.ReadLine();
                }
            });
        }
        #endregion

        #region 私有(Init)
        private static object _exchangeQueueLock = new object(); //线程锁
        private static Dictionary<string, bool> _exchangeQueueDic = new Dictionary<string, bool>();//交换机队列
        /// <summary>
        /// 初始化队列
        /// </summary>
        /// <param name="rabbitMQConsumerModel"></param>
        private void InitBindQueue(string exchangeName,string queueName)
        {
            if (!_exchangeQueueDic.ContainsKey($"InitBindQueue_{exchangeName}_{queueName}"))
            {
                lock (_exchangeQueueLock)
                {
                    if (!_exchangeQueueDic.ContainsKey($"InitBindQueue_{exchangeName}_{queueName}"))
                    {
                        this.InitConnection();
                        using (IModel channel = _CurrentConnection.CreateModel())
                        {
                            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
                            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty, arguments: null);
                        }
                        _exchangeQueueDic[$"InitBindQueue_{exchangeName}_{queueName}"] = true;
                    }
                }
            }
        }
        /// <summary>
        /// 初始化交换机
        /// </summary>
        private void InitExchange(string exchangeName)
        {
            if (!_exchangeQueueDic.ContainsKey($"InitExchange_{exchangeName}"))//没用api确认
            {
                lock (_exchangeQueueLock)
                {
                    if (!_exchangeQueueDic.ContainsKey($"InitExchange_{exchangeName}"))
                    {
                        this.InitConnection();
                        using (IModel channel = _CurrentConnection.CreateModel())
                        {
                            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false, arguments: null);
                        }
                        _exchangeQueueDic[$"InitExchange_{exchangeName}"] = true;
                    }
                }
            }
        }
        private static object _connectLock = new object();
        private static IConnection _CurrentConnection = null;//链接做成单例重用--channel是新的
        private void InitConnection()
        {
            if (_CurrentConnection == null || !_CurrentConnection.IsOpen)
            {
                lock (_connectLock)
                {
                    if (_CurrentConnection == null || !_CurrentConnection.IsOpen)
                    {
                        var factory = new ConnectionFactory()
                        {
                            HostName = this._hostName,
                            Password = this._password,
                            UserName = this._userName
                        };
                        _CurrentConnection = factory.CreateConnection();
                    }
                }
            }
        }
        #endregion
    }
}
