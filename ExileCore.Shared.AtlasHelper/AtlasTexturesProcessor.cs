using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore.Shared.AtlasHelper;

public sealed class AtlasTexturesProcessor
{
	private readonly Dictionary<string, AtlasTexture> _atlasTextures = new Dictionary<string, AtlasTexture>();

	private static readonly AtlasTexture MISSING_TEXTURE;

	private readonly string _atlasPath;

	static AtlasTexturesProcessor()
	{
		MISSING_TEXTURE = new AtlasTexture("missing_texture.png", new RectangleF(0f, 0f, 1f, 1f), "missing_texture.png");
	}

	public AtlasTexturesProcessor(string atlasPath)
	{
		_atlasPath = atlasPath;
	}

	public AtlasTexturesProcessor(string configPath, string atlasPath)
	{
		_atlasPath = atlasPath;
		LoadConfig(configPath, atlasPath);
	}

	private void LoadConfig(string configPath, string atlasPath)
	{
		_atlasTextures.Clear();
		AtlasConfigData atlasConfigData = JsonConvert.DeserializeObject<AtlasConfigData>(File.ReadAllText(configPath));
		System.Numerics.Vector2 vector = new System.Numerics.Vector2(atlasConfigData.Meta.Size.W, atlasConfigData.Meta.Size.H);
		foreach (KeyValuePair<string, FrameValue> frame in atlasConfigData.Frames)
		{
			string text = frame.Key.Replace(".png", string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				DebugWindow.LogError("Sprite '" + Path.GetFileNameWithoutExtension(configPath) + "' contain a texture with empty/null name.", 20f);
			}
			else if (_atlasTextures.ContainsKey(text))
			{
				DebugWindow.LogError($"Sprite '{Path.GetFileNameWithoutExtension(configPath)}' already have a texture with name {text}. Duplicates is not allowed!", 20f);
			}
			else
			{
				float x = (float)frame.Value.Frame.X / vector.X;
				float y = (float)frame.Value.Frame.Y / vector.Y;
				float width = (float)frame.Value.Frame.W / vector.X;
				float height = (float)frame.Value.Frame.H / vector.Y;
				RectangleF textureUv = new RectangleF(x, y, width, height);
				AtlasTexture value = new AtlasTexture(text, textureUv, atlasPath);
				_atlasTextures.Add(text, value);
			}
		}
	}

	public AtlasTexture GetTextureByName(string textureName)
	{
		if (!_atlasTextures.TryGetValue(textureName.Replace(".png", string.Empty), out var value))
		{
			DebugWindow.LogError($"Texture with name'{textureName}' is not found in texture atlas {_atlasPath}.", 20f);
			return MISSING_TEXTURE;
		}
		return value;
	}
}
