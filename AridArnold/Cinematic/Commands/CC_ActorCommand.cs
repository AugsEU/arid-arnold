
namespace AridArnold
{
	abstract class CC_ActorCommand : CinematicCommand
	{
		protected CinematicActor mTargetActor;

		public CC_ActorCommand(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mTargetActor = parent.GetActorByName(cmdNode["actor"].InnerText);
		}
	}
}
