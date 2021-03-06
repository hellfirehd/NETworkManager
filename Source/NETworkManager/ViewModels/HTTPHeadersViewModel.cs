﻿using NETworkManager.Models.Network;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using NETworkManager.Models.Settings;
using System.Net;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
using NETworkManager.Utilities;

namespace NETworkManager.ViewModels
{
    public class HTTPHeadersViewModel : ViewModelBase
    {
        #region Variables
        private bool _isLoading = true;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Stopwatch stopwatch = new Stopwatch();

        private string _websiteUri;
        public string WebsiteUri
        {
            get { return _websiteUri; }
            set
            {
                if (value == _websiteUri)
                    return;

                _websiteUri = value;
                OnPropertyChanged();
            }
        }

        private ICollectionView _websiteUriHistoryView;
        public ICollectionView WebsiteUriHistoryView
        {
            get { return _websiteUriHistoryView; }
        }

        private bool _isCheckRunning;
        public bool IsCheckRunning
        {
            get { return _isCheckRunning; }
            set
            {
                if (value == _isCheckRunning)
                    return;

                _isCheckRunning = value;
                OnPropertyChanged();
            }
        }

        private string _headers;
        public string Headers
        {
            get { return _headers; }
            set
            {
                if (value == _headers)
                    return;

                _headers = value;
                OnPropertyChanged();
            }
        }

        private bool _displayStatusMessage;
        public bool DisplayStatusMessage
        {
            get { return _displayStatusMessage; }
            set
            {
                if (value == _displayStatusMessage)
                    return;

                _displayStatusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (value == _statusMessage)
                    return;

                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get { return _startTime; }
            set
            {
                if (value == _startTime)
                    return;

                _startTime = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set
            {
                if (value == _duration)
                    return;

                _duration = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _endTime;
        public DateTime? EndTime
        {
            get { return _endTime; }
            set
            {
                if (value == _endTime)
                    return;

                _endTime = value;
                OnPropertyChanged();
            }
        }

        private int _headersCount;
        public int HeadersCount
        {
            get { return _headersCount; }
            set
            {
                if (value == _headersCount)
                    return;

                _headersCount = value;
                OnPropertyChanged();
            }
        }

        private bool _expandStatistics;
        public bool ExpandStatistics
        {
            get { return _expandStatistics; }
            set
            {
                if (value == _expandStatistics)
                    return;

                if (!_isLoading)
                    SettingsManager.Current.HTTPHeaders_ExpandStatistics = value;

                _expandStatistics = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Contructor, load settings
        public HTTPHeadersViewModel()
        {
            // Set collection view
            _websiteUriHistoryView = CollectionViewSource.GetDefaultView(SettingsManager.Current.HTTPHeaders_WebsiteUriHistory);

            LoadSettings();

            _isLoading = false;
        }

        private void LoadSettings()
        {
            ExpandStatistics = SettingsManager.Current.HTTPHeaders_ExpandStatistics;
        }
        #endregion

        #region ICommands & Actions
        public ICommand CheckCommand
        {
            get { return new RelayCommand(p => CheckAction()); }
        }

        private void CheckAction()
        {
            Check();
        }
        #endregion

        #region Methods
        private async void Check()
        {
            DisplayStatusMessage = false;
            IsCheckRunning = true;

            // Measure time
            StartTime = DateTime.Now;
            stopwatch.Start();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
            EndTime = null;

            Headers = null;
            HeadersCount = 0;

            try
            {
                HTTPHeadersOptions options = new HTTPHeadersOptions()
                {
                    Timeout = SettingsManager.Current.HTTPHeaders_Timeout
                };

                WebHeaderCollection headers = await HTTPHeaders.GetHeadersAsync(new Uri(WebsiteUri), options);

                Headers = headers.ToString();
                HeadersCount = headers.Count;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                DisplayStatusMessage = true;
            }

            AddWebsiteUriToHistory(WebsiteUri);

            // Stop timer and stopwatch
            stopwatch.Stop();
            dispatcherTimer.Stop();

            Duration = stopwatch.Elapsed;
            EndTime = DateTime.Now;

            stopwatch.Reset();

            IsCheckRunning = false;
        }

        private void AddWebsiteUriToHistory(string websiteUri)
        {
            // Create the new list
            List<string> list = ListHelper.Modify(SettingsManager.Current.HTTPHeaders_WebsiteUriHistory.ToList(), websiteUri, SettingsManager.Current.General_HistoryListEntries);

            // Clear the old items
            SettingsManager.Current.HTTPHeaders_WebsiteUriHistory.Clear();
            OnPropertyChanged(nameof(WebsiteUri)); // Raise property changed again, after the collection has been cleared

            // Fill with the new items
            list.ForEach(x => SettingsManager.Current.HTTPHeaders_WebsiteUriHistory.Add(x));
        }
        #endregion

        #region Events
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Duration = stopwatch.Elapsed;
        }
        #endregion
    }
}