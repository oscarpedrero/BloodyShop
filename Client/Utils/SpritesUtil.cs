using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloodyShop.Client.Utils
{
    internal class SpritesUtil
    {
        public static  Sprite LoadPNGTOSprite(byte[] file)
        {

            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

            Sprite NewSprite = new Sprite();
            Texture2D SpriteTexture = LoadPNGToTexture(file);
            NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), 100.0f);

            return NewSprite;
        }

        private static Texture2D LoadPNGToTexture(byte[] file)
        {

            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            Texture2D Tex2D;
            byte[] FileData;

            //if (Properties.Resources.health)
            //{
            //FileData = Properties.Resources.progressbar;
            FileData = file;
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
            //}
            return null;                     // Return null if load failed
        }
    }
}
