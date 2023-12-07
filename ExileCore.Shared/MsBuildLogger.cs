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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
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
