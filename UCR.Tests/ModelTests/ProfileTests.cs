﻿using System;
using NUnit.Framework;
using UCR.Core;
using UCR.Core.Models.Binding;
using UCR.Core.Models.Device;
using UCR.Core.Models.Profile;
using UCR.Plugins.ButtonToButton;
using UCR.Tests.Factory;

namespace UCR.Tests.ModelTests
{
    [TestFixture]
    internal class ProfileTests
    {
        private Context _context;
        private Profile _profile;
        private string _profileName;

        [SetUp]
        public void Setup()
        {
            _context = new Context();
            _context.ProfilesManager.AddProfile("Base profile");
            _profile = _context.Profiles[0];
            _profileName = "Test";
        }

        [Test]
        public void AddChildProfile()
        {
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            _profile.AddNewChildProfile(_profileName);
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(1));
            Assert.That(_profile.ChildProfiles[0].Title, Is.EqualTo(_profileName));
            Assert.That(_profile.ChildProfiles[0].ParentProfile, Is.EqualTo(_profile));
            Assert.That(_profile.ChildProfiles[0].Guid, Is.Not.EqualTo(Guid.Empty));
            Assert.That(_profile.IsActive, Is.Not.True);
            Assert.That(_context.IsNotSaved, Is.True);
        }
        
        [Test]
        public void RemoveChildProfile()
        {
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            _profile.AddNewChildProfile(_profileName);
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(1));
            Assert.That(_profile.ChildProfiles[0].Title, Is.EqualTo(_profileName));
            _profile.ChildProfiles[0].Remove();
            Assert.That(_profile.ChildProfiles.Count, Is.EqualTo(0));
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void RenameProfile()
        {
            var newName = "Renamed profile";
            Assert.That(_profile.Rename(newName), Is.True);
            Assert.That(_profile.Title, Is.EqualTo(newName));
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void AddPlugin()
        {
            var pluginName = "Test plugin";
            _profile.AddPlugin(new ButtonToButton(), pluginName);
            var plugin = _profile.Plugins[0];
            Assert.That(plugin, Is.Not.Null);
            Assert.That(plugin.Title, Is.EqualTo(pluginName));
            Assert.That(plugin.Inputs, Is.Not.Null);
            Assert.That(plugin.Outputs, Is.Not.Null);
            Assert.That(plugin.ParentProfile, Is.EqualTo(_profile));
            Assert.That(_context.IsNotSaved, Is.True);
        }

        [Test]
        public void GetDevice()
        {
            var deviceBinding = new DeviceBinding(null, null, DeviceIoType.Input)
            {
                DeviceType = DeviceType.Joystick,
                DeviceNumber = 0,
                IsBound = true
            };
            Assert.That(_profile.GetDevice(deviceBinding), Is.Null);
            var guid = _context.DeviceGroupsManager.AddDeviceGroup("Test joysticks", DeviceType.Joystick);
            _context.DeviceGroupsManager.GetDeviceGroup(deviceBinding.DeviceType, guid).Devices = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            Assert.That(guid, Is.Not.EqualTo(Guid.Empty));
            _profile.SetDeviceGroup(deviceBinding.DeviceIoType, deviceBinding.DeviceType, guid);
            Assert.That(_context.IsNotSaved, Is.True);
            Assert.That(_profile.GetDevice(deviceBinding), Is.Not.Null);
            Assert.That(_profile.GetDevice(deviceBinding).Guid, Is.EqualTo(_profile.GetDeviceList(deviceBinding)[0].Guid));
        }

        [Test]
        public void GetDeviceList()
        {
            var deviceBinding = new DeviceBinding(null, null, DeviceIoType.Input)
            {
                DeviceType = DeviceType.Joystick,
                DeviceNumber = 0,
                IsBound = true
            };
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Empty);
            var guid = _context.DeviceGroupsManager.AddDeviceGroup("Test joysticks", DeviceType.Joystick);
            _profile.SetDeviceGroup(deviceBinding.DeviceIoType, deviceBinding.DeviceType, guid);
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Not.Null.And.Empty);
            _context.DeviceGroupsManager.GetDeviceGroup(deviceBinding.DeviceType, guid).Devices = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            Assert.That(_profile.GetDeviceList(deviceBinding), Is.Not.Empty);
        }

        [Test]
        public void GetDeviceFromParent()
        {
            var deviceBinding = new DeviceBinding(null, null, DeviceIoType.Input)
            {
                DeviceType = DeviceType.Joystick,
                DeviceNumber = 0,
                IsBound = true
            };

            var singleGuid = _context.DeviceGroupsManager.AddDeviceGroup("Single device", DeviceType.Joystick);
            var deviceList = DeviceFactory.CreateDeviceList("Dummy", "Provider", 1);
            _context.DeviceGroupsManager.GetDeviceGroup(deviceBinding.DeviceType, singleGuid).Devices = deviceList;
            var multipleGuid = _context.DeviceGroupsManager.AddDeviceGroup("Test joysticks", DeviceType.Joystick);
            _context.DeviceGroupsManager.GetDeviceGroup(deviceBinding.DeviceType, multipleGuid).Devices = DeviceFactory.CreateDeviceList("Dummy", "Provider", 4);

            _profile.SetDeviceGroup(deviceBinding.DeviceIoType, deviceBinding.DeviceType, multipleGuid);

            _profile.AddNewChildProfile("Child profile");
            var childProfile = _profile.ChildProfiles[0];
            childProfile.SetDeviceGroup(deviceBinding.DeviceIoType, deviceBinding.DeviceType, singleGuid);

            Assert.That(childProfile.GetDevice(deviceBinding).Guid, Is.EqualTo(deviceList[0].Guid));

            Assert.That(_profile.GetDevice(deviceBinding).Guid, Is.Not.EqualTo(childProfile.GetDevice(deviceBinding).Guid));
            deviceBinding.DeviceNumber = 1;
            Assert.That(_profile.GetDevice(deviceBinding).Guid, Is.EqualTo(childProfile.GetDevice(deviceBinding).Guid));
            Assert.That(childProfile.GetDeviceList(deviceBinding).Count, Is.EqualTo(4));
        }

        [Test]
        public void ActivateProfile()
        {
            // TODO
            //_profile.
        }
    }
}
