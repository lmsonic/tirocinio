using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class PaintGrassMask : MonoBehaviour
    {


        Renderer rend;




        private void Start()
        {
            rend = GetComponent<Renderer>();
            ResetTexture();

            //GenerateWhiteCircle(new Vector2(128, 128), 100);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("SeedBomb"))
            {

                Ray ray = new Ray(collision.contacts[0].point - collision.contacts[0].normal, collision.contacts[0].normal);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GenerateWhiteCircle(hit.textureCoord, 50f);
                    Debug.Log("Seed Bombed at " + hit.textureCoord.x + " " + hit.textureCoord.y);
                }
                collision.gameObject.SetActive(false);
            }
        }


        void GenerateWhiteCircle(Vector2 uvCoord, float radius)
        {
            Texture2D texture = Instantiate(rend.material.GetTexture("_GrassMap")) as Texture2D;

            Vector2 center = new Vector2(uvCoord.x * texture.width, uvCoord.y * texture.height);

            rend.material.SetTexture("_GrassMap", texture);

            // colors used to tint the first 3 mip levels

            int mipCount = Mathf.Min(3, texture.mipmapCount);

            // tint each mip level
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance < radius)
                    {
                        Color input = texture.GetPixel(x, y);
                        Color color = Color.Lerp(Color.white, input, distance / radius);
                        texture.SetPixel(x, y, color);
                    }
                }
            }
            // actually apply all SetPixels, don't recalculate mip levels
            texture.Apply(false);


        }

        void ResetTexture()
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

        private void OnDestroy()
        {
            Destroy(rend.material);
        }
    }
}
