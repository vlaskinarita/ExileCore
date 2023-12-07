using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class DeployedObject : RemoteMemoryObject
{
	private readonly FrameCache<ActorDeployedObject> cacheValue;

	private Entity _entity;

	private ActorDeployedObject Struct => cacheValue.Value;

	public ushort ObjectId => Struct.ObjectId;

	public ushort SkillKey => Struct.SkillId;

	public Entity Entity => _entity ?? (_entity = EntityListWrapper.GetEntityById(ObjectId));

	public DeployedObject()
	{
		cacheValue = new FrameCache<ActorDeployedObject>(() => base.M.Read<ActorDeployedObject>(base.Address));
	}
}
