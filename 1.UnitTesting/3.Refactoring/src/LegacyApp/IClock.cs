using System;

namespace LegacyApp;

public interface IClock
{
    public DateTime Now { get; }
}

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
