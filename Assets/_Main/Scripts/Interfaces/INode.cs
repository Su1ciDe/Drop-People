using GridSystem;

namespace Interfaces
{
	public interface INode
	{
		public bool CanMove { get; }
		public GridCell CurrentGridCell { get; set; }
		public void Place(GridCell placedCell);
	}
}