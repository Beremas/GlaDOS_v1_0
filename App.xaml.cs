using GlaDOS_v1_0.Storage.Services;
using Microsoft.Win32;
using System.Windows;

namespace GlaDOS_v1_0
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			SystemEvents.SessionEnding += OnSessionEnding;
		}

		private void OnSessionEnding(object sender, SessionEndingEventArgs e)
		{
			ChatHistoryService.Instance.SaveAll();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			// Save on normal app close too
			ChatHistoryService.Instance.SaveAll();

			SystemEvents.SessionEnding -= OnSessionEnding;
			base.OnExit(e);
		}

		[STAThread]
		public static void Main2()
		{
			var app = new App();
			var window = new MainWindow();
			app.Run(window);
		}
	}
}
