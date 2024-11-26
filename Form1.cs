using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BloodlineJsonEditor
{
    public partial class Form1 : Form
    {
        JsonFile jsonFile = new JsonFile();
        EmptyJson json = new EmptyJson();
        TabPage[] tabPages;
        List<string> sprites = new List<string>();
        List<string> textures = new List<string>();
        Dictionary<string, StatModifier> AltStats = new Dictionary<string, StatModifier>();
        Dictionary<string, CharacterObject> Characters = new Dictionary<string, CharacterObject>();
        Dictionary<string, SkinObject> Skins = new Dictionary<string, SkinObject>();
        NumericUpDown[] rectXs;
        NumericUpDown[] rectYs;
        NumericUpDown[] rectWidths;
        NumericUpDown[] rectHeights;
        NumericUpDown[] pivotXs;
        NumericUpDown[] pivotYs;
        TextBox[] spriteNames;
        TextBox[] textureNames;
        Dictionary<string, byte[]> spriteSheets = new Dictionary<string, byte[]>();

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += pictureBox1_Paint;
            TabPage[] tabPages2 = { tabPage4, tabPage5, tabPage6, tabPage7, tabPage8, tabPage9, tabPage10, tabPage11, 
                tabPage12, tabPage13, tabPage14, tabPage15, tabPage16, tabPage17, tabPage18, tabPage19 };
            tabPages = tabPages2;
            foreach (TabPage page in tabPages)
            {
                tabControl2.TabPages.Remove(page);
            }

            tabControlChar.TabPages.Remove(tabPageCharOn);
            tabControlSkins.TabPages.Remove(SkinOnEveryLevelUp);

            // Probably a better way than doing this
            NumericUpDown[] rectXs2 = { numRectX1, numRectX2, numRectX3, numRectX4, numRectX5, numRectX6, numRectX7, numRectX8,
                numRectX9, numRectX10, numRectX11, numRectX12, numRectX13, numRectX14, numRectX15, numRectX16 };
            NumericUpDown[] rectYs2 = { numRectY1, numRectY2, numRectY3, numRectY4, numRectY5, numRectY6, numRectY7, numRectY8,
                numRectY9, numRectY10, numRectY11, numRectY12, numRectY13, numRectY14, numRectY15, numRectY16 };
            NumericUpDown[] rectWidths2 = { numRectWidth1, numRectWidth2, numRectWidth3, numRectWidth4, numRectWidth5, numRectWidth6, numRectWidth7, numRectWidth8,
                numRectWidth9, numRectWidth10, numRectWidth11, numRectWidth12, numRectWidth13, numRectWidth14, numRectWidth15, numRectWidth16 };
            NumericUpDown[] rectHeights2 = { numRectHeight1, numRectHeight2, numRectHeight3, numRectHeight4, numRectHeight5, numRectHeight6, numRectHeight7, numRectHeight8,
                numRectHeight9, numRectHeight10, numRectHeight11, numRectHeight12, numRectHeight13, numRectHeight14, numRectHeight15, numRectHeight16 };
            NumericUpDown[] pivotXs2 = { numPivotX1, numPivotX2, numPivotX3, numPivotX4, numPivotX5, numPivotX6, numPivotX7, numPivotX8,
                numPivotX9, numPivotX10, numPivotX11, numPivotX12, numPivotX13, numPivotX14, numPivotX15, numPivotX16 };
            NumericUpDown[] pivotYs2 = { numPivotY1, numPivotY2, numPivotY3, numPivotY4, numPivotY5, numPivotY6, numPivotY7, numPivotY8,
                numPivotY9, numPivotY10, numPivotY11, numPivotY12, numPivotY13, numPivotY14, numPivotY15, numPivotY16 };
            TextBox[] spriteNames2 = { textSprite1, textSprite2, textSprite3, textSprite4, textSprite5, textSprite6, textSprite7, textSprite8,
                textSprite9, textSprite10, textSprite11, textSprite12, textSprite13, textSprite14, textSprite15, textSprite16 };
            TextBox[] textureNames2 = { textTexture1, textTexture2, textTexture3, textTexture4, textTexture5, textTexture6, textTexture7, textTexture8,
                textTexture9, textTexture10, textTexture11, textTexture12, textTexture13, textTexture14, textTexture15, textTexture16 };

            rectXs = rectXs2;
            rectYs = rectYs2;
            rectWidths = rectWidths2;
            rectHeights = rectHeights2;
            pivotXs = pivotXs2;
            pivotYs = pivotYs2;
            spriteNames = spriteNames2;
            textureNames = textureNames2;
            jsonFile.wepSet();

            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented);
            richTextBox1.Text = parsedJson;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image|*.png;*.jpg;*.bmp;*.gif";
            openFileDialog1.ShowDialog();
            string selectedFileName = openFileDialog1.FileName;
            pictureBox1.Load(selectedFileName);
        }

        private void pictureBox1_Paint(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                button2.Enabled = true;
                button2.Visible = true;
            }
            else
            {
                button2.Visible = false;
                button2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < numericUpDown1.Value; i++)
            {
                if (!tabControl2.TabPages.Contains(tabPages[i])) tabControl2.TabPages.Insert(i, tabPages[i]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < numericUpDown1.Value; i++)
            {
                string[] filename = pictureBox1.ImageLocation.Split('.');
                filename[0] = filename[0].Substring(filename[0].LastIndexOf("\\"));
                filename[0] = filename[0].Remove(0, 1);
                int baseWidth = pictureBox1.Image.Width / (int)numericUpDown1.Value;
                rectXs[i].Value = baseWidth * i;
                rectWidths[i].Value = baseWidth;
                rectHeights[i].Value = pictureBox1.Image.Height;
                pivotXs[i].Value = baseWidth / 2;
                spriteNames[i].Text = filename[0] + "_0ChangeMe." + filename[1];
                textureNames[i].Text = filename[0] + "." + filename[1];
                if (!tabControl2.TabPages.Contains(tabPages[i])) tabControl2.TabPages.Insert(i, tabPages[i]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<JObject> spriteArray = json.Sprites;
            if (pictureBox1.Image != null)
            {
                string[] filename = pictureBox1.ImageLocation.Split('.');
                filename[0] = filename[0].Substring(filename[0].LastIndexOf("\\"));
                filename[0] = filename[0].Remove(0, 1);
                ImageConverter imageConverter = new ImageConverter();
                byte[] bytes = (byte[])imageConverter.ConvertTo(pictureBox1.Image, typeof(byte[]));
                if (!spriteSheets.ContainsValue(bytes)) spriteSheets.Add(filename[0] + "." + filename[1], bytes);
            }

            for (int i = 0; i < tabControl2.TabPages.Count; i++)
            {
                JObject sprite = jsonFile.baseSpriteJson();
                
                dynamic[] d = new dynamic[8];
                d[0] = (int)rectXs[i].Value;
                d[1] = (int)rectYs[i].Value;
                d[2] = (int)rectWidths[i].Value;
                d[3] = (int)rectHeights[i].Value;
                d[4] = (int)pivotXs[i].Value;
                d[5] = (int)pivotYs[i].Value;
                d[6] = spriteNames[i].Text;
                d[7] = textureNames[i].Text;

                sprite = jsonFile.setSpriteJsonValues(sprite, d);
                json.Sprites.Add(sprite);

                if (!SkinSprite.Items.Contains(d[6]))
                {
                    SkinSprite.Items.Add(d[6]);
                    spriteCombo.Items.Add(d[6]);
                }
                if (!SkinTexture.Items.Contains(d[7].Split('.')[0]))
                {
                    SkinTexture.Items.Add(d[7].Split('.')[0]);
                }
            }
            string parsedJson = JsonConvert.SerializeObject(json, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
            richTextBox1.Text = parsedJson;
        }

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
            altStats.Charm = (float)AltCharm.Value;
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
            altStats.SineArea = (float)AltSineArea.Value;
            altStats.SineCoolDown = (float)AltSineCooldown.Value;
            altStats.SineDuration = (float)AltSineDuration.Value;
            altStats.SineMight = (float)AltSineMight.Value;
            altStats.SineSpeed = (float)AltSineSpeed.Value;
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
                skinEveryLevel.Charm = (float)SkinOnCharm.Value;
                skinEveryLevel.Cooldown = (float)SkinOnCooldown.Value;
                skinEveryLevel.Curse = (float)SkinOnCurse.Value;
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
                skinEveryLevel.SineArea = (float)SkinOnSineArea.Value;
                skinEveryLevel.SineCoolDown = (float)SkinOnSineCooldown.Value;
                skinEveryLevel.SineDuration = (float)SkinOnSineDuration.Value;
                skinEveryLevel.SineMight = (float)SkinOnSineMight.Value;
                skinEveryLevel.SineSpeed = (float)SkinOnSineSpeed.Value;
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
            statModifier.Charm = (float)statsCharm.Value;
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
            statModifier.SineArea = (float)statsSineArea.Value;
            statModifier.SineCoolDown = (float)statsSineCooldown.Value;
            statModifier.SineDuration = (float)statsSineDuration.Value;
            statModifier.SineMight = (float)statsSineMight.Value;
            statModifier.SineSpeed = (float)statsSineSpeed.Value;
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
                charEveryLevel.Charm = (float)OnCharm.Value;
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
                charEveryLevel.SineArea = (float)OnSineArea.Value;
                charEveryLevel.SineCoolDown = (float)OnSineCooldown.Value;
                charEveryLevel.SineDuration = (float)OnSineDuration.Value;
                charEveryLevel.SineMight = (float)OnSineMight.Value;
                charEveryLevel.SineSpeed = (float)OnSineSpeed.Value;
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
            richTextBox1.Text = parsedJson;
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
            richTextBox1.Text = parsedJson;
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
