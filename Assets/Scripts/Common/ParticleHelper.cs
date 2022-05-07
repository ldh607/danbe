using UnityEngine;


namespace CellBig.Common
{
	public static class ParticleHelper
	{
		public static void SetupParticle(GameObject go)
		{
			var particles = go.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = 0; i < particles.Length; ++i)
			{
				var p = particles[i];
				if (p != null)
				{
					p.Stop(true);
					p.Clear(true);
				}
			}
		}
	}
}
