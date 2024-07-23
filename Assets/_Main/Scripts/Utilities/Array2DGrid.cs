using Array2DEditor;
using UnityEngine;

namespace Utilities
{
	[System.Serializable]
	public class Array2DGrid : Array2D<LevelEditorEnum>
	{
		[SerializeField] private CellRowGrid[] cells = new CellRowGrid[Consts.defaultGridSize];

		protected override CellRow<LevelEditorEnum> GetCellRow(int idx)
		{
			return cells[idx];
		}
	}

	[System.Serializable]
	public class CellRowGrid : CellRow<LevelEditorEnum>
	{
	}
}