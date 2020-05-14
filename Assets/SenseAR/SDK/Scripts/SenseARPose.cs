#if  UNITY_ANDROID || UNITY_EDITOR 
using System;
using System.Collections;

namespace UnityEngine
{
	public struct SenseARPose
	{

		public SenseARPose (Vector3 pos, Quaternion rot){
			position = pos;
			rotation = rot;
		}

		public Vector3 position;

		public Quaternion rotation;


		public static SenseARPose identity {
			get;
            private set;
		}

	}
}


 #endif