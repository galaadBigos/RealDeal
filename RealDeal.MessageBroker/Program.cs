using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RealDeal.MessageBrocker
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var factory = new ConnectionFactory
			{
				HostName = "rabbitmq",
			};

			IConnection? connection = null;
			int retries = 12;

			while (retries > 0)
			{
				try
				{
					connection = await factory.CreateConnectionAsync();
					Console.WriteLine("Connected to RabbitMQ!");
					break;
				}
				catch (Exception ex)
				{
					retries--;
					Console.WriteLine($"Failed to connect to RabbitMQ. Retries left: {retries}. Error: {ex.Message}");
					await Task.Delay(5000);
				}
			}

			if (connection == null)
			{
				Console.WriteLine("Could not establish a connection to RabbitMQ. Exiting...");
				return;
			}

			using var betChannel = await connection.CreateChannelAsync();
			using var userChannel = await connection.CreateChannelAsync();

			await betChannel.QueueDeclareAsync(queue: "bets", durable: false, exclusive: false, autoDelete: false, arguments: null);
			await userChannel.QueueDeclareAsync(queue: "users", durable: false, exclusive: false, autoDelete: false, arguments: null);

			Console.WriteLine("Bet Event Queue");
			var betConsumer = new AsyncEventingBasicConsumer(betChannel);
			betConsumer.ReceivedAsync += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"[Bet Queue] {message}");
				return Task.CompletedTask;
			};
			await betChannel.BasicConsumeAsync("bets", autoAck: true, consumer: betConsumer);

			Console.WriteLine("User Event Queue");
			var userConsumer = new AsyncEventingBasicConsumer(userChannel);
			userConsumer.ReceivedAsync += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"[User Queue] {message}");
				return Task.CompletedTask;
			};
			await userChannel.BasicConsumeAsync("users", autoAck: true, consumer: userConsumer);

			var resetEvent = new ManualResetEventSlim(false);
			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				Console.WriteLine("Stopping message broker...");
				resetEvent.Set();
			};
			resetEvent.Wait();
		}
	}
}
