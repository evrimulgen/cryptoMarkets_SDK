﻿using DomainModel;

namespace Models
{
    public class DefaultSettings
    {
        private static DefaultSettings _instance;

        private DefaultSettings()
        {
            OrderBookSettings = new OrderBookSettings
            {
                Depth = 10,
                OrderBookType = OrderBookType.Both,
                RefreshInterval = 1000,
                Multiplier = 1,
                HighlightLargePositions = true,
                LargeVolumeKoef = 0.25,
            };

            BlowoutVolumeSettings = new BlowoutVolumeSettings
            {
                AutoTrade = false,
                BalancePercentPerOneTrade = 10,
                SendEmailNotifications = true,
                EMail = string.Empty,
                IsShowHistoricSignals = true,
                LargeVolumeKoef = 0.25,
            };
        }

        public static DefaultSettings Instance => _instance ?? (_instance = new DefaultSettings());

        public OrderBookSettings OrderBookSettings { get; set; }

        public BlowoutVolumeSettings BlowoutVolumeSettings { get; set; }
    }
}