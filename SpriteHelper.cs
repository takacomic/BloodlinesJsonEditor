using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BloodlineJsonEditor
{
    internal class SpriteHelper
    {
        SpriteObject[,] sprites = new SpriteObject[16, 16];


        internal int CurrentRow = 0;
        internal int CurrentSprite = 0;

        internal SpriteHelper Instance
        {
            get
            {
                return this;
            }
        }

        internal MainForm mainForm { get; set; }

        internal SpriteObject GetSprite()
        {
            SpriteObject sprite = new SpriteObject();
            if (sprites[CurrentRow, CurrentSprite] != null)
                sprite = sprites[CurrentRow, CurrentSprite];

            return sprite;
        }
        internal SpriteObject GetSprite(int a, int b)
        {
            SpriteObject sprite = new SpriteObject();
            if (sprites[a, b] != null)
                sprite = sprites[a, b];

            return sprite;
        }
        internal void AddSprite(SpriteObject sprite)
        {
            sprites[CurrentRow, CurrentSprite] = sprite;
        }

        internal void AddSprite(SpriteObject sprite, int a, int b)
        {
            sprites[a, b] = sprite;
        }

        internal bool ChangedSprite(SpriteObject sprite)
        {
            if(sprites[CurrentRow, CurrentSprite] != null)
                if (sprites[CurrentRow, CurrentSprite].Equals(sprite))
                    return false;

            return true;
        }

        internal EmptyJson RegisterSprites(EmptyJson json)
        {
            for (int i = 0; i < (int)mainForm.CurrentSpriteRow.Maximum; i++)
                for (int j = 0; j < (int)mainForm.CurrentEditingSprite.Maximum; j++)
                {
                    if (sprites[i, j] != null)
                    {
                        if (mainForm.spriteCombo.Items.Contains(sprites[i, j].SpriteName))
                        {
                            MessageBox.Show($"Sprite at Row: {i+1}, Number: {j+1} has a duplicate name");
                        }
                        else
                        {
                            json.Sprites.Add(sprites[i, j]);
                            mainForm.spriteCombo.Items.Add(sprites[i, j].SpriteName);
                            if (sprites[i, j].SpriteName.Contains("walk01"))
                            {
                                mainForm.SkinSprite.Items.Add(sprites[i, j].SpriteName);
                                mainForm.idleSprite.Items.Add(sprites[i, j].SpriteName);
                            }
                            if (sprites[i, j].SpriteName.Contains("idle01"))
                                mainForm.idleSprite.Items.Add(sprites[i, j].SpriteName);
                            if (sprites[i, j].SpriteName.Contains("melee01"))
                                mainForm.meleeSprite.Items.Add(sprites[i, j].SpriteName);
                            if (sprites[i, j].SpriteName.Contains("meleex01"))
                                mainForm.melee2Sprite.Items.Add(sprites[i, j].SpriteName);
                            if (sprites[i, j].SpriteName.Contains("ranged01"))
                                mainForm.rangedSprite.Items.Add(sprites[i, j].SpriteName);
                            if (sprites[i, j].SpriteName.Contains("magic01"))
                                mainForm.magicSprite.Items.Add(sprites[i, j].SpriteName);
                            if (sprites[i, j].SpriteName.Contains("special01"))
                                mainForm.specialSprite.Items.Add(sprites[i, j].SpriteName);
                        }
                        if (!mainForm.TextureCombo.Items.Contains(sprites[i, j].TextureName))
                            mainForm.TextureCombo.Items.Add(sprites[i, j].TextureName);

                        if (mainForm.TextureToSprites.ContainsKey(sprites[i, j].TextureName))
                            mainForm.TextureToSprites[sprites[i, j].TextureName].Add(sprites[i, j]);
                        else
                            mainForm.TextureToSprites.Add(sprites[i, j].TextureName, new List<SpriteObject>() { sprites[i, j] }); 
                    }
                }

            return json;
        }
    }
}
