﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using BleLab.Commands;
using BleLab.Commands.Device;
using BleLab.Messages;
using BleLab.Model;
using BleLab.ViewModels.Characteristic;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    public class DeviceViewModel : Conductor<CharacteristicViewModel>, IHandle<CharacteristicRenamedMessage>
    {
        private readonly DeviceInfo _deviceInfo;
        private CharacteristicInfoViewModel _selectedCharacteristic;
        private readonly Dictionary<Guid, CharacteristicViewModel> _characteristicsCached = new Dictionary<Guid, CharacteristicViewModel>();
        private readonly Dictionary<ServiceViewModel, List<CharacteristicInfoViewModel>> _servicesCharacteristics = new Dictionary<ServiceViewModel, List<CharacteristicInfoViewModel>>();
        private readonly CommandRunner _commandRunner;
        private readonly IEventAggregator _eventAggregator;
        private bool _connected;
        private bool _showServices = true;
        private bool _isConnecting;
        private bool _showCharacteristics = true;
        private bool _onlyFavourites;
        private bool _searchResultEmpty;

        public DeviceViewModel(DeviceInfo deviceInfo)
        {
            _commandRunner = IoC.Get<CommandRunner>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _deviceInfo = deviceInfo;
        }
        
        public CollectionViewSource Characteristics { get; } = new CollectionViewSource();
        
        public string Name
        {
            get { return _deviceInfo.GivenName ?? _deviceInfo.Name; }
            set
            {
                _deviceInfo.GivenName = value;
                _deviceInfo.SaveAsync();
            }
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            private set
            {
                if (value == _isConnecting) return;
                _isConnecting = value;
                NotifyOfPropertyChange();
            }
        }

        public bool ShowServices
        {
            get { return _showServices; }
            set
            {
                _showServices = value;
                UpdateServicesAndCharacteristics();
            }
        }

        public bool ShowCharacteristics
        {
            get { return _showCharacteristics; }
            set
            {
                _showCharacteristics = value;
                UpdateServicesAndCharacteristics();
            }
        }

        public bool OnlyFavourites
        {
            get { return _onlyFavourites; }
            set
            {
                _onlyFavourites = value;
                UpdateServicesAndCharacteristics();
            }
        }

        public bool SearchResultEmpty
        {
            get { return _searchResultEmpty; }
            set
            {
                if (value == _searchResultEmpty) return;
                _searchResultEmpty = value;
                NotifyOfPropertyChange();
            }
        }

        public string MacAddress => _deviceInfo.MacAddress;

        public string DeviceId => _deviceInfo.DeviceId;
        
        public CharacteristicInfoViewModel SelectedCharacteristic
        {
            get { return _selectedCharacteristic; }
            set
            {
                if (Equals(value, _selectedCharacteristic)) return;

                _selectedCharacteristic = value;
                NotifyOfPropertyChange();

                if (SelectedCharacteristic != null)
                {
                    ActivateItem(GetOrCreateCharacteristicViewModel(SelectedCharacteristic));
                }
            }
        }

        public bool ShowPane => ShowPane1;

        public bool ShowPane1 { get; }

        public async void Disconnect()
        {
            await _commandRunner.Enqueue(new DisconnectDeviceCommand()).AsTask().ConfigureAwait(true);
            TryClose();
        }

        protected override async void OnActivate()
        {
            _eventAggregator.Subscribe(this);
            if (!_connected)
            {
                await ConnectToDevice().ConfigureAwait(true);
                UpdateServicesAndCharacteristics();
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventAggregator.Unsubscribe(this);
        }

        private CharacteristicViewModel GetOrCreateCharacteristicViewModel(CharacteristicInfoViewModel characteristic)
        {
            if (_characteristicsCached.ContainsKey(characteristic.Model.Uuid))
                return _characteristicsCached[characteristic.Model.Uuid];

            return _characteristicsCached[characteristic.Model.Uuid] = new CharacteristicViewModel(characteristic.Model);
        }
        
        private async Task ConnectToDevice()
        {
            try
            {
                IsConnecting = true;

                var result = await _commandRunner.Enqueue(new ConnectDeviceCommand(_deviceInfo)).AsTask().ConfigureAwait(true);
                if (result.Status != CommandStatus.Succeeded)
                {
                    await ShowCouldntConnectMessage().ConfigureAwait(true);
                    return;
                }

                _connected = true;
                _deviceInfo.IsNew = false;
                await _deviceInfo.SaveAsync();
                NotifyOfPropertyChange(nameof(MacAddress));

                _servicesCharacteristics.Clear();

                var servicesResult = await _commandRunner.Enqueue(new ListServicesCommand(_deviceInfo)).AsTask().ConfigureAwait(true);
                if (servicesResult.Status != CommandStatus.Succeeded)
                    return;

                foreach (var service in servicesResult.Services)
                {
                    var serviceCharacteristics = await LoadCharacteristics(service).ConfigureAwait(true);
                    _servicesCharacteristics
                        .Add(new ServiceViewModel(service), serviceCharacteristics.Select(t => new CharacteristicInfoViewModel(t))
                        .ToList());
                }
            }
            finally
            {
                IsConnecting = false;
            }
        }

        private async Task<List<CharacteristicInfo>> LoadCharacteristics(ServiceInfo serviceInfo)
        {
            var resultList = new List<CharacteristicInfo>();
            var result = await _commandRunner.Enqueue(new ListCharacteristicsCommand(serviceInfo) { HideInConsole = true }).AsTask().ConfigureAwait(true);
            if (result.Status != CommandStatus.Succeeded)
                return resultList;

            foreach (var gattCharacteristic in result.Characteristics)
            {
                resultList.Add(gattCharacteristic);
            }

            return resultList;
        }
        
        private void UpdateServicesAndCharacteristics()
        {
            SearchResultEmpty = false;
            Characteristics.Source = null;

            if (ShowServices)
            {
                var services = new List<ServiceViewModel>();
                foreach (var serviceCharacteristics in _servicesCharacteristics)
                {
                    var service = serviceCharacteristics.Key;
                    var characteristics = serviceCharacteristics.Value;

                    service.Characteristics.Clear();
                    
                    foreach (var characteristic in characteristics.Where(CharacteristicIsVisible))
                    {
                        service.Characteristics.Add(characteristic);
                    }

                    if (service.Characteristics.Any() || !OnlyFavourites || service.IsFavourite)
                        services.Add(service);
                }

                Characteristics.IsSourceGrouped = true;
                Characteristics.Source = services;
                Characteristics.ItemsPath = new PropertyPath(nameof(ServiceViewModel.Characteristics));

                SearchResultEmpty = services.Count == 0;
            }
            else
            {
                Characteristics.ItemsPath = null;
                Characteristics.IsSourceGrouped = false;
                var characteristics = _servicesCharacteristics.Values.SelectMany(t => t).Where(CharacteristicIsVisible).ToList();
                Characteristics.Source = characteristics;

                SearchResultEmpty = characteristics.Count == 0;
            }
        }

        private bool CharacteristicIsVisible(CharacteristicInfoViewModel characteristic)
        {
            if (!ShowCharacteristics)
                return false;

            if (OnlyFavourites && !characteristic.Model.IsFavourite)
                return false;

            return true;
        }
        
        private async Task ShowCouldntConnectMessage()
        {
            var md = new MessageDialog("Could not connect to device. Check if it is reachable and charged and app has permissions.", "Error");
            md.Commands.Add(new UICommand("Bluetooth settings", async x => { await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth")); }));
            md.Commands.Add(new UICommand("Permissions settings", async x => { await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-customdevices")); }));
            md.Commands.Add(new UICommand("Ok"));
            md.DefaultCommandIndex = 2;
            await md.ShowAsync();
            TryClose();
        }

        public void Handle(CharacteristicRenamedMessage message)
        {
            if (SelectedCharacteristic != null && SelectedCharacteristic.Info.Uuid == message.CharacteristicInfo.Uuid)
            {
                SelectedCharacteristic.Refresh();
                return;
            }

            // long path
            var characteristic = _servicesCharacteristics.Values.SelectMany(t => t).FirstOrDefault(t => t.Info.Uuid == message.CharacteristicInfo.Uuid);
            characteristic?.Refresh();
        }
    }
}
