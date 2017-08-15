﻿using DomainModel.Features;
using DomainModel.MarketModel.Updaters.Balance;
using DomainModel.MarketModel.Updaters.OrderBook;
using System.Collections.Generic;
using DomainModel.MarketModel.Updaters.PairStatistic;

namespace DomainModel
{
    public interface IModel
    {
        IEnumerable<Market> Markets { get; }

        IOrderBookUpdaterProvider OrderBookUpdaterProvider { get; }

        IBalanceUpdaterProvider BalanceUpdaterProvider { get; }

        IPairStatisticUpdaterProvider PairStatisticUpdaterProvider { get; }
    }
}