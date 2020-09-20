using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Alarm
{
    public class AlarmEventArgs : EventArgs
    {
        public AlarmEventArgs(DateTime time)
        {
            this.time = time;
        }
        public DateTime time;
    }

    public delegate void AlarmEventHandler(object sender, AlarmEventArgs e);

    public class AlarmClock
    {
        public event AlarmEventHandler Tick;
        public event AlarmEventHandler Alarm;

        private Thread thread;
        private bool threadenable = true;

        public DateTime CurrentTime { get; set; }
        public DateTime AlarmTime { get; set; }

        public void start()
        {

            thread = new Thread(() =>
             {
                 while (threadenable)
                 {
                     Tick(this, new AlarmEventArgs(CurrentTime));
                     TimeSpan diff = CurrentTime - AlarmTime;
                     if (diff < new TimeSpan(0, 0, 15) && diff >= new TimeSpan(0, 0, 0))
                     {
                         Alarm(this, new AlarmEventArgs(CurrentTime));
                     }
                     Thread.Sleep(1000);
                     CurrentTime = CurrentTime.AddSeconds(1);
                 }
             });
            thread.Start();
        }
        public void stop()
        {
            threadenable = false;
        }
    }
}
