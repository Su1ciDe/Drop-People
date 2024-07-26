using DG.Tweening;
using GamePlay.People;
using Interfaces;
using UnityEngine;

namespace GridSystem
{
	[SelectionBase]
	public class GridCell : MonoBehaviour
	{
		public bool IsShowingHighlight => highlightT.gameObject.activeSelf;

		public Vector2Int Coordinates { get; private set; }

		public PersonGroup CurrentPersonGroup { get; set; }
		public INode CurrentNode { get; set; }

		public GameObject Model => model;
		[SerializeField] private GameObject model;
 		
		[SerializeField] private Transform highlightT;
		[SerializeField] private MeshRenderer highlightMR;
		
		[SerializeField] private Transform highlightRedT;
		[SerializeField] private MeshRenderer highlightRedMR;

		private const float HIGHLIGHT_DURATION = .4F;

		private static readonly int baseColor = Shader.PropertyToID("_BaseColor");

		public void Setup(int x, int y, Vector2 nodeSize)
		{
			Coordinates = new Vector2Int(x, y);
			// transform.localScale = new Vector3(nodeSize.x, 1f, nodeSize.y);
		}

		public void ShowHighlight()
		{
			highlightT.gameObject.SetActive(true);

			var tempColor = highlightMR.material.color;
			tempColor.a = 0;
			highlightMR.material.color = tempColor;

			highlightMR.material.DOKill();
			highlightMR.material.DOFade(1, baseColor, HIGHLIGHT_DURATION).SetEase(Ease.OutSine);

			highlightT.DOKill();
			highlightT.localScale = Vector3.zero;
			highlightT.DOScale(1, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
		}

		public void HideHighlight()
		{
			highlightMR.material.DOKill();
			highlightMR.material.DOFade(0, baseColor, HIGHLIGHT_DURATION).SetEase(Ease.OutSine);

			highlightT.DOComplete();
			highlightT.DOScale(0, HIGHLIGHT_DURATION).SetEase(Ease.InBack).OnComplete(() => highlightT.gameObject.SetActive(false));
		}

		public void ShowRedHighlight()
		{
			highlightRedT.gameObject.SetActive(true);

			var tempColor = highlightRedMR.material.color;
			tempColor.a = 0;
			highlightRedMR.material.color = tempColor;

			highlightRedMR.material.DOKill();
			highlightRedMR.material.DOFade(1, baseColor, HIGHLIGHT_DURATION).SetEase(Ease.OutSine);

			highlightRedT.DOKill();
			highlightRedT.localScale = Vector3.zero;
			highlightRedT.DOScale(1, HIGHLIGHT_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
			{
				highlightRedMR.material.DOFade(0, baseColor, HIGHLIGHT_DURATION).SetEase(Ease.OutSine);

				highlightRedT.DOScale(0, HIGHLIGHT_DURATION).SetEase(Ease.InBack).OnComplete(() => highlightRedT.gameObject.SetActive(false));
			});
		}
	}
}