using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;
using OsEngine.OsTrader.Panels.Tab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OsEngine.Robots
{
    public class MyRobot : BotPanel
    {
        public MyRobot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            this.TabCreate(BotTabType.Simple);

            _tab = TabsSimple[0];

            this.CreateParameter("Mode", "Edit", new[] { "Edit", "Trade" });

            _risk = CreateParameter("Risk  % ",1m,0.1m,10m, 0.1m);
  
            _profitKoef = CreateParameter("Koef Profit", 1m, 0.1m, 10m, 0.1m);

            _countDownCandels = CreateParameter("Сount Down Candels", 1, 1, 5, 1);

            _koefVolume = CreateParameter("Koef Volume", 1m, 2m,  10m, 0.5m);

            _countCandles = CreateParameter("Сount Candels", 10, 5, 50, 1);

            _tab.CandleFinishedEvent += _tab_CandleFinishedEvent;

            _tab.PositionOpeningSuccesEvent += _tab_PositionOpeningSuccesEvent;

            _tab.PositionClosingSuccesEvent += _tab_PositionClosingSuccesEvent;

        }





        #region Fields==========================================================================

        private BotTabSimple _tab;

        /// <summary>
        /// Риск на сделку
        /// </summary>
        private StrategyParameterDecimal _risk;
        /// <summary>
        /// Во сколько раз тейк больше риска
        /// </summary>
        private StrategyParameterDecimal _profitKoef;
        /// <summary>
        /// Количество падающих свечей перед обьемным разворотом
        /// </summary>
        private StrategyParameterInt _countDownCandels;
        /// <summary>
        /// Во сколько раз обьем превышает средний
        /// </summary>
        private StrategyParameterDecimal _koefVolume;
        /// <summary>
        /// Средний обьем
        /// </summary>
        private decimal _averageVolume;
        /// <summary>
        /// Количество пунктов до стоп лосса
        /// </summary>
        private int _punkts = 0;
        /// <summary>
        /// Свеча для выставления стопа
        /// </summary>
        private decimal _lowCandles = 0;
        /// <summary>
        /// Количество свечей для вычисления среднего обьема
        /// </summary>
        private StrategyParameterInt _countCandles;


        #endregion

        #region Methods==========================================================================
        private void _tab_CandleFinishedEvent(List<Candle> candles)
        {
            if (candles.Count < _countDownCandels.ValueInt + 1
                || candles.Count < _countCandles.ValueInt +1)
            {
                return;
            }

            _averageVolume = 0;

            for (int i = candles.Count - 2; i > candles.Count - _countCandles.ValueInt - 2; i--)
            {
                _averageVolume += candles[i].Volume;
            }

            _averageVolume /= _countCandles.ValueInt;

            List<Position> position = _tab.PositionOpenLong;

            if (position.Count > 0)
            {
                return;
            }

            Candle candle = candles[candles.Count - 1];

            if (candle.Close < (candle.High + candle.Low)/2
                || candle.Volume < _averageVolume * _koefVolume.ValueDecimal)
            {
                return;
            }

            for (int i = candles.Count - 2; i > candles.Count - 2 - _countDownCandels.ValueInt; i--)
            {
                if (candles[i].Close > candles[i].Open)
                {
                    return;
                }
            }       

            _punkts = (int)((candle.Close - candle.Low)/_tab.Securiti.PriceStep);

            if (_punkts < 5) 
            {
                return;
            }

            decimal amountStop = _punkts * _tab.Securiti.PriceStepCost;

            decimal amountRisk = _tab.Portfolio.ValueBegin * _risk.ValueDecimal / 100;

            decimal volume = amountRisk / amountStop;

            decimal go = 10000;

            if (_tab.Securiti.Go > 1)
            {
                go = _tab.Securiti.Go;
            }

            decimal _maxLot = _tab.Portfolio.ValueBegin / go;

            if (volume < _maxLot)
            {
                _lowCandles = candle.Low;

                _tab.BuyAtMarket(volume);

            }

        }
        private void _tab_PositionOpeningSuccesEvent(Position pos)
        {
            decimal priceTake = pos.EntryPrice + _punkts * _profitKoef.ValueDecimal;

            _tab.CloseAtProfit(pos, priceTake, priceTake);

            _tab.CloseAtStop(pos, _lowCandles, _lowCandles - 100 * _tab.Securiti.PriceStep);

        }

        private void _tab_PositionClosingSuccesEvent(Position pos)
        {
            SeveCSV(pos);
        }

        private void SeveCSV(Position pos)
        {
            
            if (!File.Exists(@"Engine\trades.csv"))
            {
                string header = ";Позиция;Символ;Лоты;Изменение/Максимум Лотов;"+
                                "Исполнение входа;Сигнал входа;Бар входа;Дата входа;Время входа"+
                                ";Цена входа;Комиссия входа;Исполнение выхода;Сигнал выхода;Бар"+
                                "выхода;Дата выхода;Время выхода;Цена выхода;Комиссия выхода;"+
                                "Средневзвешенная цена входа;П/У;П/У сделки;П/У с одного лота;"+
                                "Зафиксированная П/У;Открытая П/У;Продолж. (баров);Доход/Бар;Общий"+
                                "П/У;% изменения;MAE;MAE %;MFE;MFE %";

                using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", false))
                {
                    writer.WriteLine(header);

                    writer.Close();
                }
            }
            using (StreamWriter writer = new StreamWriter(@"Engine\trades.csv", true))
            {
                string str = ";;;;;;;;" + pos.TimeOpen.ToShortDateString();

                str += ";" + pos.TimeOpen.TimeOfDay;
                str += ";;;;;;;;;;;;;;" + pos.ProfitPortfolioPunkt + ";;;;;;;;;"; //Добавили комментарии к строке кода

                writer.WriteLine(str);

                writer.Close();
            }
        }
        #endregion


        public override string GetNameStrategyType()
        {
            return nameof(MyRobot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            WindowMyRobot window = new WindowMyRobot(this);

            window.ShowDialog();

        }
    } 
}
