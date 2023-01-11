using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OsEngine.Commands;
using OsEngine.Robots.FrontRunner.Models;


namespace OsEngine.Robots.FrontRunner.ViewModels
{
    public class VM:BaseVM
    {
        public VM(FrontRannerBot bot)
        {
            _bot = bot;
        }
        #region Fields=============================================================================
        private FrontRannerBot _bot;

        #endregion

        #region Properties========================================================================
        public decimal BigVolume
        {
            get => _bot.BigVolume;

            set
            {
                _bot.BigVolume = value;
                OnPropertyChanged(nameof(BigVolume));   
            }

        }
       

        public int Offset
        {
            get => _bot.Offset;

            set
            {
                _bot.Offset = value;
                OnPropertyChanged(nameof(Offset));
            }

        }
   
        public int Take
        {
            get => _bot.Take;

            set
            {
                _bot.Take = value;
                OnPropertyChanged(nameof(Take));
            }

        }
     
        public decimal Lot
        {
            get => _bot.Lot;

            set
            {
                _bot.Lot = value;
                OnPropertyChanged(nameof(Lot));
            }

        }
     

        public Edit Edit
        {
            get => _edit;

            set
            {
                _edit = value;
                OnPropertyChanged(nameof(Edit));
            }
        }
        private Edit _edit;

        //public List<Edit> Start { get; set; } = new List<Edit>()
        //{
        //      Edit.Start,

        //       Edit.Stop
        //};



        //public int IndexEdit
        //{
        //    get => _indexEdit;

        //    set
        //    {
        //        _indexEdit = value;
        //        OnPropertyChanged(nameof(IndexEdit));
               
        //    }
        //}
        //private int _indexEdit = 0;


        #endregion

        #region Commands==========================================================================
        private DelegateCommand commandStart;

        public ICommand CommandStart
        {
            get
            {
                if (commandStart == null)
                {
                    commandStart = new DelegateCommand(Start);
                }
                return commandStart;
            }
        }

        #endregion


        #region Methods==========================================================================
        private void Start(object obj)
        {
            if (Edit == Edit.Start)
            {
                Edit = Edit.Stop;
            }
            else
            {
                Edit = Edit.Start;
            }

        }
        #endregion

    }
    public enum Edit
    {
         Start,

         Stop
    }



}
