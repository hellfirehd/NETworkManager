﻿using NETworkManager.Models.Settings;
using NETworkManager.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace NETworkManager.ViewModels
{
    public class IPScannerProfileViewModel : ViewModelBase
    {
        private bool _isLoading = true;

        private readonly ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name)
                    return;

                _name = value;

                if (!_isLoading)
                    HasProfileInfoChanged();

                OnPropertyChanged();
            }
        }

        private string _ipRange;
        public string IPRange
        {
            get { return _ipRange; }
            set
            {
                if (value == _ipRange)
                    return;

                _ipRange = value;

                if (!_isLoading)
                    HasProfileInfoChanged();

                OnPropertyChanged();
            }
        }

        private string _group;
        public string Group
        {
            get { return _group; }
            set
            {
                if (value == _group)
                    return;

                _group = value;

                if (!_isLoading)
                    HasProfileInfoChanged();

                OnPropertyChanged();
            }
        }

        ICollectionView _groups;
        public ICollectionView Groups
        {
            get { return _groups; }
        }

        private IPScannerProfileInfo _profileInfo;

        private bool _profileInfoChanged;
        public bool ProfileInfoChanged
        {
            get { return _profileInfoChanged; }
            set
            {
                if (value == _profileInfoChanged)
                    return;

                _profileInfoChanged = value;
                OnPropertyChanged();
            }
        }

        public IPScannerProfileViewModel(Action<IPScannerProfileViewModel> saveCommand, Action<IPScannerProfileViewModel> cancelHandler, List<string> groups, IPScannerProfileInfo profileInfo = null)
        {
            _saveCommand = new RelayCommand(p => saveCommand(this));
            _cancelCommand = new RelayCommand(p => cancelHandler(this));

            _profileInfo = profileInfo ?? new IPScannerProfileInfo();

            Name = _profileInfo.Name;
            IPRange = _profileInfo.IPRange;
            
            // Get the group, if not --> get the first group (ascending), fallback --> default group 
            Group = string.IsNullOrEmpty(_profileInfo.Group) ? (groups.Count > 0 ? groups.OrderBy(x => x).First() : LocalizationManager.GetStringByKey("String_Default")) : _profileInfo.Group;

            _groups = CollectionViewSource.GetDefaultView(groups);
            _groups.SortDescriptions.Add(new SortDescription());

            _isLoading = false;
        }

        private void HasProfileInfoChanged()
        {
            ProfileInfoChanged = (_profileInfo.Name != Name) || (_profileInfo.IPRange != IPRange) || (_profileInfo.Group != Group);
        }
    }
}