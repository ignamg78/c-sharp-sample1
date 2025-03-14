# CheeseCaveDotnet

## Description
CheeseCaveDotnet es un proyecto que utiliza sensores y dispositivos IoT para monitorear y controlar las condiciones ambientales de una cueva de quesos. El proyecto se conecta a Azure IoT Hub para enviar telemetría y recibir comandos, asegurando que los quesos se mantengan en condiciones óptimas.

## Setup Instructions
1. Clona el repositorio:
    ```sh
    git clone https://github.com/tu_usuario/CheeseCaveDotnet.git
    cd CheeseCaveDotnet
    ```

2. Instala el SDK de .NET 6.0 si no lo tienes instalado. Puedes descargarlo desde [aquí](https://dotnet.microsoft.com/download/dotnet/6.0).

3. Restaura las dependencias del proyecto:
    ```sh
    dotnet restore
    ```

4. Configura tu Azure IoT Hub y actualiza las credenciales en el código fuente según sea necesario.

5. Compila el proyecto:
    ```sh
    dotnet build
    ```

6. Ejecuta el proyecto:
    ```sh
    dotnet run
    ```

## Usage
Para utilizar el proyecto, asegúrate de tener los sensores y dispositivos correctamente conectados. El proyecto monitoreará la temperatura y la humedad, enviará telemetría a Azure IoT Hub y recibirá comandos desde el hub.

Ejemplo de uso:
```csharp
// Código de ejemplo para inicializar el sensor y enviar datos
s_gpio = new GpioController();
s_i2cDevice = I2cDevice.Create(new I2cConnectionSettings(1, Bme280.DefaultI2cAddress));
s_bme280 = new Bme280(s_i2cDevice);

// Lógica para leer datos del sensor y enviarlos a Azure IoT Hub