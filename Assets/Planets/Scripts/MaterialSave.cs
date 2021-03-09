using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;
using SFB;
public class MaterialSave : MonoBehaviour
{
    private int base_width = 100;
    private int base_height = 100;
    private List<Material> materials = new List<Material>();

    public void SaveImage(List<Material> mats, string filename)
    {
        var texture = Texture2D.whiteTexture;
        
        var renderTexture = RenderTexture.GetTemporary(base_width, base_height, 0);

   
        foreach (var mat in mats)
        {
            Graphics.Blit(texture, renderTexture, mat);    
        }
        
        RenderTexture.active = renderTexture;
        var tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        var bytes = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);
        RenderTexture.ReleaseTemporary(renderTexture);

        var path = StandaloneFileBrowser.SaveFilePanel("Save Png", "", filename, "png");
        File.WriteAllBytes(path, bytes);
        
    }
    public void SaveSheets(List<Material> mats, string filename, int w, int h,IPlanet planet, int customesize)
    {
        StartCoroutine(SaveSheetCoroutine(mats, filename, w, h, planet,customesize));
    }

    private IEnumerator SaveSheetCoroutine(List<Material> mats, string filename, int w, int h,IPlanet planet, int size)
    {
        var scale =  size / base_width;
        base_width = base_height = size;
        var texture = Texture2D.whiteTexture;

        var renderTexture = RenderTexture.GetTemporary(base_width , base_height , 0);

        var tex = new Texture2D(base_width * w,base_height * h , TextureFormat.RGBA32, false);

        int index = 1;
        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                texture = Texture2D.whiteTexture;
                foreach (var mat in mats) {
                    Graphics.Blit(texture, renderTexture, mat ); 
                }
                RenderTexture.active = renderTexture;
        
                tex.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), base_width * x , base_height * y);

                var atime = (float) index / (float) (w * h);
                var t = Mathf.Lerp(0f, 1f, atime );
                planet.SetCustomTime(t);
                
                index++;
            }
        }
        
        // apply tex and write png file
        tex.Apply();
        var bytes = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);
        RenderTexture.ReleaseTemporary(renderTexture);
        yield return new WaitForEndOfFrame();
        var path = StandaloneFileBrowser.SaveFilePanel("Save Sprite Sheets", "", filename, "png");
        File.WriteAllBytes(path, bytes);

    }
}
