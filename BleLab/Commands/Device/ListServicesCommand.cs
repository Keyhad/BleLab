using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Device
{
    public class ListServicesCommand : CommandBase
    {
        private readonly DeviceInfo _deviceInfo;
        private readonly DeviceController _deviceController;
        private readonly InfoManager _infoManager;

        public ListServicesCommand(DeviceInfo deviceInfo = null)
        {
            _deviceInfo = deviceInfo;
            _deviceController = IoC.Get<DeviceController>();
            _infoManager = IoC.Get<InfoManager>();
        }

        public ICollection<ServiceInfo> Services { get; private set; }

        protected override async Task DoExecuteAsync()
        {
            var device = _deviceController.ConnectedDevice;
            if (_deviceInfo != null && _deviceInfo.DeviceId != device.DeviceId)
                throw new InvalidOperationException("Command executed on a wrong device");
            
            if (device == null)
                throw new InvalidOperationException("Device not connected");

            Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceServicesResult gattServices = await device.GetGattServicesAsync();
            Services = _infoManager.GetAllServicesInfo(_deviceInfo, gattServices.Services);
        }
    }
}
