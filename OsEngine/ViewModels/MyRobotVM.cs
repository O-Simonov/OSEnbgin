using OkonkwoOandaV20;
using OsEngine.Commands;
using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.Robots;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace OsEngine.ViewModels
{
   public class MyRobotVM:BaseVM
    {
        public MyRobotVM() 
        {
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;
        }



        #region Properties ==========================================================
        public ObservableCollection<string> ListSecurities { get; set; } = new ObservableCollection<string>();

        public string SelectedSecurity
        {
            get => _selectedSecurity;
            set
            {
                _selectedSecurity = value;
                OnPropertyChanged(nameof(SelectedSecurity));
                _security = GetSecurityForName(_selectedSecurity);
                StartSecurity(_security);


            }
        }
        private string _selectedSecurity = "";

        #endregion

        #region Fields ==============================================================

        IServer _server;

        List<Security> _securities =new List<Security>();

        private Security _security;

        #endregion

        #region Commands ===========================================================

        private DelegateCommand commandServersToCopnnect;

        public DelegateCommand CommandServersToCopnnect
        {
            get
            {
               if( commandServersToCopnnect == null)
                {
                    commandServersToCopnnect = new DelegateCommand(ServersToCopnnect);
                }
               return commandServersToCopnnect;
            }
        }

        #endregion

        #region Methods ===========================================================

        void ServersToCopnnect(object o)
        {
            ServerMaster.ShowDialog(false);
        }

        private Security GetSecurityForName(string name)
        {
            for(int i=0;i<_securities.Count;i++)
            {
                if (_securities[i].Name == name)
                {
                    return _securities[i];
                }
            }
            return null;

        }

        private void StartSecurity(Security security)
        {
            if (security == null)
            {
                Debug.WriteLine("StartSecutity security = null");
                return;
            }

            Task.Run(() =>
            {
                while (true)
                {
                    var series = _server.StartThisSecurity(security.Name, new TimeFrameBuilder(), security.NameClass);//Заказали нужную бумагу

                    if (series != null)
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }
            });

        }
        private void ServerMaster_ServerCreateEvent(IServer newServer)
        {
            if (newServer == _server)
            {
                return;
            }

            _server = newServer;

            _server.PortfoliosChangeEvent += NewServer_PortfoliosChangeEvent;
            _server.SecuritiesChangeEvent += NewServer_SecuritiesChangeEvent; ;
            _server.NeadToReconnectEvent += NewServer_NeadToReconnectEvent;
            _server.NewMarketDepthEvent += NewServer_NewMarketDepthEvent;
            _server.NewTradeEvent += NewServer_NewTradeEvent;
            _server.NewOrderIncomeEvent += NewServer_NewOrderIncomeEvent;
            _server.NewMyTradeEvent += NewServer_NewMyTradeEvent;
            _server.ConnectStatusChangeEvent += NewServer_ConnectStatusChangeEvent;
        }

        private void NewServer_ConnectStatusChangeEvent(string connect)
        {
          
        }

        private void NewServer_SecuritiesChangeEvent(List<Security> securities)
        {
            ObservableCollection<string> listSecurities = new ObservableCollection<string>();

            for(int i=0;i < securities.Count; i++)
            {
                listSecurities.Add(securities[i].Name);
            }

            ListSecurities = listSecurities;

            OnPropertyChanged(nameof(ListSecurities));

            _securities = securities;
        }

        private void NewServer_PortfoliosChangeEvent(List<Portfolio> portfolios)
        {
            
        }

        private void NewServer_NewMyTradeEvent(MyTrade myTrade)
        {

        }

        private void NewServer_NewOrderIncomeEvent(Order order)
        {

        }

        private void NewServer_NewTradeEvent(List<Trade> trades)
        {

        }

        private void NewServer_NewMarketDepthEvent(MarketDepth marketDepth)
        {

        }
        private void NewServer_NeadToReconnectEvent()
        {
           
        }

        #endregion


    }
}
