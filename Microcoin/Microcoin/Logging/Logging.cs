﻿using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microcoin.Microcoin.Logging
{
    public static class Logging
    {
        static public void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .CreateLogger();
        }
    }
}
