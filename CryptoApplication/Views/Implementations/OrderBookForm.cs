﻿using DomainModel;
using DomainModel.Features;
using DomainModel.MarketModel;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Views.Interfaces;
using Views.Localization;

namespace Views.Implementations
{
    public partial class OrderBookForm : Form, IOrderBookView
    {
        private IOrderBookPartView OrderBookPartView { get; }

        public OrderBookForm()
        {
            InitializeComponent();

            Locale.Instance.RegisterView(this);

            // todo: to do localization
            foreach (var item in Enum.GetValues(typeof(OrderBookType)))
                typeComboBox.Items.Add(item);

            var orderbookControl = new OrderBookControl
            {
                Dock = DockStyle.Fill
            };
            OrderBookPartView = orderbookControl;
            orderBookPartPanel.Controls.Add(orderbookControl);
        }

        void IOrderBookView.SetMarkets(IEnumerable<Market> markets)
        {
            marketComboBox.BeginUpdate();
            try
            {
                marketComboBox.Items.Clear();
                marketComboBox.Items.AddRange(markets.ToArray<object>());
            }
            finally
            {
                marketComboBox.EndUpdate();
            }
        }

        void IOrderBookView.SetPairs(IEnumerable<PairOfMarket> pairs)
        {
            pairComboBox.BeginUpdate();
            try
            {
                pairComboBox.Items.Clear();
                pairComboBox.Items.AddRange(pairs.ToArray<object>());
            }
            finally
            {
                pairComboBox.EndUpdate();
            }
        }

        void IOrderBookView.SetOrderBook(IOrderBook orderBook)
        {
            OrderBookPartView.SetOrderBook(orderBook);
        }

        void IOrderBookView.SetUsdRate(double? usdRate)
        {
            OrderBookPartView.SetUsdRate(usdRate);
        }

        void IOrderBookView.SetOrderBookSettings(OrderBookSettings settings)
        {
            var changed = false;

            if (Depth != settings.Depth)
            {
                depthNumericUpDown.Value = settings.Depth;
                changed = true;
            }

            if (HighlightLargePositions != settings.HighlightLargePositions)
            {
                largePositionCheckBox.Checked = settings.HighlightLargePositions;
                changed = true;
            }

            if (Math.Abs(LargeVolumeKoef - settings.LargeVolumeKoef) > 0.00001)
            {
                largeVolumeKoefTrackBar.Value = (int)Math.Round(settings.LargeVolumeKoef * 100);
                changed = true;
            }

            if (RefreshInterval != settings.RefreshInterval)
            {
                intervalTextBox.Text = settings.RefreshInterval.ToString();
                changed = true;
            }

            if (Multiplier != settings.Multiplier)
            {
                multiplierComboBox.SelectedIndex = (int)Math.Log10(settings.Multiplier);
                changed = true;
            }

            if (typeComboBox.SelectedItem == null || Type != settings.OrderBookType)
            {
                typeComboBox.SelectedItem = settings.OrderBookType;
                changed = true;
            }

            if (changed)
                OnOrderBookSettingsChanged();
        }

        private int Depth => (int)depthNumericUpDown.Value;

        private int RefreshInterval => int.Parse(intervalTextBox.Text);

        private int Multiplier => (int)Math.Pow(10, multiplierComboBox.SelectedIndex);

        private double LargeVolumeKoef => largeVolumeKoefTrackBar.Value / 100.0;

        private bool HighlightLargePositions => largePositionCheckBox.Checked; 

        private OrderBookType Type => (OrderBookType)typeComboBox.SelectedItem;

        public Market Market
        {
            get { return marketComboBox.SelectedItem as Market; }
            set { marketComboBox.SelectedItem = value; }
        }

        public PairOfMarket Pair
        {
            get
            {
                return pairComboBox.SelectedItem as PairOfMarket;
            }
            set
            {
                if (pairComboBox.SelectedItem == value)
                    return;

                pairComboBox.SelectedItem = value;
                OnPairChanged(Pair);
            }
        }

        private void marketComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MarketChanged?.Invoke(Market);
        }

        private void pairComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPairChanged(Pair);
        }

        private void OrderBookForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Locale.Instance.UnRegisterView(this);

            ViewClosed?.Invoke();
        }

        private void depthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            OnOrderBookSettingsChanged();
        }

        private void intervalTextBox_TextChanged(object sender, EventArgs e)
        {
            OnOrderBookSettingsChanged();
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnOrderBookSettingsChanged();
        }

        private void OnPairChanged(PairOfMarket pair)
        {
            PairChanged?.Invoke(pair);
        }

        private void OnOrderBookSettingsChanged()
        {
            OrderBookSettingsChanged?.Invoke(
                new OrderBookSettings
                {
                    Depth = Depth,
                    OrderBookType = Type,
                    RefreshInterval = RefreshInterval,
                    Multiplier = Multiplier,
                    HighlightLargePositions = HighlightLargePositions,
                    LargeVolumeKoef = LargeVolumeKoef,
                });
        }

        public event Action<PairOfMarket> PairChanged;

        public event Action<Market> MarketChanged;

        public event Action<OrderBookSettings> OrderBookSettingsChanged;

        public event Action ViewClosed;

        private void multiplierComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnOrderBookSettingsChanged();
        }

        private void largeVolumeKoefTrackBar_Scroll(object sender, EventArgs e)
        {
            OnOrderBookSettingsChanged();
        }

        private void largePositionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            OnOrderBookSettingsChanged();
        }
    }
}