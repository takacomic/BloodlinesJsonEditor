using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BloodlineJsonEditor
{
    public partial class MainForm : Form
    {
        
        JsonFile jsonFile = new JsonFile();
        EmptyJson json = new EmptyJson();
        Image imageStore;
        SpriteHelper spriteHelper = new SpriteHelper();
        SpriteObject spriteObject = new SpriteObject();
        Dictionary<string, StatModifier> AltStats = new Dictionary<string, StatModifier>();
        Dictionary<string, CharacterObject> Characters = new Dictionary<string, CharacterObject>();
        Dictionary<string, SkinObject> Skins = new Dictionary<string, SkinObject>();
        Dictionary<string, byte[]> spriteSheets = new Dictionary<string, byte[]>();

        public MainForm()
        {
            spriteHelper = spriteHelper.Instance;
            spriteHelper.mainForm = this;
            InitializeComponent();
            SpriteSheetBox.Paint += pictureBox1_Paint;
            tabControlChar.TabPages.Remove(tabPageCharOn);
            tabControlSkins.TabPages.Remove(SkinOnEveryLevelUp);
            jsonFile.wepSet();

            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
            JsonView.Text = parsedJson;
        }

        ///
        /// Start Sprite Section
        ///
        internal void LoadSpriteSheet_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image|*.png;*.jpg;*.bmp;*.gif";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SpriteSheetBox.Load(openFileDialog1.FileName);
                spriteBox.Load(openFileDialog1.FileName);
                spriteBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                imageStore = SpriteSheetBox.Image;
                ImageWidth.Text = SpriteSheetBox.Image.Width.ToString();
                ImageHeight.Text = SpriteSheetBox.Image.Height.ToString();
                string[] filename = SpriteSheetBox.ImageLocation.Split('.');
                filename[0] = filename[0].Substring(filename[0].LastIndexOf("\\"));
                filename[0] = filename[0].Remove(0, 1);
                TextureName.Text = filename[0];
            }
        }
        internal void pictureBox1_Paint(object sender, EventArgs e)
        {
            if (SpriteSheetBox.Image != null)
            {
                GenerateSpriteInfo.Enabled = true;
                GenerateSpriteInfo.Visible = true;
            }
            else
            {
                GenerateSpriteInfo.Visible = false;
                GenerateSpriteInfo.Enabled = false;
            }
        }
        internal void SpriteRowNum_ValueChanged(object sender, EventArgs e)
        {
            if (SpriteRowNum.Value < CurrentSpriteRow.Value)
            {
                MessageBox.Show("Can't set Number of Sprite Rows under the Current Editing Sprite Row");
                SpriteRowNum.Value = CurrentSpriteRow.Maximum;
            }
            else
                CurrentSpriteRow.Maximum = SpriteRowNum.Value;


        }
        internal void SpritePerRow_ValueChanged(object sender, EventArgs e)
        {
            if (SpritePerRow.Value < EmptySpriteRows.Value)
            {
                MessageBox.Show("Can't set Number of Sprites per Row under the Empty Sprite Slots in Row");
                SpritePerRow.Value = EmptySpriteRows.Maximum;
            }
            else if (SpritePerRow.Value < CurrentEditingSprite.Value)
            {
                MessageBox.Show("Can't set Number of Sprites per Row under the Current Editing Sprite in Row");
                SpritePerRow.Value = EmptySpriteRows.Maximum;
            }
            else
            {
                CurrentEditingSprite.Maximum = SpritePerRow.Value;
                EmptySpriteRows.Maximum = SpritePerRow.Value - 1;
            }
        }
        internal void EmptySpriteRows_ValueChanged(object sender, EventArgs e)
        {
            if (EmptySpriteRows.Value + CurrentEditingSprite.Value > SpritePerRow.Value)
            {
                MessageBox.Show("Can't set Empty Sprites per Row above the Current Editing Sprite in Row");
                EmptySpriteRows.Value -= 1;
            }
            else
            {
                CurrentEditingSprite.Maximum = SpritePerRow.Value - EmptySpriteRows.Value;
            }
        }
        internal void CurrentSpriteRow_ValueChanged(object sender, EventArgs e)
        {
            CurrentEditingSprite.ValueChanged -= CurrrentEditingSprite_ValueChanged;
            CurrentEditingSprite.Value = 1;
            spriteHelper.CurrentSprite = 0;
            CurrentEditingSprite.ValueChanged += CurrrentEditingSprite_ValueChanged;

            spriteHelper.CurrentRow = (int)CurrentSpriteRow.Value - 1;
            spriteObject = spriteHelper.GetSprite();
            SpriteObjectSet();
        }
        internal void CurrrentEditingSprite_ValueChanged(object sender, EventArgs e)
        {
            SpriteObjectValueSet();

            spriteHelper.CurrentSprite = (int)CurrentEditingSprite.Value - 1;
            spriteObject = spriteHelper.GetSprite();

            SpriteObjectSet();
        }
        private void GenerateSpriteInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (GenerateSpriteInfo.Checked)
            {
                int baseWidth = imageStore.Width / (int)SpritePerRow.Value;
                int baseHeight = imageStore.Height / (int)SpriteRowNum.Value;
                string filename = SpriteSheetBox.ImageLocation.Split('.')[0];
                filename = filename.Substring(filename.LastIndexOf("\\")).Remove(0, 1);

                SpriteName.Text = "";
                TextureName.Text = filename;
                numRectX.Value = baseWidth * ((int)CurrentEditingSprite.Value - 1);
                numRectY.Value = baseHeight * ((int)CurrentSpriteRow.Value - 1);
                numRectWidth.Value = baseWidth;
                numRectHeight.Value = baseHeight;
            }
        }
        private void numRectX_ValueChanged(object sender, EventArgs e)
        {
            spriteBox.Left = -(int)numRectX.Value * (int)SpriteScaleX.Value;
        }
        private void numRectY_ValueChanged(object sender, EventArgs e)
        {
            spriteBox.Top = -(int)numRectY.Value * (int)SpriteScaleX.Value;
        }
        private void numRectWidth_ValueChanged(object sender, EventArgs e)
        {
            panel1.Width = (int)numRectWidth.Value * (int)SpriteScaleX.Value;
        }
        private void numRectHeight_ValueChanged(object sender, EventArgs e)
        {
            panel1.Height = (int)numRectHeight.Value * (int)SpriteScaleX.Value;
        }
        private void SpriteScaleX_ValueChanged(object sender, EventArgs e)
        {
            panel1.Height = (int)numRectHeight.Value * (int)SpriteScaleX.Value;
            panel1.Width = (int)numRectWidth.Value * (int)SpriteScaleX.Value;
            spriteBox.Image = ResizeImage(imageStore, imageStore.Width * (int)SpriteScaleX.Value, imageStore.Height * (int)SpriteScaleX.Value);
            spriteBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            numRectX.Value += 1;
            numRectY.Value += 1;
            numRectX.Value -= 1;
            numRectY.Value -= 1;
        }
        private void SpriteAnimation_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpriteNameAppend.Text = $"{SpriteName.Text}_{SpriteAnimation.Text.ToLower()}0{(int)SpritePlacement.Value}";
        }
        private void SpritePlacement_ValueChanged(object sender, EventArgs e)
        {
            SpriteNameAppend.Text = $"{SpriteName.Text}_{SpriteAnimation.Text.ToLower()}0{(int)SpritePlacement.Value}";
        }
        private void SpriteObjectSet()
        {
            if (spriteObject.IsEmpty())
            {
                SpriteName.Text = spriteObject.SpriteName.Split('_')[0];
                TextureName.Text = spriteObject.TextureName.Split('.')[0];
                SpriteRect rect = spriteObject.Rect;
                numRectX.Value = rect.X;
                numRectY.Value = rect.Y;
                numRectWidth.Value = rect.Width;
                numRectHeight.Value = rect.Height;
                SpriteAnimation.Text = spriteObject.AnimationType;
                SpritePlacement.Value = spriteObject.SpritePlacement;
            }
            else if (GenerateSpriteInfo.Checked)
            {
                int baseWidth = imageStore.Width / (int)SpritePerRow.Value;
                int baseHeight = imageStore.Height / (int)SpriteRowNum.Value;
                string filename = SpriteSheetBox.ImageLocation.Split('.')[0];
                filename = filename.Substring(filename.LastIndexOf("\\")).Remove(0, 1);

                SpriteName.Text = "";
                TextureName.Text = filename;
                numRectX.Value = baseWidth * ((int)CurrentEditingSprite.Value - 1);
                numRectY.Value = baseHeight * ((int)CurrentSpriteRow.Value - 1);
                numRectWidth.Value = baseWidth;
                numRectHeight.Value = baseHeight;
            }
            else
            {
                string filename = SpriteSheetBox.ImageLocation.Split('.')[0];
                filename = filename.Substring(filename.LastIndexOf("\\")).Remove(0, 1);

                SpriteName.Text = "";
                TextureName.Text = filename;
                numRectX.Value = 0;
                numRectY.Value = 0;
                numRectWidth.Value = 0;
                numRectHeight.Value = 0;
            }
        }
        private void SpriteObjectValueSet()
        {
            spriteObject.SpriteName = SpriteName.Text + SpriteNameAppend.Text + ".png";
            spriteObject.TextureName = TextureName.Text + ".png";
            SpriteRect rect = new SpriteRect();
            rect.X = (int)numRectX.Value;
            rect.Y = (int)numRectY.Value;
            rect.Width = (int)numRectWidth.Value;
            rect.Height = (int)numRectHeight.Value;
            spriteObject.Rect = rect;
            spriteObject.AnimationType = SpriteAnimation.Text;
            spriteObject.SpritePlacement = (int)SpritePlacement.Value;

            if (spriteHelper.ChangedSprite(spriteObject))
                spriteHelper.AddSprite(spriteObject);
        }
        private void SpriteName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('.'))
            {
                e.Handled = true;
                MessageBox.Show("You do not need to add .png");
            }
            else
                e.Handled = false;
        }
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapmode = new ImageAttributes())
                {
                    wrapmode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipX);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapmode);
                }
            }

            return destImage;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            SpriteObjectValueSet();
            spriteHelper.AddSprite(spriteObject);
            if (SpriteSheetBox.Image != null)
            {
                string filename = SpriteSheetBox.ImageLocation.Split('.')[0];
                filename = filename.Substring(filename.LastIndexOf("\\")).Remove(0, 1);
                ImageConverter imageConverter = new ImageConverter();
                byte[] bytes = (byte[])imageConverter.ConvertTo(SpriteSheetBox.Image, typeof(byte[]));
                if (!spriteSheets.ContainsKey(filename + ".png")) spriteSheets.Add(filename + ".png", bytes);
            }
            string parsedJson = JsonConvert.SerializeObject(spriteHelper.RegisterSprites(json), Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
            JsonView.Text = parsedJson;
        }
        ///
        /// End Sprite Section
        ///

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CharExWeapons.Text.Length > 0)
            {
                CharExWeapons.Text = CharExWeapons.Text + "," + CharExWeaponsCombo.Text;
            }
            else
            {
                CharExWeapons.Text = CharExWeaponsCombo.Text;
            }

            CharExWeaponsCombo.ResetText();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(CharOnEveryLevelUp.Checked == true)
                tabControlChar.TabPages.Insert(1, tabPageCharOn);
            else
                tabControlChar.TabPages.Remove(tabPageCharOn);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StatModifier altStats = new StatModifier();
            altStats.Amount = (float)AltAmount.Value;
            altStats.Area = (float)AltArea.Value;
            altStats.Armor = (float)AltArmor.Value;
            altStats.Banish = (float)AltBanish.Value;
            altStats.Charm = (int)AltCharm.Value;
            altStats.Cooldown = (float)AltCooldown.Value;
            altStats.Curse = (float)AltCurse.Value;
            altStats.Defang = (float)AltDefang.Value;
            altStats.Duration = (float)AltDuration.Value;
            altStats.Fever = (float)AltFever.Value;
            altStats.Greed = (float)AltGreed.Value;
            altStats.Growth = (float)AltGrowth.Value;
            altStats.Invul = (float)AltInvul.Value;
            altStats.Level = (int)AltLevel.Value;
            altStats.Luck = (float)AltLuck.Value;
            altStats.Magnet = (float)AltMagnet.Value;
            altStats.MaxHp = (int)AltHp.Value;
            altStats.MoveSpeed = (float)AltMove.Value;
            altStats.Power = (float)AltPower.Value;
            altStats.Regen = (float)AltRegen.Value;
            altStats.Rerolls = (float)AltReroll.Value;
            altStats.Revivals = (float)AltRevivals.Value;
            altStats.Shields = (float)AltShields.Value;
            /*altStats.SineArea = AltSineArea.Value.ToString();
            altStats.SineCoolDown = AltSineCooldown.Value.ToString();
            altStats.SineDuration = AltSineDuration.Value.ToString();
            altStats.SineMight = AltSineMight.Value.ToString();
            altStats.SineSpeed = AltSineSpeed.Value.ToString();*/
            altStats.Shroud = (float)AltShroud.Value;
            altStats.Skips = (float)AltSkips.Value;
            altStats.Speed = (float)AltSpeed.Value;
            if (AltStats.ContainsKey($"{AltLevelText.Text} {(int)AltLevel.Value}"))
            {
                MessageBox.Show($"{AltLevelText.Text} {(int)AltLevel.Value} already has stats assigned. Delete Them First");
            }
            else
            {
                AltStats.Add($"{AltLevelText.Text} {(int)AltLevel.Value}", altStats.Cleaner());
                AltStatBox.Items.Add($"{AltLevelText.Text} {(int)AltLevel.Value}");
            }
        }

        private void RemoveAltStatButton_Click(object sender, EventArgs e)
        {
            if(AltStatBox.Text != "Select Alt Stats")
            {
                AltStats.Remove(AltStatBox.Text);
                AltStatBox.Items.Remove(AltStatBox.Text);
                AltStatBox.Text = "Select Alt Stats";
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            StatModifier skinEveryLevel = new StatModifier();
            if (SkinEveryLevelUp.Checked)
            {
                skinEveryLevel.Amount = (float)SkinOnAmount.Value;
                skinEveryLevel.Area = (float)SkinOnArea.Value;
                skinEveryLevel.Armor = (float)SkinOnArmor.Value;
                skinEveryLevel.Banish = (float)SkinOnBanish.Value;
                skinEveryLevel.Charm = (int)SkinOnCharm.Value;
                skinEveryLevel.Cooldown = (float)SkinOnCooldown.Value;
                skinEveryLevel.Curse = (int)SkinOnCurse.Value;
                skinEveryLevel.Defang = (float)SkinOnDefang.Value;
                skinEveryLevel.Duration = (float)SkinOnDuration.Value;
                skinEveryLevel.Fever = (float)SkinOnFever.Value;
                skinEveryLevel.Greed = (float)SkinOnGreed.Value;
                skinEveryLevel.Growth = (float)SkinOnGrowth.Value;
                skinEveryLevel.Invul = (float)SkinOnInvul.Value;
                skinEveryLevel.Luck = (float)SkinOnLuck.Value;
                skinEveryLevel.Magnet = (float)SkinOnMagnet.Value;
                skinEveryLevel.MaxHp = (int)SkinOnHp.Value;
                skinEveryLevel.MoveSpeed = (float)SkinOnMove.Value;
                skinEveryLevel.Power = (float)SkinOnPower.Value;
                skinEveryLevel.Regen = (float)SkinOnRegen.Value;
                skinEveryLevel.Rerolls = (float)SkinOnRerolls.Value;
                skinEveryLevel.Revivals = (float)SkinOnRevivals.Value;
                skinEveryLevel.Shields = (float)SkinOnShields.Value;
                /*skinEveryLevel.SineArea = SkinOnSineArea.Value.ToString();
                skinEveryLevel.SineCoolDown = SkinOnSineCooldown.Value.ToString();
                skinEveryLevel.SineDuration = SkinOnSineDuration.Value.ToString();
                skinEveryLevel.SineMight = SkinOnSineMight.Value.ToString();
                skinEveryLevel.SineSpeed = SkinOnSineSpeed.Value.ToString();*/
                skinEveryLevel.Shroud = (float)SkinOnShroud.Value;
                skinEveryLevel.Skips = (float)SkinOnSkips.Value;
                skinEveryLevel.Speed = (float)SkinOnSpeed.Value;
            }

            SkinObject skin = new SkinObject();
            List<string> exacc = new List<string>();
            List<string> exweap = new List<string>();
            List<string> hiddenweap = new List<string>();
            if (SkinExAccessories.Text.Length > 0)
                foreach (string s in SkinExAccessories.Text.ToString().Split(',') )
                    exacc.Add(jsonFile.wepNameToKey[s]);
            if (SkinExWeapons.Text.Length > 0)
                foreach (string s in SkinExWeapons.Text.ToString().Split(','))
                    exweap.Add(jsonFile.wepNameToKey[s]);
            if (SkinHiddenWeapons.Text.Length > 0)
                foreach (string s in SkinHiddenWeapons.Text.ToString().Split(','))
                    hiddenweap.Add(jsonFile.wepNameToKey[s]);

            skin.AlwaysAnimated = SkinAnimated.Checked;
            skin.AlwaysHidden = SkinAlwaysHidden.Checked;
            skin.Amount = (float)SkinAmount.Value;
            skin.Area = (float)SkinArea.Value;
            skin.Banish = (float)SkinBanish.Value;
            skin.Charm = (float)SkinCharm.Value;
            skin.Cooldown = (float)SkinCooldown.Value;
            skin.Curse = (float)SkinCurse.Value;
            skin.Description = SkinDescription.Text;
            skin.Duration = (float)SkinDuration.Value;
            skin.ExAccessories = exacc;
            skin.ExWeapons = exweap;
            skin.Greed = (float)SkinGreed.Value;
            skin.Growth = (float)SkinGrowth.Value;
            skin.Hidden = SkinHidden.Checked;
            skin.HiddenWeapons = hiddenweap;
            skin.Luck = (float)SkinLuck.Value;
            skin.Magnet = (float)SkinMagnet.Value;
            skin.MaxHp = (int)SkinHp.Value;
            skin.MoveSpeed = (float)SkinMove.Value;
            skin.Name = SkinName.Text;
            if (SkinEveryLevelUp.Checked) skin.OnEveryLevelUp = skinEveryLevel.Cleaner();
            skin.Power = (float)SkinPower.Value;
            skin.Prefix = SkinPrefix.Text;
            skin.Price = (float)SkinPrice.Value;
            skin.Regen = (float)SkinRegen.Value;
            skin.Rerolls = (float)SkinRerolls.Value;
            skin.Revivals = (float)SkinRevivals.Value;
            skin.Secret = SkinSecret.Checked;
            skin.Shields = (float)SkinShields.Value;
            skin.SkinType = SkinType.Text;
            skin.Skips = (float)SkinSkips.Value;
            skin.Speed = (float)SkinSpeed.Value;
            skin.SpriteName = SkinSprite.Text;
            skin.Suffix = SkinSuffix.Text;
            skin.TextureName = SkinTexture.Text;
            skin.Unlocked = SkinUnlocked.Checked;

            if (Skins.ContainsKey($"{SkinType.Text}"))
            {
                MessageBox.Show($"{SkinType.Text} already has stats assigned. Delete Them First");
            }
            else
            {
                Skins.Add($"{SkinType.Text}", skin.Cleaner());
                SkinCombo.Items.Add($"{SkinType.Text}");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (SkinCombo.Text != "Select Skin")
            {
                Skins.Remove(SkinCombo.Text);
                SkinCombo.Items.Remove(SkinCombo.Text);
                SkinCombo.Text = "Select Skin";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            StatModifier statModifier = new StatModifier();
            statModifier.Amount = (float)statsAmount.Value;
            statModifier.Area = (float)statsArea.Value;
            statModifier.Armor = (float)statsArmor.Value;
            statModifier.Banish = (float)statsBanish.Value;
            statModifier.Charm = (int)statsCharm.Value;
            statModifier.Cooldown = (float)statsCooldown.Value;
            statModifier.Curse = (float)statsCurse.Value;
            statModifier.Defang = (float)statsDefang.Value;
            statModifier.Duration = (float)statsDuration.Value;
            statModifier.Fever = (float)statsFever.Value;
            statModifier.Greed = (float)statsGreed.Value;
            statModifier.Growth = (float)statsGrowth.Value;
            statModifier.Invul = (float)statsInvul.Value;
            statModifier.Level = (int)statsLevel.Value;
            statModifier.Luck = (float)statsLuck.Value;
            statModifier.Magnet = (float)statsMagnet.Value;
            statModifier.MaxHp = (int)statsHp.Value;
            statModifier.MoveSpeed = (float)statsMove.Value;
            statModifier.Power = (float)statsPower.Value;
            statModifier.Regen = (float)statsRegen.Value;
            statModifier.Rerolls = (float)statsReroll.Value;
            statModifier.Revivals = (float)statsRevivals.Value;
            statModifier.Shields = (float)statsShields.Value;
            /*statModifier.SineArea = statsSineArea.Value.ToString();
            statModifier.SineCoolDown = statsSineCooldown.Value.ToString();
            statModifier.SineDuration = statsSineDuration.Value.ToString();
            statModifier.SineMight = statsSineMight.Value.ToString();
            statModifier.SineSpeed = statsSineSpeed.Value.ToString();*/
            statModifier.Shroud = (float)statsShroud.Value;
            statModifier.Skips = (float)statsSkips.Value;
            statModifier.Speed = (float)statsSpeed.Value;

            StatModifier charEveryLevel = new StatModifier();
            if (CharOnEveryLevelUp.Checked)
            {
                charEveryLevel.Amount = (float)OnAmount.Value;
                charEveryLevel.Area = (float)OnArea.Value;
                charEveryLevel.Armor = (float)OnArmor.Value;
                charEveryLevel.Banish = (float)OnBanish.Value;
                charEveryLevel.Charm = (int)OnCharm.Value;
                charEveryLevel.Cooldown = (float)OnCooldown.Value;
                charEveryLevel.Curse = (float)OnCurse.Value;
                charEveryLevel.Defang = (float)OnDefang.Value;
                charEveryLevel.Duration = (float)OnDuration.Value;
                charEveryLevel.Fever = (float)OnFever.Value;
                charEveryLevel.Greed = (float)OnGreed.Value;
                charEveryLevel.Growth = (float)OnGrowth.Value;
                charEveryLevel.Invul = (float)OnInvul.Value;
                charEveryLevel.Luck = (float)OnLuck.Value;
                charEveryLevel.Magnet = (float)OnMagnet.Value;
                charEveryLevel.MaxHp = (int)OnHp.Value;
                charEveryLevel.MoveSpeed = (float)OnMove.Value;
                charEveryLevel.Power = (float)OnPower.Value;
                charEveryLevel.Regen = (float)OnRegen.Value;
                charEveryLevel.Rerolls = (float)OnRerolls.Value;
                charEveryLevel.Revivals = (float)OnRevivals.Value;
                charEveryLevel.Shields = (float)OnShields.Value;
                /*charEveryLevel.SineArea = OnSineArea.Value.ToString();
                charEveryLevel.SineCoolDown = OnSineCooldown.Value.ToString();
                charEveryLevel.SineDuration = OnSineDuration.Value.ToString();
                charEveryLevel.SineMight = OnSineMight.Value.ToString();
                charEveryLevel.SineSpeed = OnSineSpeed.Value.ToString();*/
                charEveryLevel.Shroud = (float)OnShroud.Value;
                charEveryLevel.Skips = (float)OnSkips.Value;
                charEveryLevel.Speed = (float)OnSpeed.Value;
            }

            CharacterObject character = new CharacterObject();
            List<string> exweap = new List<string>();
            List<string> hiddenweap = new List<string>();
            List<string> showcase = new List<string>();
            if (CharShowcase.Text.Length > 0)
                foreach (string s in CharShowcase.Text.ToString().Split(','))
                    showcase.Add(jsonFile.wepNameToKey[s]);
            if (CharExWeapons.Text.Length > 0)
                foreach (string s in CharExWeapons.Text.ToString().Split(','))
                    exweap.Add(jsonFile.wepNameToKey[s]);
            if (CharHiddenWeapons.Text.Length > 0)
                foreach (string s in CharHiddenWeapons.Text.ToString().Split(','))
                    hiddenweap.Add(jsonFile.wepNameToKey[s]);

            character.AlwaysHidden = CharAlwaysHidden.Checked;
            character.CharName = CharName.Text;
            character.CurrentSkin = CharCurrentSkin.Text;
            character.Description = CharDescription.Text;
            character.ExLevels = (int)CharExLevels.Value;
            character.ExWeapons = exweap;
            character.Hidden = CharHidden.Checked;
            character.HiddenWeapons = hiddenweap;
            character.IsBought = CharIsBought.Checked;
            character.Level = (int)CharLevel.Value;
            if (CharOnEveryLevelUp.Checked) character.OnEveryLevelUp = charEveryLevel.Cleaner();
            character.PortraitName = CharPortrait.Text;
            character.Prefix = CharPrefix.Text;
            character.Price = (int)CharPrice.Value;
            character.Showcase = showcase;
            if (Skins.Count > 0)
                foreach(SkinObject skin in Skins.Values)
                    character.Skins.Add(skin);
            character.StartingWeapon = jsonFile.wepNameToKey[CharStartingWeapon.Text];
            character.Suffix = CharSuffix.Text;
            character.Surname = CharSurname.Text;
            character.StatModifiers.Add(statModifier.Cleaner());
            if (AltStats.Count > 0)
                foreach (StatModifier stat in AltStats.Values)
                    character.StatModifiers.Add(stat);

            if (Characters.ContainsKey($"{CharName.Text}"))
            {
                MessageBox.Show($"{SkinType.Text} already has stats assigned. Delete Them First");
            }
            else
            {
                Characters.Add($"{SkinType.Text}", character.Cleaner());
                CharacterCombo.Items.Add($"{CharName.Text}");
                json.Characters.Add(character.Cleaner());
            }
            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                });
            JsonView.Text = parsedJson;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (CharacterCombo.Text != "Select Character")
            {
                json.Characters.Remove(Characters[CharacterCombo.Text]);
                Characters.Remove(CharacterCombo.Text);
                CharacterCombo.Items.Remove(CharacterCombo.Text);
                CharacterCombo.Text = "Select Character";
            }
            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
            JsonView.Text = parsedJson;
        }

        private void SkinEveryLevelUp_CheckedChanged(object sender, EventArgs e)
        {
            if (SkinEveryLevelUp.Checked == true)
                tabControlSkins.TabPages.Insert(2, SkinOnEveryLevelUp);
            else
                tabControlSkins.TabPages.Remove(SkinOnEveryLevelUp);
        }

        private void CharHiddenWeaponsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CharHiddenWeapons.Text.Length > 0)
            {
                CharHiddenWeapons.Text = CharHiddenWeapons.Text + "," + CharHiddenWeaponsCombo.Text;
            }
            else
            {
                CharHiddenWeapons.Text = CharHiddenWeaponsCombo.Text;
            }

            CharHiddenWeaponsCombo.ResetText();
        }

        private void CharShowcaseCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CharShowcase.Text.Length > 0)
            {
                CharShowcase.Text = CharShowcase.Text + "," + CharShowcaseCombo.Text;
            }
            else
            {
                CharShowcase.Text = CharShowcaseCombo.Text;
            }

            CharShowcaseCombo.ResetText();
        }

        private void SkinAccCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SkinExAccessories.Text.Length > 0)
            {
                SkinExAccessories.Text = SkinExAccessories.Text + "," + SkinAccCombo.Text;
            }
            else
            {
                SkinExAccessories.Text = SkinAccCombo.Text;
            }

            SkinAccCombo.ResetText();
        }

        private void SkinExWeapCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SkinExWeapons.Text.Length > 0)
            {
                SkinExWeapons.Text = SkinExWeapons.Text + "," + SkinExWeapCombo.Text;
            }
            else
            {
                SkinExWeapons.Text = SkinExWeapCombo.Text;
            }

            SkinExWeapCombo.ResetText();
        }

        private void SkinHiddenCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SkinHiddenWeapons.Text.Length > 0)
            {
                SkinHiddenWeapons.Text = SkinHiddenWeapons.Text + "," + SkinHiddenCombo.Text;
            }
            else
            {
                SkinHiddenWeapons.Text = SkinHiddenCombo.Text;
            }

            SkinHiddenCombo.ResetText();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
            saveFileDialog1.Filter = "Json File| *.json";
            saveFileDialog1.FileName = "character.json";
            saveFileDialog1.ShowDialog();
            using (StreamWriter writer = new StreamWriter(saveFileDialog1.OpenFile()))
            {
                writer.Write(parsedJson);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
            byte[] jsonBytes = Encoding.UTF8.GetBytes(parsedJson);
            saveFileDialog1.Filter = "Zip File| *.zip";
            saveFileDialog1.ShowDialog();
            using (ZipArchive archive = new ZipArchive(saveFileDialog1.OpenFile(), ZipArchiveMode.Create))
            {
                ZipArchiveEntry zipArchiveEntry = archive.CreateEntry("character.json", CompressionLevel.Fastest);
                using (var stream = zipArchiveEntry.Open())
                {
                    stream.Write(jsonBytes, 0, jsonBytes.Length);
                }
                foreach (string key in spriteSheets.Keys)
                {
                    byte[] bytes = spriteSheets[key];
                    ZipArchiveEntry zipArchiveEntry1 = archive.CreateEntry(key, CompressionLevel.Fastest);
                    using (var stream = zipArchiveEntry1.Open())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (spriteCombo.Text != "Select Sprite")
            {
                SkinSprite.Items.Remove(spriteCombo.Text);
                spriteCombo.Items.Remove(spriteCombo.Text);
                spriteCombo.Text = "Select Sprite";
            }
        }
    }
}
