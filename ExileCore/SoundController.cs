using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace ExileCore;

public class SoundController : IDisposable
{
	private readonly List<SourceVoice> _list = new List<SourceVoice>();

	private readonly bool _initialized;

	private readonly MasteringVoice _masteringVoice;

	private readonly Dictionary<string, MyWave> _sounds = new Dictionary<string, MyWave>();

	private readonly string _soundsDir;

	private readonly XAudio2 _xAudio2;

	public SoundController(string dir)
	{
		_soundsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
		if (!Directory.Exists(_soundsDir))
		{
			_initialized = false;
			DebugWindow.LogError("Sounds dir not found, continue working without any sound.");
			return;
		}
		try
		{
			_xAudio2 = new XAudio2();
			_xAudio2.StartEngine();
			_masteringVoice = new MasteringVoice(_xAudio2);
			_initialized = true;
		}
		catch (Exception ex)
		{
			DebugWindow.LogError(ex.ToString());
		}
	}

	public void Dispose()
	{
		foreach (KeyValuePair<string, MyWave> sound in _sounds)
		{
			sound.Value.Buffer.Stream.Dispose();
		}
		_xAudio2.StopEngine();
		_masteringVoice?.Dispose();
		_xAudio2?.Dispose();
	}

	public void PlaySound(string name)
	{
		if (!_initialized)
		{
			return;
		}
		MyWave myWave = _sounds.GetValueOrDefault(name) ?? LoadSound(name);
		if (myWave == null)
		{
			DebugWindow.LogError("Sound file: " + name + ".wav not found.");
			return;
		}
		SourceVoice sourceVoice = new SourceVoice(_xAudio2, myWave.WaveFormat, enableCallbackEvents: true);
		sourceVoice.SubmitSourceBuffer(myWave.Buffer, myWave.DecodedPacketsInfo);
		sourceVoice.Start();
		_list.Add(sourceVoice);
		for (int i = 0; i < _list.Count; i++)
		{
			SourceVoice sourceVoice2 = _list[i];
			if (sourceVoice2.State.BuffersQueued <= 0)
			{
				sourceVoice2.Stop();
				sourceVoice2.DestroyVoice();
				sourceVoice2.Dispose();
				_list.RemoveAt(i);
			}
		}
	}

	public void PreloadSound(string name)
	{
		if (_initialized)
		{
			LoadSound(name);
		}
	}

	private MyWave LoadSound(string name)
	{
		if (name.IndexOf(".wav", StringComparison.Ordinal) == -1)
		{
			name = Path.Combine(_soundsDir, name + ".wav");
		}
		FileInfo fileInfo = new FileInfo(name);
		if (!fileInfo.Exists)
		{
			return null;
		}
		SoundStream soundStream = new SoundStream(File.OpenRead(name));
		WaveFormat format = soundStream.Format;
		AudioBuffer buffer = new AudioBuffer
		{
			Stream = soundStream.ToDataStream(),
			AudioBytes = (int)soundStream.Length,
			Flags = BufferFlags.EndOfStream
		};
		soundStream.Close();
		MyWave myWave = new MyWave
		{
			Buffer = buffer,
			WaveFormat = format,
			DecodedPacketsInfo = soundStream.DecodedPacketsInfo
		};
		_sounds[fileInfo.Name.Split('.').First()] = myWave;
		_sounds[fileInfo.Name] = myWave;
		return myWave;
	}

	public void SetVolume(float volume)
	{
		if (_initialized)
		{
			_masteringVoice.SetVolume(volume);
		}
	}
}
