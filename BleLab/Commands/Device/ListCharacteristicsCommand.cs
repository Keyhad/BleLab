using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Device
{
    public class ListCharacteristicsCommand : CommandBase
    {
        private readonly ServiceInfo _serviceInfo;
        private readonly DeviceController _deviceController;
        private readonly InfoManager _infoManager;

        public ListCharacteristicsCommand(ServiceInfo serviceInfo)
        {
            _serviceInfo = serviceInfo;
            _deviceController = IoC.Get<DeviceController>();
            _infoManager = IoC.Get<InfoManager>();
        }

        public ICollection<CharacteristicInfo> Characteristics { get; private set; }

        protected override async Task DoExecuteAsync()
        {
            if (_deviceController.ConnectedDevice == null)
                throw new InvalidOperationException("Device not connected.");

            if (_deviceController.ConnectedDevice.DeviceId != _serviceInfo.Device.DeviceId)
                throw new InvalidOperationException("Command executed against not connected device.");

            // ToDo: cache services on controller
            var results = await _deviceController.ConnectedDevice.GetGattServicesForUuidAsync(_serviceInfo.Uuid).GetResults().Services.AsEnumerable().First().GetCharacteristicsAsync();
            IReadOnlyList<Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic> characteristics = results.Characteristics;
            Characteristics = _infoManager.GetAllCharacteristicsInfo(_serviceInfo, characteristics);
        }
    }
}
