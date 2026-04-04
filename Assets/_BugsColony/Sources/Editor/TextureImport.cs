using UnityEditor;

public class TextureImport : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        var textureImporter = (TextureImporter)assetImporter;
        textureImporter.maxTextureSize = 2048;
        textureImporter.crunchedCompression = true;
        textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
        textureImporter.textureType = TextureImporterType.Sprite;
        
    }
}