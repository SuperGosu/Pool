using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioReset : MonoBehaviour
{
	private AudioSource m_Source;

	private void Awake()
	{
			m_Source = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		m_Source.time = 0;
		m_Source.Play();
	}
}
