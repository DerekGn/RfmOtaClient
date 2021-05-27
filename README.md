# RfmOTAClient

The RfmOTAClient is a console application that allows a hex file to be flashed to a remote device that is currently executing the Rfm bootloader. The RfmOTAClient uses an RfmUsb dongle to transmit and receive packets from the remote device executing the Rfm bootloader.

The radio module used by Rfm devices and the RfmUsb dongle are based on the HopeRf Frm69 modules.

The console will run on Windows, Linux and Mac Os. It should also run on raspberry pi.

## Requirements

RfmOtaClient requires .Net Core 3.1 runtime to be installed on the host PC

See [here](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net50) for instructions on installing on Windows

See [here](https://docs.microsoft.com/en-us/dotnet/core/install/linux) for instructions on installing on Linux

## Linux

### Finding the RfmUsb

To find the RfmUsb device in Linux execute the following:

``` bash
ls /dev/ttyACM*
```

### Serial Port Permissions

On Linux you will need permissions to access a serial device as a non root user. Without serial port permissions assigned to the user account executing the RfmOtaClient console, the RfmOtaConsole app can not access the RfmUsb device. The cleanest method is to assign the user to the dialout group.

First check user is not part of the dialout group by executing the following, where \<username> is the name of the current user:

``` bash
id -Gn <username>
```

This results in something like the following **\<username> adm cdrom sudo dip plugdev lpadmin sambashare kvm**

Add the user to the dialout group

``` bash
sudo usermod -a -G dialout <username>
```

The user assigned to the dialout group will need to login and log out to have the updated permissions applied.