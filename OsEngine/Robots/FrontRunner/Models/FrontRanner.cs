using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using OsEngine.Robots.FrontRunner.ViewModels;
using OsEngine.Robots.FrontRunner.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsEngine.Robots.FrontRunner.Models
{
    public class FrontRannerBot : BotPanel
    {
        public FrontRannerBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            _tab = TabsSimple[0];

            _tab.MarketDepthUpdateEvent += _tab_MarketDepthUpdateEvent;

            _tab.PositionOpeningSuccesEvent += _tab_PositionOpeningSuccesEvent;
            
        }

       

        #region Fields===================================================================================
        public decimal BigVolume = 20000;

        public int Offset = 1;

        public int Take = 10 ;

        public decimal Lot = 2;

        public Position Position = null;

        private BotTabSimple _tab;

        #endregion

        #region Properties=======================================================================

        public Edit Edit
        {
            get => _edit;
            set
            {
                _edit = value;

                if(_edit == Edit.Stop && Position !=null && Position.State == PositionStateType.Opening)
                {
                    _tab.CloseAllOrderToPosition(Position);
                }
            }
        }
        Edit _edit = Edit.Stop;



        #endregion
        #region Methods==========================================================================

        private void _tab_PositionOpeningSuccesEvent(Position pos)
        {
            Position = pos;

            _tab.CloseAllOrderInSystem();

            if (pos.Direction == Side.Sell)
            {
                decimal takePrice = Position.EntryPrice - Take * _tab.Securiti.PriceStep;

                _tab.CloseAtProfit(Position, takePrice, takePrice);
            }
            else if (pos.Direction == Side.Buy)
            {
                decimal takePrice = Position.EntryPrice + Take * _tab.Securiti.PriceStep;

                _tab.CloseAtProfit(Position, takePrice, takePrice);

            }
        }

        private void _tab_MarketDepthUpdateEvent(MarketDepth marketDepth)
        {
            if (Edit == Edit.Stop)
            {
                return;
            }

            if(marketDepth.SecurityNameCode != _tab.Securiti.Name)
            {
                return;
            }
            for (int i=0; i< marketDepth.Asks.Count; i++)
            {
                if (marketDepth.Asks[i].Ask >= BigVolume 
                    && Position == null) 
                    
                {
                    decimal price = marketDepth.Asks[i].Price - Offset * _tab.Securiti.PriceStep;

                  //  Position =_tab.SellAtLimit(Lot, price);
                }

                if (Position != null
                    && marketDepth.Asks[i].Price == Position.EntryPrice + Offset * _tab.Securiti.PriceStep
                    && marketDepth.Asks[i].Ask < BigVolume / 2)
                {
                    //if (Position.State == PositionStateType.Open)
                    //{
                    //    _tab.CloseAtMarket(Position, Position.OpenVolume);
                    //}
                    //else if (Position.State == PositionStateType.Opening)
                    //{
                    //    _tab.CloseAllOrderToPosition(Position);
                    //}
                    

                }

            }

            for (int i = 0; i < marketDepth.Bids.Count; i++)
            {
                if (marketDepth.Bids[i].Bid >= BigVolume
                    && Position == null)
                {
                    decimal price = marketDepth.Bids[i].Price + Offset * _tab.Securiti.PriceStep;

                    Position = _tab.BuyAtLimit(Lot, price);
                }


                if (Position != null
                    && marketDepth.Bids[i].Price == Position.EntryPrice - Offset * _tab.Securiti.PriceStep
                    && marketDepth.Bids[i].Bid < BigVolume / 2)
                {
                    if(Position.State == PositionStateType.Open)
                    {
                        _tab.CloseAtMarket(Position, Position.OpenVolume);
                    }
                    else if (Position.State == PositionStateType.Opening)
                    {
                        _tab.CloseAllOrderToPosition(Position);
                    }

                }
                else if (Position!= null
                    && Position.State == PositionStateType.Opening 
                    && marketDepth.Bids[i].Bid >= BigVolume
                    && marketDepth.Bids[i].Price > Position.EntryPrice - Offset * _tab.Securiti.PriceStep)
                {
                    _tab.CloseAllOrderToPosition(Position);
                    Position = null;
                    break;
                }
            }
        }

        public override string GetNameStrategyType()
        {
            return "FrontRannerBot";
        }

        public override void ShowIndividualSettingsDialog()
        {
            FrontRannerUi ui = new FrontRannerUi(this);

            ui.Show();

        }
        #endregion
    }
}
