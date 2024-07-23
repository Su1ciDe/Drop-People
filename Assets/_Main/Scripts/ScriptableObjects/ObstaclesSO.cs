using AYellowpaper.SerializedCollections;
using GamePlay.Obstacles;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Obstacles", menuName = "DropPeople/Obstacles", order = 0)]
	public class ObstaclesSO : ScriptableObject
	{
		public SerializedDictionary<LevelEditorEnum, BaseObstacle> Obstacles = new SerializedDictionary<LevelEditorEnum, BaseObstacle>();
	}
}