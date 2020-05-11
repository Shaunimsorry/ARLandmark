#if  UNITY_ANDROID || UNITY_EDITOR 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SenseAR;

public class PointcloudVisualizer2 : MonoBehaviour {
	public int maxPointsToShow;
	public float particleSize = 5.0f;
	public Color particleColor;
	private const int k_MaxPointCount = 10000;
	private ParticleSystem currentPS;
	private ParticleSystem.Particle [] particles;
	private List<Vector4> m_PointsList = new List<Vector4>();
	// Use this for initialization
	void Start () {
		currentPS = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update () {
		//if (!SenseARSLAMController.IsSlamStart) {
		//	m_PointsList.Clear();
		//	int numParticles = Mathf.Min (Frame.PointCloud.PointCount, maxPointsToShow);
		//	ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
		//	currentPS.SetParticles (particles, numParticles);
		//	return;
		//}

		if (Frame.PointCloud.PointCount > 0 && Frame.PointCloud.IsUpdatedThisFrame) 
		{
			int numParticles = Mathf.Min (Frame.PointCloud.PointCount, maxPointsToShow);
			ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
			Frame.PointCloud.CopyPoints(m_PointsList);
			for (int i = 0; i < Frame.PointCloud.PointCount; i++)
			{
				particles [i].position = m_PointsList[i];
				particles [i].startColor = particleColor;
				particles [i].startSize = particleSize;
			}
			currentPS.SetParticles (particles, numParticles);
		} 
	}
}


 #endif