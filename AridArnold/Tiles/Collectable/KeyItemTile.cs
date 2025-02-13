﻿namespace AridArnold
{
	class KeyItemTile : InteractableTile
	{
		KeyItemFlagType mFlagType;

		public KeyItemTile(Vector2 pos, KeyItemFlagType type) : base(pos)
		{
			mFlagType = type;

			//Hack to stop jeans from re-appearing.
			Level currLevel = CampaignManager.I.GetCurrentLevel();
			if(currLevel is HubLevel && FlagsManager.I.CheckFlag(FlagCategory.kKeyItems, (UInt32)mFlagType))
			{
				pEnabled = false;
			}
		}

		public override void LoadContent()
		{
			mTexture = FlagTypeHelpers.LoadKeyItemTexture(mFlagType);
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				// Collect the item.
				EventManager.I.TriggerEvent(EventType.KeyCollect);
				FlagsManager.I.SetFlag(FlagCategory.kKeyItems, (UInt32)mFlagType, true);
				FXManager.I.AddAnimator(mPosition, "Shared/Coin/Explode.max", DrawLayer.TileEffects);
				if (CampaignManager.I.GetCurrentLevelType() == AuxData.LevelType.Hub)
				{
					SFXManager.I.PlaySFX(AridArnoldSFX.CollectKey, 0.3f);
				}
				else
				{
					SFXManager.I.PlaySFX(AridArnoldSFX.Collect, 0.5f);
				}
				
				pEnabled = false;
			}
		}
	}
}
