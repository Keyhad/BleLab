using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Characteristic
{
    public abstract class CharacteristicCommandBase : CommandBase
    {
        private readonly DeviceController _deviceController;

        protected CharacteristicCommandBase(CharacteristicInfo characteristicInfo)
        {
            CharacteristicInfo = characteristicInfo;
            _deviceController = IoC.Get<DeviceController>();
        }

        public CharacteristicInfo CharacteristicInfo { get; }

        protected GattCharacteristic Characteristic
        {
            get
            {
                if (_deviceController.ConnectedDevice == null)
                    throw new InvalidOperationException("Device not connected");

                if (_deviceController.ConnectedDevice.DeviceId != CharacteristicInfo.Service.Device.DeviceId)
                    throw new InvalidOperationException("Targeted device not connected");

                var result = _deviceController.ConnectedDevice.GetGattServicesForUuidAsync(CharacteristicInfo.Service.Uuid).GetResults();
                return result.Services[0].GetCharacteristicsForUuidAsync(CharacteristicInfo.Uuid).GetResults().Characteristics[0];
            }
        }
    }
}