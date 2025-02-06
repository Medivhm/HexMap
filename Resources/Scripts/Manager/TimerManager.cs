using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 使用QuickTimers管理，尽量少用TimerManager.Add
/// </summary>
class TimerManager : MonoSingleton<TimerManager>
{
    class Timer
    {
        public bool IsRunning
        {
            get { return isRunning; }
        }

        bool isRunning;
        float time;
        float total;
        Action callback;

        public Timer(float time, Action callback)
        {
            this.isRunning = true;
            this.time = time;
            this.callback = callback;
            this.total = 0;
        }

        public void Tick(float deltaTime)
        {
            this.total += deltaTime;
            if (this.total >= time)
            {
                this.callback();
                this.isRunning = false;
            }
        }

        public void Stop()
        {
            this.isRunning = false;
        }
    };

    // 手动定时器
    static Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
    List<string> deleteTimers = new List<string>();
    // 自动定时器
    static Dictionary<int, Timer> autoTimers = new Dictionary<int, Timer>();
    List<int> deleteAutoTimers = new List<int>();

    public static readonly int TimerMax = 2000;
    static private int timerId;
    static private int TimerId
    {
        get
        {
            int temp = timerId;
            int rangeTimes = 0;
            while (autoTimers.ContainsKey(temp) && rangeTimes < TimerMax)
            {
                temp = (temp + 1) % TimerMax;
                rangeTimes++;
            }
            if (rangeTimes == TimerMax)
            {
                DebugTool.Error("定时器超数量了");
                throw new Exception("定时器超数量了");
            }
            else
            {
                timerId = temp;
                return temp;
            }
        }
        set
        {
            timerId = value;
        }
    }

    public void Init()
    {
        TimerId = 0;
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        TimerDestroy();
        TimerUpdate(deltaTime);
    }

    private void TimerUpdate(float deltaTime)
    {
        List<string> timerNames = timers.Keys.ToList();
        foreach (string timerName in timerNames)
        {
            Timer timer = null;
            timers.TryGetValue(timerName, out timer);
            if (timer != null)
            {
                if (timer.IsRunning)
                {
                    timer.Tick(deltaTime);
                }
                else
                {
                    deleteTimers.Add(timerName);
                }
            }
        }

        List<int> autoTimerIds = autoTimers.Keys.ToList();
        foreach (int timerId in autoTimerIds)
        {
            Timer timer = null;
            autoTimers.TryGetValue(timerId, out timer);
            if (timer != null)
            {
                if (timer.IsRunning)
                {
                    timer.Tick(deltaTime);
                }
                else
                {
                    deleteAutoTimers.Add(timerId);
                }
            }
        }
    }

    private void TimerDestroy()
    {
        foreach (string timerName in deleteTimers)
        {
            if (timers.TryGetValue(timerName, out _))
            {
                timers.Remove(timerName);
            }
        }
        deleteTimers.Clear();

        foreach (int timerId in deleteAutoTimers)
        {
            if (autoTimers.TryGetValue(timerId, out _))
            {
                autoTimers.Remove(timerId);
            }
        }
        deleteAutoTimers.Clear();
    }

    public static void Add(string timerName, float time, Action callback)
    {
        if (timers.ContainsKey(timerName))
        {
            DebugTool.Error("TimersManager 已存在相同名称Timer");
            return;
        }
        timers.Add(timerName, new Timer(time, callback));
    }

    public static int Add(float time, Action callback)
    {
        int timerId = TimerId;
        autoTimers.Add(TimerId, new Timer(time, callback));
        return timerId;
    }

    public static void Remove(string timerName)
    {
        Timer timer = null;
        timers.TryGetValue(timerName, out timer);
        if (timer != null)
        {
            timer.Stop();
        }
    }

    public static void Remove(int timerId)
    {
        Timer timer = null;
        autoTimers.TryGetValue(timerId, out timer);
        if (timer != null)
        {
            timer.Stop();
        }
    }

    public void LateUpdate()
    {

    }
}