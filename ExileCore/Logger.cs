using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ExileCore;

public class Logger
{
	private static ILogger _instance;

	public static ILogger Log => _instance ?? (_instance = new LoggerConfiguration().MinimumLevel.ControlledBy(new LoggingLevelSwitch(LogEventLevel.Verbose)).WriteTo.Logger(delegate(LoggerConfiguration l)
	{
		l.Filter.ByIncludingOnly((LogEvent e) => e.Level == LogEventLevel.Information).WriteTo.File("Logs\\Info.log", LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, 1073741824L, null, buffered: false, shared: false, null, RollingInterval.Day, rollOnFileSizeLimit: false, 31);
	}).WriteTo.Logger(delegate(LoggerConfiguration l)
	{
		l.Filter.ByIncludingOnly((LogEvent e) => e.Level == LogEventLevel.Debug).WriteTo.File("Logs\\Debug.log", LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, 1073741824L, null, buffered: false, shared: false, null, RollingInterval.Day, rollOnFileSizeLimit: false, 31);
	}).WriteTo.Logger(delegate(LoggerConfiguration l)
	{
		l.Filter.ByIncludingOnly((LogEvent e) => e.Level == LogEventLevel.Warning).WriteTo.File("Logs\\Warning.log", LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, 1073741824L, null, buffered: false, shared: false, null, RollingInterval.Day, rollOnFileSizeLimit: false, 31);
	}).WriteTo.Logger(delegate(LoggerConfiguration l)
	{
		l.Filter.ByIncludingOnly((LogEvent e) => e.Level == LogEventLevel.Error).WriteTo.File("Logs\\Error.log", LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, 1073741824L, null, buffered: false, shared: false, null, RollingInterval.Day, rollOnFileSizeLimit: false, 31);
	}).WriteTo.Logger(delegate(LoggerConfiguration l)
	{
		l.Filter.ByIncludingOnly((LogEvent e) => e.Level == LogEventLevel.Fatal).WriteTo.File("Logs\\Fatal.log", LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, 1073741824L, null, buffered: false, shared: false, null, RollingInterval.Day, rollOnFileSizeLimit: false, 31);
	}).WriteTo.File("Logs\\Verbose.log", LogEventLevel.Verbose, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null, 1073741824L, null, buffered: false, shared: false, null, RollingInterval.Day, rollOnFileSizeLimit: false, 31).CreateLogger());
}
