using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.Models;
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

        public VM(FrontRannerBot bot)
        {
            this.bot = bot;
        }

        private MyRobot _robot;
        private FrontRannerBot bot;

        public static implicit operator VM(FrontRunner.ViewModels.VM v)
        {
            throw new NotImplementedException();
        }
    }


}
