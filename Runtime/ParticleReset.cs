using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gosu.Pool{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleReset : MonoBehaviour
	{
		private ParticleSystem[] particleSystems;

		void Awake()
		{
			particleSystems = GetComponentsInChildren<ParticleSystem>();
		}

		private void OnEnable()
		{
			foreach (ParticleSystem p in particleSystems)
			{
				ResetParticle(p);
			}
		}

		private void ResetParticle(ParticleSystem particleSystem)
		{
			particleSystem.Clear();
			particleSystem.Play();
		}	

	}
}
