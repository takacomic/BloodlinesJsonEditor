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

        internal void AddSprite(SpriteObject sprite)
        {
            sprites[CurrentRow, CurrentSprite] = sprite;
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
                    json.Sprites.Add(sprites[i, j]);
                    if (sprites[i, j] != null)
                        mainForm.spriteCombo.Items.Add(sprites[i, j].SpriteName);
                    else
                    {
                        MessageBox.Show($"Current Sprite Row {i + 1}, Current Editing Sprite {j + 1}. Does not exist, please add and try again");
                        return DeregisterSprites(json);
                    }
                }

            return json;
        }

        private EmptyJson DeregisterSprites(EmptyJson json)
        {
            foreach (var sprite in json.Sprites)
            {
                json.Sprites.Remove(sprite);
                mainForm.spriteCombo.Items.Remove(sprite.SpriteName);
            }
            return json;
        }
    }
}
