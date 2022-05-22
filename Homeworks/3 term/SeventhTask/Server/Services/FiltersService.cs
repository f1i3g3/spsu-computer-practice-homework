using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;

namespace Server
{
	public class FiltersService : FiltersApp.FiltersAppBase
	{
		private readonly ILogger<FiltersService> logger;
		private readonly IOptions<FiltersSettings> options;

		public override async Task ApplyFilter(FilterRequest request, IServerStreamWriter<FilterReply> responseStream, ServerCallContext context)
		{
			var token = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken).Token;
			byte[] bytes = null;
			var reply = new FilterReply
			{
				Image = new ImageReply()
			};
			Bitmap image = null;

			try
			{
				logger.LogInformation("Receiving an image..");

				using var inMS = new MemoryStream(request.ImageBytes.ToByteArray());

				try
				{
					image = new Bitmap(inMS);
				}
				catch (OperationCanceledException)
				{
					throw new OperationCanceledException();
				}
				catch
				{
					logger.LogInformation("Error in receiving!\n");
					reply.Image.ErrorFlag = -1;
				}

				if (reply.Image.ErrorFlag != -1)
				{
					try
					{
						var filter = new ImageFiltering(image, responseStream, token);

						logger.LogInformation("Applying filter..");
						filter.Apply(request.FilterName);
					}
					catch (OperationCanceledException)
					{
						throw new OperationCanceledException();
					}
					catch
					{
						logger.LogInformation("Error in filtering!\n");
						reply.Image.ErrorFlag = -1;
					}
				}

				if (reply.Image.ErrorFlag != -1)
				{
					using var outMS = new MemoryStream();

					try
					{
						image.Save(outMS, ImageFormat.Bmp);
						bytes = outMS.GetBuffer();
					}
					catch (OperationCanceledException)
					{
						throw new OperationCanceledException();
					}
					catch
					{
						logger.LogInformation("Error in saving!\n");
						reply.Image.ErrorFlag = -1;
					}
				}

				if (reply.Image.ErrorFlag != -1)
				{
					reply.Image.ImageBytes = ByteString.CopyFrom(bytes);
				}
			}
			catch (OperationCanceledException)
			{
				if (reply.Image.ErrorFlag != -1)
				{
					reply.Image.ErrorFlag = 1;
				}

				logger.LogInformation("Operation was cancelled!");
			}
			finally
			{
				image?.Dispose();
			}

			logger.LogInformation("Sending a reply..");
			await responseStream.WriteAsync(reply);

			logger.LogInformation("Finished!");
		}

		public override Task<FiltersListReply> LoadFiltersList(Empty request, ServerCallContext context)
		{
			var reply = new FiltersListReply();
			reply.FiltersList.AddRange(options.Value.ListOfFilters);

			logger.LogInformation("Sending a list of filters.");

			return Task.FromResult(reply);
		}

		public FiltersService(ILogger<FiltersService> logger, IOptions<FiltersSettings> options)
		{
			this.logger = logger;
			this.options = options;
		}
	}
}