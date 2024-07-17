using DG.Tweening;
using GamePlay.People;
using Interfaces;
using UnityEngine;

namespace GridSystem
{
	[SelectionBase]
	public class GridCell : MonoBehaviour
	{
		public bool IsShowingHighlight => highlight.gameObject.activeSelf;

		public Vector2Int Coordinates { get; private set; }

		public PersonGroup CurrentPersonGroup { get; set; }
		public INode CurrentNode { get; set; }

		[SerializeField] private MeshRenderer highlight;

		private const float HIGHLIGHT_DURATION = .4F;

		private static readonly int baseColor = Shader.PropertyToID("_BaseColor");

		public void Setup(int x, int y, Vector2 nodeSize)
		{
			Coordinates = new Vector2Int(x, y);
			// transform.localScale = new Vector3(nodeSize.x, 1f, nodeSize.y);
		}

		public void ShowHighlight()
		{
			highlight.gameObject.SetActive(true);

			var tempColor = highlight.material.color;
			tempColor.a = 0;
			highlight.material.color = tempColor;

			highlight.material.DOKill();
			highlight.material.DOFade(1, baseColor, HIGHLIGHT_DURATION).SetEase(Ease.OutSine);
			highlight.material.DOFade(1, baseColor, HIGHLIGHT_DURATION).SetEase(Ease.OutSine);

			highlight.transform.localScale = Vector3.zero;
			highlight.transform.DOKill();
			highlight.transform.DOScale(1, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
		}

		public void HideHighlight()
		{
			highlight.material.DOKill();
			highlight.material.DOFade(0, baseColor, HIGHLIGHT_DURATION).SetEase(Ease.OutSine);

			highlight.transform.DOKill();
			highlight.transform.DOScale(0, HIGHLIGHT_DURATION).SetEase(Ease.InBack).OnComplete(() => highlight.gameObject.SetActive(false));
		}
	}
}