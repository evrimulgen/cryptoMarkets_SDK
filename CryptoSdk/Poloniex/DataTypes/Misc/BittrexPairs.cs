﻿using DomainModel.Features;

namespace CryptoSdk.Poloniex.DataTypes.Misc
{
    internal class BittrexPairs
    {
        public static string AsString(Pair pair)
        {
            return $"{pair.QuoteCurrency.Name}_{pair.BaseCurrency.Name}";
        }

        public static string AsString(PairOfMarket pair)
        {
            return AsString(pair.Pair);
        }
    }
}