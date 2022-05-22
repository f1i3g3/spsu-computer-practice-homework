using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Client
{
	public class MessageHandler
	{
		private MainWindow window;
		private Bitmap image;

		private GrpcChannel channel;
		private FiltersApp.FiltersAppClient client;

		private CancellationTokenSource cancelFlag;

		public async Task<List<string>> GetFilters()
		{
			try
			{
				var reply = await client.LoadFiltersListAsync(new Empty());

				return reply.FiltersList.ToList();
			}
			catch
			{
				return null;
			}
		}

		public void SetImage(string path)
		{
			image = new Bitmap(path);
		}

		public Bitmap GetImage()
		{
			return image;
		}

		public void SaveImage(string path)
		{
			image.Save(path);
		}

		public async Task SendAndReceiveImage(string filterName)
		{
			cancelFlag = new CancellationTokenSource();
			CancellationToken token = cancelFlag.Token;

			byte[] imageBytes = null;
			using (var ms = new MemoryStream())
			{
				image.Save(ms, ImageFormat.Bmp);
				imageBytes = ms.GetBuffer();
				
			}

			using var call = client.ApplyFilter(new FilterRequest
			{
				FilterName = filterName,
				ImageBytes = ByteString.CopyFrom(imageBytes)
			}, cancellationToken: token);

			Bitmap newImage = null;
			try
			{
				await foreach (var response in call.ResponseStream.ReadAllAsync(token))
				{
					switch (response.ReplyCase)
					{
						case FilterReply.ReplyOneofCase.CurrentProgress:
							window.progressBar.Value = response.CurrentProgress.Progress; // сделать красивый и последовательный вывод?
							break;
						case FilterReply.ReplyOneofCase.Image:
							if (response.Image.ErrorFlag == 0)
							{
								using (var ms = new MemoryStream(response.Image.ImageBytes.ToByteArray()))
								{
									newImage = new Bitmap(ms);
								}

								image = new Bitmap(newImage);
								// window.progressBar.Value = 100;
							}

							window.progressBar.Value = 0;

							break;
					}
				}
			}
			catch (OperationCanceledException)
			{
				window.progressBar.Value = 0;
				throw new OperationCanceledException();
			}
			finally
			{
				newImage?.Dispose();
				call?.Dispose();
				cancelFlag?.Dispose();
			}
		}

		public void CancelOperation()
		{
			try
			{
				cancelFlag?.Cancel();
			}
			catch (ObjectDisposedException)
			{

			}
			finally
			{
				cancelFlag?.Dispose();
			}
		}

		public void OpenConnection(MainWindow window = null)
		{
			if (window is not null)
			{
				this.window = window;
				this.window.progressBar.Value = 0;
			}

			channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
			{
				MaxReceiveMessageSize = null,
				MaxSendMessageSize = null,
				ThrowOperationCanceledOnCancellation = true
			});

			client = new FiltersApp.FiltersAppClient(channel);
		}

		public async void CloseConnection()
		{
			CancelOperation();

			client = null;

			image?.Dispose();

			await channel.ShutdownAsync();
			channel?.Dispose();
		}
	}
}