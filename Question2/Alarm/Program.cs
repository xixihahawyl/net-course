using System;

namespace Alarm
{
    class Program
    {
        static void Main(string[] args)
        {
            AlarmClock clock = new AlarmClock();
            clock.CurrentTime = new DateTime(2020, 9, 20, 8, 0, 0);
            clock.AlarmTime = new DateTime(2020, 9, 20, 8, 0, 10);
            clock.Tick += (s, e) =>
              {
                  Console.WriteLine($"当前时间{e.time}");
              };
            clock.Alarm += (s, e) =>
              {
                  Console.WriteLine("正在alarming");
              };
            clock.start();
            Console.ReadKey();
            clock.stop();
        }
    }
}
