﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CryptoSdk;
using DomainModel;
using DomainModel.Features;
using Views.Interfaces;
using Views.Localization;

namespace Views.Implementations
{
    public partial class TradeForm : Form, ITradeView
    {
        public TradeForm()
        {
            InitializeComponent();

            Locale.Instance.RegisterView(this);
        }

        void ITradeView.SetMarkets(IEnumerable<Market> markets)
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

        void ITradeView.SetPairs(IEnumerable<PairOfMarket> pairs)
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

                baseCurrencyLabel.Text = Pair.Pair.BaseCurrency.Name;
                baseCurrencyLabel2.Text = Pair.Pair.BaseCurrency.Name;
                quoteCurrencyLabel2.Text = Pair.Pair.QuoteCurrency.Name;

                pairComboBox.SelectedItem = value;
                OnPairChanged(Pair);
            }
        }

        void ITradeView.SetIsMayTrade(bool value)
        {
            tradeButton.Enabled = value;
        }

        void ITradeView.SetIsMayTrade2(bool value)
        {
            tradeButton2.Enabled = value;
        }

        public event EventHandler<Market> MarketChanged;
        public event EventHandler<PairOfMarket> PairChanged;
        public event EventHandler ViewClosed;
        public event Action TradeParamsChanged;
        public event Action TradeParams2Changed;
        public event Action Trade;
        public event Action Trade2;

        protected virtual void OnPairChanged(PairOfMarket pair)
        {
            PairChanged?.Invoke(this, pair);
        }

        private void marketComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MarketChanged?.Invoke(this, Market);
        }

        private void pairComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPairChanged(Pair);
        }

        private void TradeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Locale.Instance.UnRegisterView(this);

            ViewClosed?.Invoke(this, EventArgs.Empty);
        }

        private double Quantity
        {
            get
            {
                double quantity;
                return quantityTextBox.Text.TryParseToDouble(out quantity) ? quantity : double.NaN;
            }
        }

        private double Quantity2
        {
            get
            {
                double quantity;
                return quantityTextBox2.Text.TryParseToDouble(out quantity) ? quantity : double.NaN;
            }
        }

        public TradeParams TradeParams => !double.IsNaN(Quantity) ? new TradeParams(TradePosition, Quantity) : null;
        public TradeParams TradeParams2 => !double.IsNaN(Quantity2) ? new TradeParams(TradePosition2, Quantity2) : null;

        private TradePosition TradePosition => buyRadioButton.Checked ? TradePosition.Buy : TradePosition.Sell;
        private TradePosition TradePosition2 => buyRadioButton2.Checked ? TradePosition.Buy : TradePosition.Sell;

        private void quantityTextBox_TextChanged(object sender, EventArgs e)
        {
            OnTradeParamsChanged();
        }

        private void OnTradeParamsChanged()
        {
            if (TradeParams != null)
                TradeParamsChanged?.Invoke();
        }

        private void OnTradeParamsChanged2()
        {
            if (TradeParams2 != null)
                TradeParams2Changed?.Invoke();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            OnTradeParamsChanged();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            OnTradeParamsChanged2();
        }

        private void quantityTextBox2_TextChanged(object sender, EventArgs e)
        {
            OnTradeParamsChanged2();
        }

        private void tradeButton_Click(object sender, EventArgs e)
        {
            Trade?.Invoke();
        }

        private void tradeButton2_Click(object sender, EventArgs e)
        {
            Trade2?.Invoke();
        }
    }
}