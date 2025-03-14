using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.ReadResult;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Text;

namespace CheeseCaveDotnet;

class Device
{
        private static readonly int s_pin = 21; // Pin GPIO utilizado para controlar el ventilador.
    private static GpioController s_gpio; // Controlador GPIO.
    private static I2cDevice s_i2cDevice; // Dispositivo I2C para el sensor BME280.
    private static Bme280 s_bme280; // Sensor BME280 para medir temperatura y humedad.
    
    const double DesiredTempLimit = 5; // Rango aceptable por encima o por debajo de la temperatura deseada, en grados Fahrenheit.
    const double DesiredHumidityLimit = 10; // Rango aceptable por encima o por debajo de la humedad deseada, en porcentajes.
    const int IntervalInMilliseconds = 5000; // Intervalo en milisegundos para enviar telemetría a la nube.
    
    private static DeviceClient s_deviceClient; // Cliente del dispositivo para comunicarse con Azure IoT Hub.
    private static stateEnum s_fanState = stateEnum.off; // Estado actual del ventilador.
    
    private static readonly string s_deviceConnectionString = "YOUR DEVICE CONNECTION STRING HERE"; // Cadena de conexión del dispositivo.
    
    enum stateEnum
    {
        off, // Ventilador apagado.
        on, // Ventilador encendido.
        failed // Ventilador fallido.
    }
    
    private static void Main(string[] args)
    {
        // Inicializa el controlador GPIO y abre el pin para el ventilador.
        s_gpio = new GpioController();
        s_gpio.OpenPin(s_pin, PinMode.Output);
    
        // Configura el dispositivo I2C para el sensor BME280.
        var i2cSettings = new I2cConnectionSettings(1, Bme280.DefaultI2cAddress);
        s_i2cDevice = I2cDevice.Create(i2cSettings);
    
        // Inicializa el sensor BME280.
        s_bme280 = new Bme280(s_i2cDevice);
    
        // Muestra un mensaje en la consola.
        ColorMessage("Cheese Cave device app.\n", ConsoleColor.Yellow);
    
        // Crea el cliente del dispositivo utilizando la cadena de conexión.
        s_deviceClient = DeviceClient.CreateFromConnectionString(s_deviceConnectionString, TransportType.Mqtt);
    
        // Configura el manejador para el método directo "SetFanState".
        s_deviceClient.SetMethodHandlerAsync("SetFanState", SetFanState, null).Wait();
    
        // Inicia la monitorización de las condiciones y la actualización del gemelo digital.
        MonitorConditionsAndUpdateTwinAsync();
    
        // Espera a que el usuario presione Enter antes de cerrar el pin GPIO.
        Console.ReadLine();
        s_gpio.ClosePin(s_pin);
    }
    
    private static async void MonitorConditionsAndUpdateTwinAsync()
    {
        while (true)
        {
            // Lee los datos del sensor BME280.
            Bme280ReadResult sensorOutput = s_bme280.Read();
    
            // Actualiza el gemelo digital con los valores actuales de temperatura y humedad.
            await UpdateTwin(
                    sensorOutput.Temperature.Value.DegreesFahrenheit,
                    sensorOutput.Humidity.Value.Percent);
    
            // Espera el intervalo definido antes de la siguiente lectura.
            await Task.Delay(IntervalInMilliseconds);
        }
    }
    
    private static Task<MethodResponse> SetFanState(MethodRequest methodRequest, object userContext)
    {
        if (s_fanState is stateEnum.failed)
        {
            // Si el ventilador ha fallado, devuelve un error.
            string result = "{\"result\":\"Fan failed\"}";
            RedMessage("Direct method failed: " + result);
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
        }
        else
        {
            try
            {
                // Procesa la solicitud para cambiar el estado del ventilador.
                var data = Encoding.UTF8.GetString(methodRequest.Data);
                data = data.Replace("\"", "");
                s_fanState = (stateEnum)Enum.Parse(typeof(stateEnum), data);
                GreenMessage("Fan set to: " + data);
    
                // Escribe el estado del ventilador en el pin GPIO.
                s_gpio.Write(s_pin, s_fanState == stateEnum.on ? PinValue.High : PinValue.Low);
    
                // Devuelve una respuesta exitosa.
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            catch
            {
                // Si hay un error en el parámetro, devuelve un error.
                string result = "{\"result\":\"Invalid parameter\"}";
                RedMessage("Direct method failed: " + result);
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
        }
    }
    
    private static async Task UpdateTwin(double currentTemperature, double currentHumidity)
    {
        // Crea una colección de propiedades reportadas para el gemelo digital.
        var reportedProperties = new TwinCollection();
        reportedProperties["fanstate"] = s_fanState.ToString();
        reportedProperties["humidity"] = Math.Round(currentHumidity, 2);
        reportedProperties["temperature"] = Math.Round(currentTemperature, 2);
    
        // Actualiza las propiedades reportadas en el gemelo digital.
        await s_deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
    
        // Muestra un mensaje en la consola con el estado reportado.
        GreenMessage("Twin state reported: " + reportedProperties.ToJson());
    }
    
    private static void ColorMessage(string text, ConsoleColor clr)
    {
        // Muestra un mensaje en la consola con el color especificado.
        Console.ForegroundColor = clr;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    private static void GreenMessage(string text) =>
        ColorMessage(text, ConsoleColor.Green); // Muestra un mensaje en verde.
    
    private static void RedMessage(string text) =>
        ColorMessage(text, ConsoleColor.Red); // Muestra un mensaje en rojo.
}
