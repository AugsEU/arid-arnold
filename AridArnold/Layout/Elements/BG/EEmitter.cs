
namespace AridArnold
{
	internal class EEmitter : LayElement
	{
		ParticleEmitter mEmitter;

		public EEmitter(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mEmitter = ParticleEmitter.FromXML(rootNode["emitter"]);
		}

		public override void Update(GameTime gameTime)
		{
			mEmitter.Update(gameTime);
			base.Update(gameTime);
		}
	}
}
