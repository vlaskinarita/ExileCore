using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ExileCore.Shared;

public class MsBuildLogger : Logger
{
	public IList<BuildTarget> Targets { get; private set; }

	public IList<BuildError> Errors { get; private set; }

	public IList<BuildWarning> Warnings { get; private set; }

	public IList<string> BuildDetails { get; private set; }

	public override void Initialize(IEventSource eventSource)
	{
	
		BuildDetails = new List<string>();
		Targets = new List<BuildTarget>();
		Errors = new List<BuildError>();
		Warnings = new List<BuildWarning>();
		eventSource.ProjectStarted += new ProjectStartedEventHandler(EventSource_ProjectStarted);
		eventSource.TargetFinished += new TargetFinishedEventHandler(EventSource_TargetFinished);
		eventSource.ErrorRaised += new BuildErrorEventHandler(EventSource_ErrorRaised);
		eventSource.WarningRaised += new BuildWarningEventHandler(EventSource_WarningRaised);
		eventSource.ProjectFinished += new ProjectFinishedEventHandler(EventSource_ProjectFinished);
	}

	private void EventSource_ProjectStarted(object sender, ProjectStartedEventArgs e)
	{
		BuildDetails.Add(((BuildEventArgs)e).Message);
	}

	private void EventSource_TargetFinished(object sender, TargetFinishedEventArgs e)
	{
		BuildTarget item = new BuildTarget
		{
			Name = e.TargetName,
			File = e.TargetFile,
			Succeeded = e.Succeeded,
			Outputs = e.TargetOutputs
		};
		Targets.Add(item);
	}

	private void EventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
	{
		BuildError item = new BuildError
		{
			File = e.File,
			Timestamp = ((BuildEventArgs)e).Timestamp,
			LineNumber = e.LineNumber,
			ColumnNumber = e.ColumnNumber,
			Code = e.Code,
			Message = ((BuildEventArgs)e).Message
		};
		Errors.Add(item);
	}

	private void EventSource_WarningRaised(object sender, BuildWarningEventArgs e)
	{
		BuildWarning item = new BuildWarning
		{
			File = e.File,
			Timestamp = ((BuildEventArgs)e).Timestamp,
			LineNumber = e.LineNumber,
			ColumnNumber = e.ColumnNumber,
			Code = e.Code,
			Message = ((BuildEventArgs)e).Message
		};
		Warnings.Add(item);
	}

	private void EventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
	{
		BuildDetails.Add(((BuildEventArgs)e).Message);
	}
}
