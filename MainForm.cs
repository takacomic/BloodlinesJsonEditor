using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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
        int CustomSkinCounter = 7000;
        Dictionary<string, StatModifier> AltStats = new Dictionary<string, StatModifier>();
        Dictionary<string, CharacterObject> Characters = new Dictionary<string, CharacterObject>();
        Dictionary<string, SkinObject> Skins = new Dictionary<string, SkinObject>();
        internal Dictionary<string, List<SpriteObject>> TextureToSprites = new Dictionary<string, List<SpriteObject>>();
        Dictionary<string, byte[]> spriteSheets = new Dictionary<string, byte[]>();

        public MainForm()
        {
            spriteHelper = spriteHelper.Instance;
            spriteHelper.mainForm = this;
            InitializeComponent();
            SpriteSheetBox.Paint += pictureBox1_Paint;
            tabControlChar.TabPages.Remove(tabPageCharOn);
            tabControlSkins.TabPages.Remove(SkinOnEveryLevelUp);
            tabControlSkins.TabPages.Remove(tabPage26);
            jsonFile.wepSet();

            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
            JsonView.Text = parsedJson;
        }

        ///
        /// Start Sprite Section
        ///
        private void LoadSpritebyTexture_Click(object sender, EventArgs e)
        {
            if (TextureCombo.Text != "Select Texture")
            {
                foreach (SpriteObject s in TextureToSprites[TextureCombo.Text])
                    spriteHelper.AddSprite(s, s.EditingSpriteRow, s.EditingSprite);
                spriteObject = spriteHelper.GetSprite();
                SpriteObjectSet();
            }
        }
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

            json.Editor.SpriteRows = (int)SpriteRowNum.Value;
        }
        internal void SpritePerRow_ValueChanged(object sender, EventArgs e)
        {
            if (SpritePerRow.Value < CurrentEditingSprite.Value)
            {
                MessageBox.Show("Can't set Number of Sprite Rows under the Current Editing Sprite Row");
                SpritePerRow.Value = CurrentEditingSprite.Maximum;
            }
            else
                CurrentEditingSprite.Maximum = SpritePerRow.Value;

            json.Editor.SpritesInRow = (int)SpritePerRow.Value;
        }
        internal void CurrentSpriteRow_ValueChanged(object sender, EventArgs e)
        {
            SpriteObjectValueSet();
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
                if (SpriteSheetBox.ImageLocation != null)
                {
                    string filename = SpriteSheetBox.ImageLocation.Split('.')[0];
                    filename = filename.Substring(filename.LastIndexOf("\\")).Remove(0, 1);
                    TextureName.Text = filename;
                }

                SpriteName.Text = "";
                numRectX.Value = 0;
                numRectY.Value = 0;
                numRectWidth.Value = 0;
                numRectHeight.Value = 0;
            }
        }
        private void SpriteObjectValueSet()
        {
            spriteObject.SpriteName = SpriteNameAppend.Text + ".png";
            spriteObject.TextureName = TextureName.Text + ".png";
            SpriteRect rect = new SpriteRect();
            rect.X = (int)numRectX.Value;
            rect.Y = (int)numRectY.Value;
            rect.Width = (int)numRectWidth.Value;
            rect.Height = (int)numRectHeight.Value;
            spriteObject.Rect = rect;
            spriteObject.AnimationType = SpriteAnimation.Text;
            spriteObject.SpritePlacement = (int)SpritePlacement.Value;
            spriteObject.EditingSprite = spriteHelper.CurrentSprite;
            spriteObject.EditingSpriteRow = spriteHelper.CurrentRow;

            if (spriteHelper.ChangedSprite(spriteObject))
                spriteHelper.AddSprite(spriteObject);
        }
        private void SpriteName_TextChanged(object sender, EventArgs e)
        {
            SpriteNameAppend.Text = $"{SpriteName.Text}_{SpriteAnimation.Text.ToLower()}0{(int)SpritePlacement.Value}";
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

                SkinTexture.Items.Add(filename);
                idleTexture.Items.Add(filename);
                meleeTexture.Items.Add(filename);
                melee2Texture.Items.Add(filename);
                rangedTexture.Items.Add(filename);
                magicTexture.Items.Add(filename);
                specialTexture.Items.Add(filename);

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

        ///
        /// Begin Character Section
        /// 

        private StatModifier StatModiferApplicator(List<Control> c)
        {
            StatModifier modifier = new StatModifier();

            foreach (Control control in c)
            {
                NumericUpDown num = control as NumericUpDown;
                if (control.Name.Contains("Amount")) modifier.Amount = (float)num.Value;
                if (control.Name.Contains("Area")) modifier.Area = (float)num.Value;
                if (control.Name.Contains("Armor")) modifier.Armor = (float)num.Value;
                if (control.Name.Contains("Banish")) modifier.Banish = (float)num.Value;
                if (control.Name.Contains("Charm")) modifier.Charm = (int)num.Value;
                if (control.Name.Contains("Cooldown")) modifier.Cooldown = (float)num.Value;
                if (control.Name.Contains("Curse")) modifier.Curse = (float)num.Value;
                if (control.Name.Contains("Defang")) modifier.Defang = (float)num.Value;
                if (control.Name.Contains("Duration")) modifier.Duration = (float)num.Value;
                if (control.Name.Contains("Fever")) modifier.Fever = (float)num.Value;
                if (control.Name.Contains("Greed")) modifier.Greed = (float)num.Value;
                if (control.Name.Contains("Growth")) modifier.Growth = (float)num.Value;
                if (control.Name.Contains("Invul")) modifier.Invul = (float)num.Value;
                if (control.Name.Contains("Level")) modifier.Level = (int)num.Value;
                if (control.Name.Contains("Luck")) modifier.Luck = (float)num.Value;
                if (control.Name.Contains("Magnet")) modifier.Magnet = (float)num.Value;
                if (control.Name.Contains("Hp")) modifier.MaxHp = (int)num.Value;
                if (control.Name.Contains("Move")) modifier.MoveSpeed = (float)num.Value;
                if (control.Name.Contains("Power")) modifier.Power = (float)num.Value;
                if (control.Name.Contains("Regen")) modifier.Regen = (float)num.Value;
                if (control.Name.Contains("Rerolld")) modifier.Rerolls = (float)num.Value;
                if (control.Name.Contains("Revivals")) modifier.Revivals = (float)num.Value;
                if (control.Name.Contains("Shields")) modifier.Shields = (float)num.Value;
                if (control.Name.Contains("Shroud")) modifier.Shroud = (float)num.Value;
                if (control.Name.Contains("Skips")) modifier.Skips = (float)num.Value;
                if (control.Name.Contains("Speed")) modifier.Speed = (float)num.Value;

                /*modifier.SineArea = AltSineArea.Value.ToString();
                modifier.SineCoolDown = AltSineCooldown.Value.ToString();
                modifier.SineDuration = AltSineDuration.Value.ToString();
                modifier.SineMight = AltSineMight.Value.ToString();
                modifier.SineSpeed = AltSineSpeed.Value.ToString();*/
            }

            return modifier;
        }

        private decimal StatIsNull (decimal value)
        {
            if (value != null) return value;

            return 0;
        }

        private void StatModiferApplicatorInvert(string s, StatModifier modifier)
        {
            List<Control> c = new List<Control>();
            if (s == "stats")
            {
                ControlAdder(c, StandardStats.Controls, s);
                ControlAdder(c, SpoopyStats.Controls, s);
            }

            if( s == "On")
            {
                ControlAdder(c, StandardStatsOn.Controls, s);
                ControlAdder(c, SpoopyStatsOn.Controls, s);
            }

            if ( s == "Alt")
            {
                ControlAdder(c, StandardAlt.Controls, s);
                ControlAdder(c, SpoopyAlt.Controls, s);
            }

            if (s == "SkinOn")
            {
                ControlAdder(c, StandardSkin.Controls, s);
                ControlAdder(c, SpoopySkin.Controls, s);
            }

            foreach (Control control in c)
            {
                NumericUpDown num = control as NumericUpDown;
                if (control.Name.Contains("Amount")) num.Value = (decimal)modifier.Amount;
                if (control.Name.Contains("Area")) num.Value = (decimal)modifier.Area;
                if (control.Name.Contains("Armor")) num.Value = (decimal)modifier.Armor;
                if (control.Name.Contains("Banish")) num.Value = (decimal)modifier.Banish;
                if (control.Name.Contains("Charm")) num.Value = modifier.Charm;
                if (control.Name.Contains("Cooldown")) num.Value = (decimal)modifier.Cooldown;
                if (control.Name.Contains("Curse")) num.Value = (decimal)modifier.Curse;
                if (control.Name.Contains("Defang")) num.Value = (decimal)modifier.Defang;
                if (control.Name.Contains("Duration")) num.Value = (decimal)modifier.Duration;
                if (control.Name.Contains("Fever")) num.Value = (decimal)modifier.Fever;
                if (control.Name.Contains("Greed")) num.Value = (decimal)modifier.Greed;
                if (control.Name.Contains("Growth")) num.Value = (decimal)modifier.Growth;
                if (control.Name.Contains("Invul")) num.Value = (decimal)modifier.Invul;
                if (control.Name.Contains("Level")) num.Value = modifier.Level;
                if (control.Name.Contains("Luck")) num.Value = (decimal)modifier.Luck;
                if (control.Name.Contains("Magnet")) num.Value = (decimal)modifier.Magnet;
                if (control.Name.Contains("Hp")) num.Value = (decimal)modifier.MaxHp;
                if (control.Name.Contains("Move")) num.Value = (decimal)modifier.MoveSpeed;
                if (control.Name.Contains("Power")) num.Value = (decimal)modifier.Power;
                if (control.Name.Contains("Regen")) num.Value = (decimal)modifier.Regen;
                if (control.Name.Contains("Rerolld")) num.Value = (decimal)modifier.Rerolls;
                if (control.Name.Contains("Revivals")) num.Value = (decimal)modifier.Revivals;
                if (control.Name.Contains("Shields")) num.Value = (decimal)modifier.Shields;
                if (control.Name.Contains("Shroud")) num.Value = (decimal)modifier.Shroud;
                if (control.Name.Contains("Skips")) num.Value = (decimal)modifier.Skips;
                if (control.Name.Contains("Speed")) num.Value = (decimal)modifier.Speed;

                /*modifier.SineArea = AltSineArea.Value.ToString();
                modifier.SineCoolDown = AltSineCooldown.Value.ToString();
                modifier.SineDuration = AltSineDuration.Value.ToString();
                modifier.SineMight = AltSineMight.Value.ToString();
                modifier.SineSpeed = AltSineSpeed.Value.ToString();*/
            }
        }

        //
        // Begin Skin Subsection
        //

        private bool animChecker(List<Control> lc, string s)
        {
            int pass = 0;
            foreach (Control c in lc)
                if (c.Name.Contains(s))
                    if (c.Text != "" && c.Text != "0")
                        pass++;

            if(pass == 4) return true;
            return false;
        }
        private void ControlAdder(List<Control> lc, Control.ControlCollection a, string sep)
        {
            foreach(Control c in a)
                if(c.Name.Contains(sep))
                    lc.Add(c);
        }
        private AnimBaseObject AnimObjectSetter(List<Control> lc, string sep)
        {
            AnimBaseObject anim = new AnimBaseObject();

            foreach (Control c in lc)
                if (c.Name.Contains(sep))
                {
                    if(c.Name.Contains("Texture")) anim.TextureName = c.Text;
                    if (c.Name.Contains("Sprite")) anim.SpriteName = c.Text;
                    if (c.Name.Contains("Frames")) anim.FramesNumber = Int32.Parse(c.Text);
                    if (c.Name.Contains("Rate")) anim.FrameRate = Int32.Parse(c.Text);
                }
            return anim;
        }
        private void AnimObjectSetterInvert(List<Control> lc, AnimBaseObject anim, string sep)
        {
            foreach (Control c in lc)
                if (c.Name.Contains(sep))
                {
                    if (c.Name.Contains("Texture"))  c.Text = anim.TextureName;
                    if (c.Name.Contains("Sprite"))  c.Text = anim.SpriteName;
                    if (c.Name.Contains("Frames"))  c.Text = anim.FramesNumber.ToString();
                    if (c.Name.Contains("Rate"))  c.Text = anim.FrameRate.ToString();
                }
        }
        private void addSkinButton(object sender, EventArgs e)
        {
            if (SkinSprite.Text == "" || SkinTexture.Text == "" || SkinType.Text == "")
                MessageBox.Show("Neccessary entries on your skin are empty. Look for the * to find the neccessary entries");
            else
            {
                List<Control> controls = new List<Control>();
                if (SkinEveryLevelUp.Checked)
                {
                    ControlAdder(controls, StandardSkinOn.Controls, "SkinOn");
                    ControlAdder(controls, SpoopySkinOn.Controls, "SkinOn");
                }
                StatModifier skinEveryLevel = StatModiferApplicator(controls);
            
                SpriteAnimsObject spriteAnims = new SpriteAnimsObject();
                AnimBaseObject animBaseObject = new AnimBaseObject();
                List<Control> animControls = new List<Control>();
                ControlAdder(animControls, IdleGroup.Controls, "idle");
                ControlAdder(animControls, MeleeGroup.Controls, "melee");
                ControlAdder(animControls, Melee2Group.Controls, "melee2");
                ControlAdder(animControls, MagicGroup.Controls, "magic");
                ControlAdder(animControls, RangeGroup.Controls, "ranged");
                ControlAdder(animControls, SpecialGroup.Controls, "special");
            
                if (animChecker(animControls, "idle"))
                    spriteAnims.IdleAnimation = AnimObjectSetter(animControls, "idle");

                if (animChecker(animControls, "melee"))
                    spriteAnims.MeleeAttack = AnimObjectSetter(animControls, "melee");

                if (animChecker(animControls, "melee2"))
                    spriteAnims.MeleeAttack2 = AnimObjectSetter(animControls, "melee2");

                if (animChecker(animControls, "ranged"))
                    spriteAnims.RangedAttack = AnimObjectSetter(animControls, "ranged");

                if (animChecker(animControls, "magic"))
                    spriteAnims.MagicAttack = AnimObjectSetter(animControls, "magic");

                if (animChecker(animControls, "special"))
                    spriteAnims.SpecialAnimation = AnimObjectSetter(animControls, "special");

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
                skin.SpriteAnims = spriteAnims;
                skin.SpriteName = SkinSprite.Text;
                skin.Suffix = SkinSuffix.Text;
                skin.TextureName = SkinTexture.Text;
                skin.Unlocked = SkinUnlocked.Checked;
                skin.WalkingFrames = (int)WalkingFrames.Value;

                if (Skins.ContainsKey($"{SkinType.Text}"))
                    MessageBox.Show($"{SkinType.Text} already has stats assigned. Delete Them First");
                else
                    {
                    if (SkinType.Text == "Custom")
                    {
                        SkinType.Text = $"{CustomSkinCounter}";
                        skin.SkinType = SkinType.Text;
                        CustomSkinCounter++;
                    }
                    Skins.Add($"{SkinType.Text}", skin);
                    SkinCombo.Items.Add($"{SkinType.Text}");
                }
            }
        }
        private void SkinEveryLevelUp_CheckedChanged(object sender, EventArgs e)
        {
            if (SkinEveryLevelUp.Checked == true)
                tabControlSkins.TabPages.Insert(2, SkinOnEveryLevelUp);
            else
                tabControlSkins.TabPages.Remove(SkinOnEveryLevelUp);
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
        private void LoadRemoveSkin_Click(object sender, EventArgs e)
        {
            if (SkinCombo.Text != "Select Skin")
            {
                SkinObject skin = Skins[SkinCombo.Text];
                if (skin.OnEveryLevelUp != null)
                {
                    CharOnEveryLevelUp.Checked = true;
                    StatModiferApplicatorInvert("SkinOn", skin.OnEveryLevelUp);
                }
                if(skin.GreaterThanZero())
                {
                    SkinStats.Checked = true;
                    SkinAmount.Value = (decimal)skin.Amount;
                    SkinArea.Value = (decimal)skin.Area;
                    SkinArmor.Value = (decimal)skin.Armor;
                    SkinBanish.Value = (decimal)skin.Banish;
                    SkinCharm.Value = (decimal)skin.Charm;
                    SkinCooldown.Value = (decimal)skin.Cooldown;
                    SkinCurse.Value = (decimal)skin.Curse;
                    SkinDuration.Value = (decimal)skin.Duration;
                    SkinGreed.Value = (decimal)skin.Greed;
                    SkinGrowth.Value = (decimal)skin.Growth;
                    SkinLuck.Value = (decimal)skin.Luck;
                    SkinMagnet.Value = (decimal)skin.Magnet;
                    SkinHp.Value = (decimal)skin.MaxHp;
                    SkinMove.Value = (decimal)skin.MoveSpeed;
                    SkinPower.Value = (decimal)skin.Power;
                    SkinRegen.Value = (decimal)skin.Regen;
                    SkinRerolls.Value = (decimal)skin.Rerolls;
                    SkinRevivals.Value = (decimal)skin.Revivals;
                    SkinShields.Value = (decimal)skin.Shields;
                    SkinSkips.Value = (decimal)skin.Skips;
                    SkinSpeed.Value = (decimal)skin.Speed;
                }
                else
                    SkinStats.Checked = false;

                SkinAnimated.Checked = skin.AlwaysAnimated;
                SkinUnlocked.Checked = skin.Unlocked;
                SkinSprite.Text = skin.SpriteName;
                SkinTexture.Text = skin.TextureName;
                SkinType.Text = skin.SkinType;
                WalkingFrames.Value = skin.WalkingFrames;
                SkinPrefix.Text = skin.Prefix;
                SkinName.Text = skin.Name;
                SkinSuffix.Text = skin.Suffix;
                SkinDescription.Text = skin.Description;
                SkinPrice.Value = (decimal)skin.Price;

                if (skin.ExWeapons != null && skin.ExWeapons.Count > 0)
                    foreach (string s in skin.ExWeapons)
                    {
                        if (SkinExWeapons.Text.Length > 0)
                            SkinExWeapons.Text = SkinExWeapons.Text + "," + jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                        else
                            SkinExWeapons.Text = jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                    }
                if (skin.HiddenWeapons != null && skin.HiddenWeapons.Count > 0)
                    foreach (string s in skin.HiddenWeapons)
                    {
                        if (SkinHiddenWeapons.Text.Length > 0)
                            SkinHiddenWeapons.Text = SkinHiddenWeapons.Text + "," + jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                        else
                            SkinHiddenWeapons.Text = jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                    }
                if (skin.ExAccessories != null && skin.ExAccessories.Count > 0)
                    foreach (string s in skin.ExAccessories)
                    {
                        if (SkinExAccessories.Text.Length > 0)
                            SkinExAccessories.Text = SkinExAccessories.Text + "," + jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                        else
                            SkinExAccessories.Text = jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                    }

                List<Control> animControls = new List<Control>();
                ControlAdder(animControls, IdleGroup.Controls, "idle");
                ControlAdder(animControls, MeleeGroup.Controls, "melee");
                ControlAdder(animControls, Melee2Group.Controls, "melee2");
                ControlAdder(animControls, MagicGroup.Controls, "magic");
                ControlAdder(animControls, RangeGroup.Controls, "ranged");
                ControlAdder(animControls, SpecialGroup.Controls, "special");

                SpriteAnimsObject spriteAnimsObject = skin.SpriteAnims;
                if (spriteAnimsObject != null)
                {
                    if (spriteAnimsObject.IdleAnimation != null)
                        AnimObjectSetterInvert(animControls, spriteAnimsObject.IdleAnimation, "idle");
                    if (spriteAnimsObject.MeleeAttack != null)
                        AnimObjectSetterInvert(animControls, spriteAnimsObject.MeleeAttack, "melee");
                    if (spriteAnimsObject.MeleeAttack2 != null)
                        AnimObjectSetterInvert(animControls, spriteAnimsObject.MeleeAttack2, "melee2");
                    if (spriteAnimsObject.RangedAttack != null)
                        AnimObjectSetterInvert(animControls, spriteAnimsObject.RangedAttack, "ranged");
                    if (spriteAnimsObject.MagicAttack != null)
                        AnimObjectSetterInvert(animControls, spriteAnimsObject.MagicAttack, "magic");
                    if (spriteAnimsObject.SpecialAnimation != null) 
                        AnimObjectSetterInvert(animControls, spriteAnimsObject.SpecialAnimation, "special");
                }

                Skins.Remove(SkinCombo.Text);
                SkinCombo.Items.Remove(SkinCombo.Text);
                SkinCombo.Text = "Select Skin";
            }
        }
        private void SkinStats_CheckedChanged(object sender, EventArgs e)
        {
            if (CharOnEveryLevelUp.Checked == true)
                tabControlSkins.TabPages.Insert(1, tabPage26);
            else
                tabControlSkins.TabPages.Remove(tabPage26);
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

        //
        // End Skin Subsection
        //

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CharOnEveryLevelUp.Checked == true)
                tabControlChar.TabPages.Insert(3, tabPageCharOn);
            else
                tabControlChar.TabPages.Remove(tabPageCharOn);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (Characters.ContainsKey($"{CharName.Text}"))
                MessageBox.Show($"{CharName.Text} already has exists. Delete Them First");
            else if (Skins.Count == 0)
            {
                addSkinButton(sender, e);
                if (Skins.Count == 1) button7_Click(sender, e);
            }
            else if (CharName.Text == "")
                MessageBox.Show("Character Name cannot be left blank");
            else
            {
                List<Control> controls = new List<Control>();
                ControlAdder(controls, StandardStats.Controls, "stats");
                ControlAdder(controls, SpoopyStats.Controls, "stats");
                StatModifier statModifier = StatModiferApplicator(controls);
                statModifier.Level = 1;


                StatModifier charEveryLevel = new StatModifier();
                if (CharOnEveryLevelUp.Checked)
                {
                    controls = new List<Control>();
                    ControlAdder(controls, StandardStatsOn.Controls, "On");
                    ControlAdder(controls, SpoopyStatsOn.Controls, "On");
                    charEveryLevel = StatModiferApplicator(controls);
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
                if (CharName.Text == "") CharName.Text = "Empty Name";
                character.CharName = CharName.Text;
                character.CurrentSkin = CharCurrentSkin.Text;
                character.Description = CharDescription.Text;
                character.ExLevels = (int)CharExLevels.Value;
                character.ExWeapons = exweap;
                character.Hidden = CharHidden.Checked;
                character.HiddenWeapons = hiddenweap;
                character.IsBought = CharIsBought.Checked;
                character.Level = 1;
                if (CharOnEveryLevelUp.Checked) character.OnEveryLevelUp = charEveryLevel.Cleaner();
                //character.PortraitName = CharPortrait.Text;
                character.Prefix = CharPrefix.Text;
                character.Price = (int)CharPrice.Value;
                character.Showcase = showcase;
                if (Skins.Count > 0)
                    foreach (SkinObject skin in Skins.Values)
                        character.Skins.Add(skin);
                Skins.Clear();
                SkinCombo.Items.Clear();
                character.StartingWeapon = jsonFile.wepNameToKey[CharStartingWeapon.Text];
                character.Suffix = CharSuffix.Text;
                character.Surname = CharSurname.Text;
                character.StatModifiers.Add(statModifier.Cleaner());
                if (AltStats.Count > 0)
                    foreach (StatModifier stat in AltStats.Values)
                        character.StatModifiers.Add(stat);
                AltStats.Clear();
                AltStatBox.Items.Clear();

                Characters.Add(CharName.Text, character);
                CharacterCombo.Items.Add(CharName.Text);
                json.Characters.Add(character);
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
        private void LoadCharacter_Click(object sender, EventArgs e)
        {
            if (CharacterCombo.Text != "Select Character")
            {
                CharacterObject character = Characters[CharacterCombo.Text];
                List<SkinObject> skinObjects = character.Skins;
                List<StatModifier> statModifiers = character.StatModifiers;
                /// Character Data
                if (character.OnEveryLevelUp != null)
                {
                    CharOnEveryLevelUp.Checked = true;
                    StatModiferApplicatorInvert("On", character.OnEveryLevelUp);
                }
                CharIsBought.Checked = character.IsBought;
                CharStartingWeapon.Text = jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == character.StartingWeapon).Key;
                CharCurrentSkin.Text = character.CurrentSkin;
                CharName.Text = character.CharName;
                CharPrefix.Text = character.Prefix;
                CharSuffix.Text = character.Suffix;
                CharSurname.Text = character.Surname;
                CharExLevels.Value = character.ExLevels;
                CharPrice.Value = character.Price;
                CharDescription.Text = character.Description;
                PortraitSprite.Text = character.PortraitName;
                if(character.ExWeapons != null && character.ExWeapons.Count > 0)
                    foreach( string s in character.ExWeapons)
                {
                    if (CharExWeapons.Text.Length > 0)
                        CharExWeapons.Text = CharExWeapons.Text + "," + jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                    else
                        CharExWeapons.Text = jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                }
                if (character.HiddenWeapons != null && character.HiddenWeapons.Count > 0)
                    foreach (string s in character.HiddenWeapons)
                {
                    if (CharHiddenWeapons.Text.Length > 0)
                        CharHiddenWeapons.Text = CharHiddenWeapons.Text + "," + jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                    else
                        CharHiddenWeapons.Text = jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                }
                if (character.Showcase != null&& character.Showcase.Count > 0 )
                    foreach (string s in character.Showcase)
                {
                    if (CharShowcase.Text.Length > 0)
                        CharShowcase.Text = CharShowcase.Text + "," + jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                    else
                        CharShowcase.Text = jsonFile.wepNameToKey.FirstOrDefault(x => x.Value == s).Key;
                }
                /// End
                /// StatModifiers
                foreach (StatModifier modifier in statModifiers)
                    if (modifier.Level == 1)
                       StatModiferApplicatorInvert("stats", modifier);
                    else
                    {
                       AltStats.Add($"Level {modifier.Level}", modifier);
                       AltStatBox.Items.Add($"Level {modifier.Level}");
                    }
                /// End
                /// Skins
                foreach (SkinObject skinObject in skinObjects)
                {
                   Skins.Add($"{skinObject.SkinType}", skinObject);
                    SkinCombo.Items.Add($"{skinObject.SkinType}");
                }
                
                Console.WriteLine(json.Characters.Remove(character));
                Characters.Remove(CharacterCombo.Text);
                CharacterCombo.Items.Remove(CharacterCombo.Text);
                CharacterCombo.Text = "Select Character";

                string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                JsonView.Text = parsedJson;
            }
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
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CharExWeapons.Text.Length > 0)
                CharExWeapons.Text = CharExWeapons.Text + "," + CharExWeaponsCombo.Text;
            else
                CharExWeapons.Text = CharExWeaponsCombo.Text;

            CharExWeaponsCombo.ResetText();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            List<Control> controls = new List<Control>();

            foreach (Control c in StandardAlt.Controls)
                if (c.Name.Contains("Alt")) controls.Add(c);
            foreach (Control c in SpoopyAlt.Controls)
                if (c.Name.Contains("Alt")) controls.Add(c);

            StatModifier altStats = StatModiferApplicator(controls);
            altStats.Level = (int)AltLevel.Value;

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
        private void LoadRemoveAlt_Click(object sender, EventArgs e)
        {
            if (AltStatBox.Text != "Select Alt Stats")
            {
                StatModiferApplicatorInvert("Alt", AltStats[AltStatBox.Text]);
                AltStats.Remove(AltStatBox.Text);
                AltStatBox.Items.Remove(AltStatBox.Text);
                AltStatBox.Text = "Select Alt Stats";
            }
        }
        private void RemoveAltStatButton_Click(object sender, EventArgs e)
        {
            if (AltStatBox.Text != "Select Alt Stats")
            {
                AltStats.Remove(AltStatBox.Text);
                AltStatBox.Items.Remove(AltStatBox.Text);
                AltStatBox.Text = "Select Alt Stats";
            }
        }

        /// 
        /// End Character Section
        /// 

        private void button10_Click(object sender, EventArgs e)
        {
            if(Characters.Count == 0)
                button7_Click(sender, e);
            else
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
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (Characters.Count == 0)
                button7_Click(sender, e);
            else
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

        private void statsAmount_ValueChanged(object sender, EventArgs e)
        {
            StatInfoValue(statsAmount, SkinAmount, AmountInfo, SkinAmountInfo);
        }
        private void statsArea_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsArea, SkinArea, AreaInfo, SkinAreaInfo);
        }
        private void statsArmor_ValueChanged(object sender, EventArgs e)
        {
            StatInfoValue(statsArmor, SkinArmor, ArmorInfo, SkinArmorInfo);
        }
        private void statsBanish_ValueChanged(object sender, EventArgs e)
        {
            StatInfoValue(statsBanish, SkinBanish, BanishInfo, SkinBanishInfo);
        }
        private void statsCooldown_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsCooldown, SkinCooldown, CooldownInfo, SkinCooldownInfo);
        }
        private void statsCurse_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsCurse, SkinCurse, CurseInfo, SkinCurseInfo);
        }
        private void statsDuration_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsDuration, SkinDuration, DurationInfo, SkinDurationInfo);
        }
        private void statsGreed_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsGreed, SkinGreed, GreedInfo, SkinGreedInfo);
        }
        private void statsGrowth_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsGrowth, SkinGrowth, GrowthInfo, SkinGrowthInfo);
        }
        private void statsLuck_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsLuck, SkinLuck, LuckInfo, SkinLuckInfo);
        }
        private void statsMagnet_ValueChanged(object sender, EventArgs e)
        {
            if (statsMagnet.Value > 0)
            {
                MagnetInfo.Text = "+" + (int)(statsMagnet.Value * 100) + "%";
                SkinMagnetInfo.Text = "+" + (int)((statsMagnet.Value + SkinMagnet.Value) * 100) + "%";
            }
            else if (statsMagnet.Value < 0)
            {
                MagnetInfo.Text = "" + (int)(statsMagnet.Value * 100) + "%";
                SkinMagnetInfo.Text = "" + (int)((statsMagnet.Value + SkinMagnet.Value) * 100) + "%";
            }
            else
            {
                MagnetInfo.Text = "-";
                SkinMagnetInfo.Text = "-";
            }
        }
        private void statsHp_ValueChanged(object sender, EventArgs e)
        {
            if (statsHp.Value > 0)
            {
                MaxHealthInfo.Text = "" + (int)statsHp.Value;
                SkinMaxHealthInfo.Text = "" + (int)(statsHp.Value + SkinHp.Value);
            }
            else
            {
                MaxHealthInfo.Text = "-";
                SkinMaxHealthInfo.Text = "-";
            }
        }
        private void statsMove_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsMove, SkinMove, MoveSpeedInfo, SkinMoveSpeedInfo);
        }
        private void statsPower_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsPower, SkinPower, MightInfo, SkinMightInfo);
        }
        private void statsRegen_ValueChanged(object sender, EventArgs e)
        {
            if (statsRegen.Value > 0)
            {
                RecoveryInfo.Text = "" + statsRegen.Value;
                SkinRecoveryInfo.Text = "" + (statsRegen.Value + SkinRegen.Value);
            }
            else if (statsRegen.Value < 0)
            {
                RecoveryInfo.Text = "" + statsRegen.Value;
                SkinRecoveryInfo.Text = "" + (statsRegen.Value + SkinRegen.Value);
            }
            else
            {
                RecoveryInfo.Text = "-";
                SkinRecoveryInfo.Text = "-";
            }
        }
        private void statsReroll_ValueChanged(object sender, EventArgs e)
        {
            StatInfoValue(statsReroll, SkinRerolls, RerollInfo, SkinRerollInfo);
        }
        private void statsRevivals_ValueChanged(object sender, EventArgs e)
        {
            StatInfoValue(statsRevivals, SkinRevivals, RevivalInfo, SkinRevivalInfo);
        }
        private void statsSkips_ValueChanged(object sender, EventArgs e)
        {
            StatInfoValue(statsSkips, SkinSkips, SkipInfo ,SkinSkipInfo);
        }
        private void statsSpeed_ValueChanged(object sender, EventArgs e)
        {
            StatInfoPercent(statsSpeed, SkinSpeed, SpeedInfo, SkinSpeedInfo);
        }




        private void SkinAmount_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoValue(statsAmount, SkinAmount, SkinAmountInfo);
        }
        private void SkinArea_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsArea, SkinArea, SkinAreaInfo);
        }
        private void SkinArmor_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoValue(statsArmor, SkinArmor, SkinArmorInfo);
        }
        private void SkinBanish_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoValue(statsBanish, SkinBanish, SkinBanishInfo);
        }
        private void SkinCooldown_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsCooldown, SkinCooldown, SkinCooldownInfo);
        }
        private void SkinCurse_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsCurse, SkinCurse, SkinCurseInfo);
        }
        private void SkinDuration_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsDuration, SkinDuration, SkinDurationInfo);
        }
        private void SkinGreed_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsGreed, SkinGreed, SkinGreedInfo);
        }
        private void SkinGrowth_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsGrowth, SkinGrowth, SkinGrowthInfo);
        }
        private void SkinLuck_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsLuck, SkinLuck, SkinLuckInfo);
        }
        private void SkinMove_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsMove, SkinMove, SkinMoveSpeedInfo);
        }
        private void SkinMagnet_ValueChanged(object sender, EventArgs e)
        {
            if (statsMagnet.Value + SkinMagnet.Value > 0)
                SkinMagnetInfo.Text = "+" + (int)((statsMagnet.Value + SkinMagnet.Value) * 100) + "%";
            else if (statsMagnet.Value + SkinMagnet.Value < 0)
                SkinMagnetInfo.Text = "" + (int)((statsMagnet.Value + SkinMagnet.Value) * 100) + "%";
            else
                SkinMagnetInfo.Text = "-";
        }
        private void SkinHp_ValueChanged(object sender, EventArgs e)
        {
            if (statsHp.Value + SkinHp.Value > 0)
                SkinMaxHealthInfo.Text = "" + (int)(statsHp.Value + SkinHp.Value);
            else
                SkinMaxHealthInfo.Text = "-";
        }
        private void SkinPower_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsPower, SkinPower, SkinMightInfo);
        }
        private void SkinRegen_ValueChanged(object sender, EventArgs e)
        {
            if (statsRegen.Value + SkinRegen.Value > 0)
                SkinRecoveryInfo.Text = "" + (statsRegen.Value + SkinRegen.Value);
            else if (statsRegen.Value + SkinRegen.Value < 0)
                SkinRecoveryInfo.Text = "" + (statsRegen.Value + SkinRegen.Value);
            else
                SkinRecoveryInfo.Text = "-";
        }
        private void SkinRerolls_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoValue(statsReroll, SkinRerolls, SkinRerollInfo);
        }
        private void SkinRevivals_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoValue(statsRevivals, SkinRevivals, SkinRevivalInfo);
        }
        private void SkinSkips_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoValue(statsSkips, SkinSkips, SkinSkipInfo);
        }
        private void SkinSpeed_ValueChanged(object sender, EventArgs e)
        {
            SkinInfoPercent(statsSpeed, SkinSpeed, SkinSpeedInfo);
        }

        private void StatInfoPercent(NumericUpDown a, NumericUpDown b, TextBox c, TextBox d)
        {
            if (a.Value > 1)
            {
                c.Text = "+" + (int)((a.Value - 1) * 100) + "%";
                d.Text = "+" + (int)((a.Value - 1 + b.Value) * 100) + "%";
            }
            else if (a.Value < 1)
            {
                c.Text = "" + (int)((a.Value - 1) * 100) + "%";
                d.Text = "" + (int)((a.Value - 1 + b.Value) * 100) + "%";
            }
            else
            {
                c.Text = "-";
                d.Text = "-";
            }
        }
        private void SkinInfoPercent(NumericUpDown a, NumericUpDown b, TextBox c)
        {
            if (a.Value + b.Value > 1)
                c.Text = "+" + (int)((a.Value - 1 + b.Value) * 100) + "%";
            else if (a.Value + b.Value < 1)
                c.Text = "" + (int)((a.Value - 1 + b.Value) * 100) + "%";
            else
                c.Text = "-";
        }
        private void StatInfoValue(NumericUpDown a, NumericUpDown b, TextBox c, TextBox d)
        {
            if (a.Value > (decimal)0.99)
            {
                c.Text = "+" + (int)a.Value;
                d.Text = "+" + (int)(a.Value + b.Value);
            }
            else if (a.Value < (decimal)-0.99)
            {
                c.Text = "" + (int)a.Value;
                d.Text = "" + (int)(a.Value + b.Value);
            }
            else
            {
                c.Text = "-";
                d.Text = "-";
            }
        }
        private void SkinInfoValue(NumericUpDown a, NumericUpDown b, TextBox c)
        {
            if (a.Value + b.Value > (decimal)0.99)
                c.Text = "+" + (int)(a.Value + b.Value);
            else if (a.Value + b.Value < (decimal)-0.99)
                c.Text = "" + (int)(a.Value + b.Value);
            else
                c.Text = "-";
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void checkedListBox1_MouseLeave(object sender, EventArgs e)
        {
            checkedListBox1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkedListBox1.Visible = true;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string Checked = checkedListBox1.Items[e.Index].ToString();
            if (Checked == "Weird Stats")
                if (e.NewValue == CheckState.Checked)
                {
                    SpoopyStats.Visible = true;
                    SpoopyAlt.Visible = true;
                    SpoopyStatsOn.Visible = true;
                    SpoopySkin.Visible = true;
                    SpoopySkinOn.Visible = true;
                }
                else
                {
                    SpoopyStats.Visible = false;
                    SpoopyAlt.Visible = false;
                    SpoopyStatsOn.Visible = false;
                    SpoopySkin.Visible = false;
                    SpoopySkinOn.Visible = false;
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Json|*.json";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string file = File.ReadAllText(openFileDialog1.FileName);
                JObject jobject = JObject.Parse(file);
                if (jobject["version"].ToString() == "0.3" && jobject["editorData"] != null)
                {
                    List<CharacterObject> characters = jobject["characters"].ToObject<List<CharacterObject>>();
                    EditorObject editorObject = jobject["editorData"].ToObject<EditorObject>();

                    SpriteRowNum.Value = editorObject.SpriteRows;
                    SpritePerRow.Value = editorObject.SpritesInRow;
                    CustomSkinCounter = editorObject.customSkinCounter;
                    TextureToSprites = new Dictionary<string, List<SpriteObject>>();
                    string textureName = "";

                    foreach (SpriteObject s in jobject["spriteData"].ToObject<List<SpriteObject>>())
                    {
                        if (textureName == "") textureName = s.TextureName;
                        if (s.TextureName != textureName)
                        {
                            json = spriteHelper.RegisterSprites(json);
                            textureName = s.TextureName;
                        }
                        spriteHelper.AddSprite(s, s.EditingSpriteRow, s.EditingSprite);
                        
                    }
                    json = spriteHelper.RegisterSprites(json);

                    foreach (CharacterObject c in jobject["characterData"].ToObject<List<CharacterObject>>())
                    {
                        Characters.Add($"{c.CharName}", c);
                        CharacterCombo.Items.Add($"{c.CharName}");
                        json.Characters.Add(c);
                    }
                    string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                    JsonView.Text = parsedJson;
                }
                else
                {
                    MessageBox.Show("Old character json, Unable to import detailed sprite data (Not implemented)");

                    foreach(SpriteObject s in jobject["spriteData"].ToObject<List<SpriteObject>>())
                    {
                        json.Sprites.Add(s);
                        if(s.SpriteName.Contains("01")) spriteCombo.Items.Add(s.SpriteName);
                    }

                    foreach (CharacterObject c in jobject["characters"].ToObject<List<CharacterObject>>())
                    {
                        Characters.Add($"{c.CharName}", c);
                        CharacterCombo.Items.Add($"{c.CharName}");
                        json.Characters.Add(c);
                    }

                    string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                    JsonView.Text = parsedJson;
                }
            }
        }

        private void spriteCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
    }
}
