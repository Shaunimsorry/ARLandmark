#if  UNITY_ANDROID || UNITY_EDITOR 
using System;
using System.Collections;

namespace UnityEngine
{
	public struct Pose
	{

		public Pose (Vector3 pos, Quaternion rot){
			position = pos;
			rotation = rot;
		}

		public Vector3 position;

		public Quaternion rotation;


		public static Pose identity {
			get;
            private set;
		}

	}
}


 #endif