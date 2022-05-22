using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Server;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StressTester
{
	public class StressTesterUnit
	{
		private readonly AutoResetEvent handler;
		private readonly int timeLimit;

		private DateTime start;
		private DateTime finish;

		private GrpcChannel channel;
		private FiltersApp.FiltersAppClient client;
		private CancellationToken token;

		private readonly byte[] original;
		private byte[] result;

		private volatile bool isWorking;
		private volatile bool isCompleted;

		private void StartOperation()
		{
			try
			{
				isWorking = true;
				start = DateTime.Now;

				channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
				{
					MaxReceiveMessageSize = null,
					MaxSendMessageSize = null,
				});
				client = new FiltersApp.FiltersAppClient(channel);
				token = new CancellationToken();

				var filter = client.LoadFiltersList(new Empty()).FiltersList.ToList();

				result = ApplyFilter(filter[2]).Result; // Gauss5x5

				finish = DateTime.Now;
			}
			finally
			{
				channel?.ShutdownAsync();
			}
		}

		private Task<byte[]> ApplyFilter(string filterName)
		{
			try
			{
				using var call = client.ApplyFilter(new FilterRequest
				{
					FilterName = filterName,
					ImageBytes = ByteString.CopyFrom(original)
				}, cancellationToken: token);

				byte[] output = null;
				var task = Task.Run(async () =>
				{
					await foreach (var response in call.ResponseStream.ReadAllAsync(token))
					{
						switch (response.ReplyCase)
						{
							case FilterReply.ReplyOneofCase.CurrentProgress:
								break;
							case FilterReply.ReplyOneofCase.Image:
								output = response.Image.ImageBytes.ToByteArray();
								break;
						}
					}
				});
				task.Wait();

				isCompleted = true;
				isWorking = false;
				handler.Set();

				return Task.FromResult(output);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public int OperationTime()
		{
			int currTime = 0;

			if (isWorking || isCompleted)
			{
				handler.WaitOne(timeLimit);

				currTime = result is null ? -1 : (int)(finish - start).TotalMilliseconds;
			}

			return currTime;
		}

		public StressTesterUnit(byte[] data, int limit)
		{
			handler = new AutoResetEvent(false);
			timeLimit = limit * 1000;

			original = data;

			isWorking = false;
			isCompleted = false;

			StartOperation();
		}
	}
}