using UnityEditor;

namespace Fiber.Build
{
	public class TexturePostprocessor : AssetPostprocessor
	{
		private void OnPreprocessTexture()
		{
			Compress();
		}

		private void Compress()
		{
			if (assetPath.Contains("_Uncompressed")) return;

			var importer = (TextureImporter)assetImporter;
			if (!importer) return;
			Crunch(importer);
		}

		private void Crunch(TextureImporter importer)
		{
			if (importer.crunchedCompression) return;
			importer.textureCompression = TextureImporterCompression.Compressed;
			importer.crunchedCompression = true;
			importer.compressionQuality = 50;
		}
	}
}