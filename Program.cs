using System.Text;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

string deviceKey = "sqHOzHwaWNcqV8ISxKPMp8gzFj4z3qKZGdUeqglvnIM=";
string deviceId = "iothubtraining01";
string iotHubHostName = "iothubtraining-linkedinlearning.azure-devices.net";

var deviceAuthentication = new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey);
var deviceClient = DeviceClient.Create(iotHubHostName, deviceAuthentication, TransportType.Mqtt);

await deviceClient.SetMethodHandlerAsync("my-method", (request, context) => {
    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(@"{""result"":""Tout est ok.""}"), 200));
}, null);

var Rand = new Random();
var _messageId = 1;

while(true){
    double currentTemperature = 20 + Rand.NextDouble() * 15;
    double currentHumidity = 60 + Rand.NextDouble() * 20;

    var telemetryDataPoint = new
    {
        messageId = _messageId++,
        deviceId,
        temperature = currentTemperature,
        humidity = currentHumidity,
        kind = "event"
    };

    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
    var message = new Message(Encoding.UTF8.GetBytes(messageString));

    await deviceClient.SendEventAsync(message);
    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

    await Task.Delay(1000);
}

Console.WriteLine("Prêt !");
Console.ReadLine();