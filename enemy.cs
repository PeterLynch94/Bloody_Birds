using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Bloody_Birds
{
	public class enemy
	{
		private static SpriteUV 	sprite;
		private static TextureInfo	textureInfo;
		
		
		public enemy ()
		{
		}
		
		public void Dispose()
		{
			textureInfo.Dispose ();
		}
		
		public void Update(float deltaTime)
		{			
			
			
		}	
		
		public void Tapped()
		{
			Dispose ();
		}
		
		public SpriteUV getSprite()
		{
			return sprite;
		}
		
	}
}

