using NLog;

namespace Proxima
{
	public static class Log
	{
		private static Logger LoggerEngine = LogManager.GetLogger("Engine");
		private static Logger LoggerApplication = LogManager.GetLogger("Application");

		public static void Info(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerEngine.Info(o.ToString(), args);
		}

		public static void Debug(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerEngine.Debug(o.ToString(), args);
		}

		public static void Warn(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerEngine.Warn(o.ToString(), args);
		}

		public static void Error(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerEngine.Error(o.ToString(), args);
		}

		private static void InfoApp(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerApplication.Info(o.ToString(), args);
		}

		private static void DebugApp(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerApplication.Debug(o.ToString(), args);
		}

		private static void WarnApp(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerApplication.Warn(o.ToString(), args);
		}

		private static void ErrorApp(object? o, params object[] args)
		{
			o ??= "Null";
			LoggerApplication.Error(o.ToString(), args);
		}
	}
}