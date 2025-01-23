using System;
using System.Collections.Generic;

public class QuickTimer
{
    // 自动定时器
    protected List<int> timerIDs;

    public QuickTimer()
    {
        timerIDs = new List<int>();
    }

    public int AddTimer(float time, Action action)
    {
        int id = 0;
        id = TimerManager.Add(3f, () =>
        {
            action.Invoke();
            timerIDs.Remove(id);
        });
        timerIDs.Add(id);
        return id;
    }

    public void RemoveTimer(int timerID)
    {
        timerIDs.Remove(timerID);
        TimerManager.Remove(timerID);
    }

    public void DestroyTimers()
    {
        foreach (var id in timerIDs)
        {
            TimerManager.Remove(id);
        }
        timerIDs.Clear();
    }
}