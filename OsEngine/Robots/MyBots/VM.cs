﻿using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OsEngine.Robots
{
    public class VM : BaseVM
    {
        public VM(MyRobot robot)
        {
            _robot = robot;
        }

       private MyRobot _robot;
      
    }


}
