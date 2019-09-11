using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading;

namespace PowerShell_Test
{
	class Program
	{
		private static int _counter = 0;
		private static readonly Random _random = new Random();

		const int MaxThreads = 10;

		static void Main()
		{
			Thread[] threads = new Thread[MaxThreads];

			for (int i = 0; i < MaxThreads; ++i)
			{
				threads[i] = new Thread(RunPowerShell);

				lock (_random) Console.WriteLine("Starting thread #" + ++_counter);
				threads[i].Start();

				Thread.Sleep(_random.Next(100, 500));
			}

			while (_counter > 0) Thread.Sleep(500);

			Console.WriteLine("Press any key to continue ...");
			while (!Console.KeyAvailable) Thread.Sleep(200);
		}



		private static void RunPowerShell()
		{
			using (PowerShell ps = PowerShell.Create())
			{
				try
				{
					Collection<PSObject> retVal;

					ps.AddScript(File.ReadAllText(@".\Test.ps1"));
					retVal = ps.Invoke();

					if (!hasFailed(ps) && retVal.Count > 0)
					{
						lock (_random) Console.WriteLine("Success.");
					}
				}
				catch (RuntimeException rex)
				{
					lock (_random)
					{
						ConsoleColor fc = Console.ForegroundColor, bc = Console.BackgroundColor;

						Console.ForegroundColor = ConsoleColor.Red;
						Console.BackgroundColor = ConsoleColor.Black;

						Console.WriteLine("Exception: " + rex.Message);
						Console.WriteLine(rex.ErrorRecord.InvocationInfo.PositionMessage);

						Console.ForegroundColor = fc;
						Console.BackgroundColor = bc;
					}
				}
				catch (Exception ex)
				{
					ConsoleColor fc = Console.ForegroundColor, bc = Console.BackgroundColor;

					Console.ForegroundColor = ConsoleColor.Red;
					Console.BackgroundColor = ConsoleColor.Black;

					lock (_random) Console.WriteLine("Exception: " + ex.Message);

					Console.ForegroundColor = fc;
					Console.BackgroundColor = bc;
				}

				lock (_random) Console.WriteLine("Finishing thread #" + --_counter);
			}
		}

		private static bool hasFailed(PowerShell ps, string context = null)
		{
			lock (_random)
			{
				bool hasErrors = true;
				ConsoleColor fc = Console.ForegroundColor, bc = Console.BackgroundColor;

				Console.ForegroundColor = ConsoleColor.Red;
				Console.BackgroundColor = ConsoleColor.Black;

				if (!string.IsNullOrWhiteSpace(context)) context = " " + context;

				if (ps.HadErrors)
				{
					StringBuilder sb = new StringBuilder(1024);

					foreach (ErrorRecord er in ps.Streams.Error.ReadAll()) sb.AppendLine(er.ToString());

					Console.WriteLine($"Error{context}:{Environment.NewLine}{sb.ToString()}");
				}
				else if (ps.InvocationStateInfo.State == PSInvocationState.Failed) Console.WriteLine($"Failed{context}: {ps.InvocationStateInfo.Reason.Message}");
				else hasErrors = false;

				Console.ForegroundColor = fc;
				Console.BackgroundColor = bc;

				return hasErrors;
			}
		}
	}
}
