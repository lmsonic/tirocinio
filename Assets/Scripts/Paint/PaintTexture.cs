using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class PaintTexture : MonoBehaviour
    {


        Renderer rend;



        private void Start()
        {
            rend = GetComponent<Renderer>();
            ResetTextureToBlack();

            GenerateWhiteCircle(new Vector2(128, 128), 100);
        }

        void GenerateWhiteCircle(Vector2 center, float radius)
        {
            Texture2D texture = Instantiate(rend.material.GetTexture("_GrassMap")) as Texture2D;

            rend.material.SetTexture("_GrassMap", texture);

            // colors used to tint the first 3 mip levels

            int mipCount = Mathf.Min(3, texture.mipmapCount);

            // tint each mip level
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    Color color = Color.Lerp(Color.white, Color.black, distance / radius);
                    texture.SetPixel(x, y, color);
                }
            }
            // actually apply all SetPixels, don't recalculate mip levels
            texture.Apply(false);


        }

        void ResetTextureToBlack()
        {

            Texture2D texture = new Texture2D(256, 256);

            // duplicate the original texture and assign to the material
            rend.material.SetTexture("_GrassMap", texture);

            // colors used to tint the first 3 mip levels

            int mipCount = Mathf.Min(3, texture.mipmapCount);

            // tint each mip level
            for (int mip = 0; mip < mipCount; ++mip)
            {
                Color[] cols = texture.GetPixels(mip);
                for (int i = 0; i < cols.Length; ++i)
                {
                    cols[i] = Color.black;
                }
                texture.SetPixels(cols, mip);
            }
            // actually apply all SetPixels, don't recalculate mip levels
            texture.Apply(false);

        }
    }
}
