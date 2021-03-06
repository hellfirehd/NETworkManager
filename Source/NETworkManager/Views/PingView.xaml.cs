﻿using NETworkManager.ViewModels;
using System;
using System.Windows.Controls;

namespace NETworkManager.Views
{
    public partial class PingView : UserControl
    {
        PingViewModel viewModel;
            
        public PingView(int tabId)
        {
            InitializeComponent();

            viewModel = new PingViewModel(tabId);
        
            DataContext = viewModel;

            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            viewModel.OnClose();
        }

        public void CloseTab()
        {
            viewModel.OnClose();
        }
    }
}
