﻿using DomainModel;
using DomainModel.Features;
using DomainModel.MarketModel;
using System;
using System.Collections.Generic;

namespace CryptoSdk.Dummy
{
    public class DummyInfo : IMarketInfo
    {
        public IEnumerable<PairOfMarket> Pairs(Market market)
        {
            yield return new PairOfMarket(PairDummy.BtcLtc, market, 0.000001);
            yield return new PairOfMarket(PairDummy.BtcEth, market, 0.000001);
            yield return new PairOfMarket(PairDummy.EthLtc, market, 0.000001);
            yield return new PairOfMarket(PairDummy.BtcDoge, market, 0.000001, false);
        }

        public IEnumerable<CurrencyOfMarket> Currencies(Market market)
        {
            yield return new CurrencyOfMarket(CurrencyDummy.Btc, market, 0.01);
            yield return new CurrencyOfMarket(CurrencyDummy.Ltc, market, 0.01);
            yield return new CurrencyOfMarket(CurrencyDummy.Eth, market, 0.01);
            yield return new CurrencyOfMarket(CurrencyDummy.Doge, market, 0.01, false);
        }

        public Tick Tick(Pair pair)
        {
            var bid = new Random().NextDouble();
            var ask = bid - 0.0001;
            return new Tick(bid, ask);
        }

        public OrderBook OrderBook(Pair pair, int depth = 10, OrderBookType orderBookType = OrderBookType.Both)
        {
            var result = new OrderBook(pair);

            var rnd = new Random();
            var startPrice = rnd.NextDouble();

            var asks = new List<OrderBookPart>();
            for (var i = 0; i < depth; i++)
            {
                var step = rnd.NextDouble();
                asks.Add(new OrderBookPart(startPrice + step, rnd.NextDouble()));
            }

            var bids = new List<OrderBookPart>();
            for (var i = 0; i < depth; i++)
            {
                var step = rnd.NextDouble();
                bids.Add(new OrderBookPart(startPrice - step, rnd.NextDouble()));
            }

            result.ReplaceAsk(asks);
            result.ReplaceBids(bids);

            return result;
        }

        public ICollection<PairStatistic> PairsStatistic()
        {
            var result = new List<PairStatistic>();
            result.Add(new PairStatistic(PairDummy.BtcLtc, 0.1234, 0.0234, 1234, 0.02, 0.02345, 40, 34));
            result.Add(new PairStatistic(PairDummy.BtcEth, 10, 7, 100, 7, 4, 400, 340));

            return result;
        }
    }
}