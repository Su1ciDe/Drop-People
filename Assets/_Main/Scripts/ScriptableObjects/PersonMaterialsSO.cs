using AYellowpaper.SerializedCollections;
using GamePlay.People;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Person Materials", menuName = "DropPeople/Person Materials", order = 11)]
	public class PersonMaterialsSO : ScriptableObject
	{
		public SerializedDictionary<PersonType, Material> PersonMaterials = new SerializedDictionary<PersonType, Material>();
		public SerializedDictionary<PersonType, Material> GoalHolderMaterials = new SerializedDictionary<PersonType, Material>();
	}
}