using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;

namespace Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MessageHandler handler;

		public MainWindow()
		{
			handler = new MessageHandler();
			DataContext = handler;

			InitializeComponent();

			handler.OpenConnection(this);
			Loaded += LoadFilters;
		}

		private async void LoadFilters(object sender, RoutedEventArgs e)
		{
			try
			{
				List<string> filtersList = await handler.GetFilters();

				if (filtersList is null)
				{
					throw new Exception();
				}

				foreach (string filter in filtersList)
				{
					filtersBox.Items.Add(filter);
				}

				// applyButton.IsEnabled = true;
				loadButton.IsEnabled = true;
				saveButton.IsEnabled = true;
			}
			catch
			{
				MessageBox.Show("Connection error!\nPlease, restart an application.", "Error!");
				Application.Current.Shutdown();
			}
		}

		private void LoadButtonClick(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog
			{
				Filter = "Images (*.jpg; *.png; *.bmp)|*.jpg;*.png;*.bmp" + "|All files|*.*",
				Title = "Open"
			};

			if ((bool)ofd.ShowDialog())
			{
				try
				{
					handler.SetImage(ofd.FileName);

					BitmapSource src = Imaging.CreateBitmapSourceFromHBitmap(handler.GetImage().GetHbitmap(),
						IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(handler.GetImage().Width, handler.GetImage().Height));

					imagePanel.Source = src;

					applyButton.IsEnabled = true;
				}
				catch
				{
					MessageBox.Show("Image is not found!", "Error!");
				}
			}
		}

		private void SaveButtonClick(object sender, RoutedEventArgs e)
		{
			var sfd = new SaveFileDialog
			{
				Filter = "All files| *.*",
				Title = "Save as"
			};

			if ((bool)sfd.ShowDialog())
			{
				try
				{
					handler.SaveImage(sfd.FileName);
				}
				catch (ArgumentNullException)
				{
					MessageBox.Show("Image is not selected!", "Error!");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!");
				}
			}
		}

		private async void ApplyButtonClick(object sender, RoutedEventArgs e)
		{
			try
			{
				loadButton.IsEnabled = false;
				saveButton.IsEnabled = false;
				applyButton.IsEnabled = false;
				cancelButton.IsEnabled = true;

				var filterName = (string)filtersBox.SelectedItem;
				if (filterName is null)
				{
					throw new Exception("Filter is not selected!");
				}

				await handler.SendAndReceiveImage(filterName);

				Bitmap image = handler.GetImage();
				if (image is null)
				{
					throw new Exception("Error with getting an image!");
				}

				BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(),
					IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(image.Width, image.Height));
				imagePanel.Source = source;
			}
			catch (OperationCanceledException)
			{
				applyButton.IsEnabled = true;
			}
			catch (Exception ex)
			{
				applyButton.IsEnabled = true;
				MessageBox.Show(ex.Message, "Error!");
			}
			finally
			{
				loadButton.IsEnabled = true;
				saveButton.IsEnabled = true;
				cancelButton.IsEnabled = false;
			}
		}

		private void CancelButtonClick(object sender, RoutedEventArgs e)
		{
			try
			{
				progressBar.Value = 0;

				handler.CancelOperation();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!");
			}
		}

		private void OnClosed(object sender, EventArgs e)
		{
			handler.CloseConnection();
		}
	}
}