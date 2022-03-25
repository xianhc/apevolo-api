using System;

namespace ApeVolo.Common.SnowflakeIdHelper;

/**
     * tweeter的snowflake 移植到Java翻译成Net:
     *   (a) id构成: 42位的时间前缀 + 10位的节点标识 + 12位的sequence避免并发的数字(12位不够用时强制得到新的时间前缀)
     *       注意这里进行了小改动: snowkflake是5位的datacenter加5位的机器id; 这里变成使用10位的机器id
     *   (b) 对系统时间的依赖性非常强，需关闭ntp的时间同步功能。当检测到ntp时间调整后，将会拒绝分配id
     */
public sealed class SnowflakeIdWorker
{
    private const long Twepoch = 1288834974657L;

    const int WorkerIdBits = 10;
    const int DatacenterIdBits = 0;
    const int SequenceBits = 12;

    const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    //const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);

    private long _sequence;
    private long _lastTimestamp = -1L;


    public SnowflakeIdWorker(long workerId, long sequence = 0L)
    {
        WorkerId = workerId;
        DatacenterId = 0;
        _sequence = sequence;

        // sanity check for workerId
        if (workerId > MaxWorkerId || workerId < 0)
        {
            throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");
        }

        //if (datacenterId > MaxDatacenterId || datacenterId < 0)
        //{
        //    throw new ArgumentException(String.Format("datacenter Id can't be greater than {0} or less than 0", MaxDatacenterId));
        //}

        //log.info(
        //    String.Format("worker starting. timestamp left shift {0}, datacenter id bits {1}, worker id bits {2}, sequence bits {3}, workerid {4}",
        //                  TimestampLeftShift, DatacenterIdBits, WorkerIdBits, SequenceBits, workerId)
        //    );	
    }

    public long WorkerId { get; }
    public long DatacenterId { get; }

    //public long Sequence
    //{
    //    get { return _sequence; }
    //    internal set { _sequence = value; }
    //}

    // def get_timestamp() = System.currentTimeMillis

    readonly object _lock = new();

    public long NextId()
    {
        lock (_lock)
        {
            var timestamp = TimeGen();

            if (timestamp < _lastTimestamp)
            {
                throw new System.Exception(
                    $"Clock moved backwards.  Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;
            var id = ((timestamp - Twepoch) << TimestampLeftShift) |
                     (DatacenterId << DatacenterIdShift) |
                     (WorkerId << WorkerIdShift) | _sequence;

            return id;
        }
    }

    /**
         * 等待下一个毫秒的到来, 保证返回的毫秒数在参数lastTimestamp之后
         */
    private long TilNextMillis(long lastTimestamp)
    {
        long timestamp = TimeGen();

        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }

        return timestamp;
    }

    /**
         * 获得系统当前毫秒数
         */
    private static long TimeGen()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }
}