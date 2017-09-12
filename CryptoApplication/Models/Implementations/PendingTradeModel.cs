﻿using DomainModel.Features;
using Models.Interfaces;
using System.Collections.Generic;
using IModel = DomainModel.IModel;

namespace Models.Implementations
{
    public class PendingTradeModel : IPendingTradeModel
    {
        private readonly IModel _domainModel;

        public PendingTradeModel(IModel domainModel)
        {
            _domainModel = domainModel;
        }

        IEnumerable<Market> IPendingTradeModel.Markets => _domainModel.Markets;

        IEnumerable<Order> IPendingTradeModel.OpenedOrders(Market market)
        {
            return null;
            //market
        }

        public void NeedOrderBookOf(PairOfMarket pair)
        {
        }

        public void NeedBalanceOf(CurrencyOfMarket currency)
        {
        }

        public double Available(Currency currency)
        {
            throw new System.NotImplementedException();
        }

        public double Price(Currency currency)
        {
            throw new System.NotImplementedException();
        }
    }
}