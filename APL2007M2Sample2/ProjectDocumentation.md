# Project Documentation

## Overview
CheeseCaveDotnet es un proyecto que utiliza sensores y dispositivos IoT para monitorear y controlar las condiciones ambientales de una cueva de quesos. El proyecto se conecta a Azure IoT Hub para enviar telemetría y recibir comandos.

## Features
- Monitoreo de temperatura y humedad utilizando sensores BME280.
- Control de pines GPIO para interactuar con dispositivos externos.
- Envío de telemetría a Azure IoT Hub.
- Recepción de comandos desde Azure IoT Hub.
- Configuración de límites deseados de temperatura y humedad.

## Requirements
- .NET 6.0 SDK
- Azure IoT Hub
- Sensor BME280
- Controlador GPIO compatible

## Constraints
- El proyecto está diseñado para ejecutarse en dispositivos con soporte para .NET 6.0.
- Requiere conexión a Internet para comunicarse con Azure IoT Hub.
- Los sensores y dispositivos deben estar correctamente conectados y configurados.

## Dependencies
- `System.Device.Gpio`
- `System.Device.I2c`
- `Iot.Device.Bmxx80`
- `Microsoft.Azure.Devices.Client`
- `Microsoft.Azure.Devices.Shared`
- `DotNetty.Buffers`
- `DotNetty.Codecs`
- `DotNetty.Codecs.Mqtt`
- `DotNetty.Common`
- `Microsoft.Extensions.Logging.Abstractions`
- `SixLabors.ImageSharp`
- `System.Drawing.Common`
- `System.IO.Ports`
- `System.Management`
- `System.Text.Json`
- `UnitsNet`
- `Microsoft.Azure.Amqp`
- `Newtonsoft.Json`
- `WindowsAzure.Storage`

## Summary
Este proyecto permite monitorear y controlar las condiciones ambientales de una cueva de quesos utilizando sensores IoT y Azure IoT Hub. Proporciona una solución completa para la gestión de la temperatura y la humedad, asegurando que los quesos se mantengan en condiciones óptimas.