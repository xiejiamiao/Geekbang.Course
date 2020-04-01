using System;
using System.Timers;
using Microsoft.Extensions.Configuration;

namespace ConfigurationCustomDemo
{
    internal class MyConfigurationProvider:ConfigurationProvider
    {
        private Timer timer;

        public MyConfigurationProvider():base()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Load(true);
        }


        //public override void Load()
        //{
        //    Load(false);
        //}

        void Load(bool reload)
        {
            this.Data["lastTime"] = DateTime.Now.ToString();
            if (reload)
            {
                base.OnReload();
            }
        }
    }
}
