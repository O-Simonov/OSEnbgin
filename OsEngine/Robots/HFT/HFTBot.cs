using OsEngine.Entity;
using OsEngine.Market;
using OsEngine.Market.Servers;
using OsEngine.Market.Servers.Kraken.KrakenEntity;
using OsEngine.OsTrader.Panels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OsEngine.Robots.HFT
{
    public class HFTBot : BotPanel
    {
        public HFTBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            ServerMaster.ServerCreateEvent += ServerMaster_ServerCreateEvent;
        }
        #region Fields==============================================================================================

        private List<IServer> _servers = new List<IServer>();

        private List<Portfolio> _portfolios = new List<Portfolio>();

        private string _nameSecurity = "BTCUSDT";

        private ServerType _serverType = ServerType.Binance;

        private Security _security = null;

        private IServer _server;

        CandleSeries _series = null;

        #endregion
        #region Methods=============================================================================================

        private void ServerMaster_ServerCreateEvent(IServer newServer)
        {
            foreach (IServer server in _servers)
            {
                if (newServer == server)
                {
                    return;
                }
            }

            if(newServer.ServerType == _serverType)
            {
                _server = newServer;

            }
            _servers.Add(newServer);

            newServer.PortfoliosChangeEvent += NewServer_PortfoliosChangeEvent;
            newServer.SecuritiesChangeEvent += NewServer_SecuritiesChangeEvent;
            newServer.NeadToReconnectEvent += NewServer_NeadToReconnectEvent;
            newServer.NewMarketDepthEvent += NewServer_NewMarketDepthEvent;
            newServer.NewTradeEvent += NewServer_NewTradeEvent;
            newServer.NewOrderIncomeEvent += NewServer_NewOrderIncomeEvent;
            newServer.NewMyTradeEvent += NewServer_NewMyTradeEvent;

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
            StartSecurity(_security);
        }

        private void NewServer_SecuritiesChangeEvent(List<Security> securities)
        {
            if(_security != null)
            {
                return;
            }

            for (int i=0; i<securities.Count; i++)
            {
                if (_nameSecurity == securities[i].Name)
                {
                    _security = securities[i];

                    StartSecurity(_security);

                    break;
                }
            }
        }

        private void StartSecurity(Security security)
        {
            if(security == null)
            {
                Debug.WriteLine("StartSecutity security = null");
                return;
            }

            Task.Run(() =>
            {
                while (true)
                {
                    _series = _server.StartThisSecurity(security.Name, new TimeFrameBuilder(), security.NameClass);//Заказали нужную бумагу

                    if (_series != null)
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }
            });
            
        }

        private void NewServer_PortfoliosChangeEvent(List<Portfolio> newportfolios)
        {
            for (int x = 0; x < newportfolios.Count; x++)
            {
                bool flag = true;

                for (int i = 0; i < _portfolios.Count; i++)
                {
                    if (newportfolios[x].Number == _portfolios[i].Number)
                    {
                        flag = false;
                        break;
                    }

                }
                if (flag)
                {
                    _portfolios.Add(newportfolios[x]);
                }
            }
        }

        public override string GetNameStrategyType()
        {
            return nameof(HFTBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            
        }
        #endregion
    }
}
