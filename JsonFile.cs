using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BloodlineJsonEditor
{
    internal class JsonFile
    {
        public Dictionary<string, string> wepNameToKey { get; } = new Dictionary<string, string>();
        
        //Why this worked but not setting during initialization beats me
        public void wepSet()
        {
            wepNameToKey.Add("Magic Wand", "MAGIC_MISSILE");
            wepNameToKey.Add("Holy Wand", "HOLY_MISSILE" ); 
            wepNameToKey.Add("Whip", "WHIP" ); 
            wepNameToKey.Add("Bloody Tear", "VAMPIRICA" ); 
            wepNameToKey.Add("Axe", "AXE" ); 
            wepNameToKey.Add("Death Spiral", "SCYTHE" ); 
            wepNameToKey.Add("Knife", "KNIFE" ); 
            wepNameToKey.Add("Thousand Edge", "THOUSAND" ); 
            wepNameToKey.Add("Santa Water", "HOLYWATER" ); 
            wepNameToKey.Add("La Borra", "BORA" ); 
            wepNameToKey.Add("Runetracer", "DIAMOND" ); 
            wepNameToKey.Add("Fire Wand", "FIREBALL" ); 
            wepNameToKey.Add("Hellfire", "HELLFIRE" ); 
            wepNameToKey.Add("King Bible", "HOLYBOOK" ); 
            wepNameToKey.Add("Unholy Vespers", "VESPERS" ); 
            wepNameToKey.Add("Cross", "CROSS" ); 
            wepNameToKey.Add("Heaven Sword", "HEAVENSWORD" ); 
            wepNameToKey.Add("Garlic", "GARLIC" ); 
            wepNameToKey.Add("Soul Eater", "VORTEX" ); 
            wepNameToKey.Add("Laurel", "LAUREL" ); 
            wepNameToKey.Add("Lightning Ring", "LIGHTNING" ); 
            wepNameToKey.Add("Thunder Loop", "LOOP" ); 
            wepNameToKey.Add("Pentagram", "PENTAGRAM" ); 
            wepNameToKey.Add("Gorgeous Moon", "SIRE" ); 
            wepNameToKey.Add("Peachone", "SILF" ); 
            wepNameToKey.Add("Ebony Wings", "SILF2" ); 
            wepNameToKey.Add("Vandalier", "SILF3" ); 
            wepNameToKey.Add("Bone", "BONE" ); 
            wepNameToKey.Add("Clock Lancet", "LANCET" ); 
            wepNameToKey.Add("Song of Mana", "SONG" ); 
            wepNameToKey.Add("Mannajja", "MANNAGGIA" ); 
            wepNameToKey.Add("Cherry Bomb", "CHERRY" ); 
            wepNameToKey.Add("Dairy Cart", "CART" ); 
            wepNameToKey.Add("Carréllo", "CART2" ); 
            wepNameToKey.Add("Gatti Amari", "GATTI" ); 
            wepNameToKey.Add("Vicious Hunger", "STIGRANGATTI" ); 
            wepNameToKey.Add("Celestial Dusting", "FLOWER" ); 
            wepNameToKey.Add("Phiera Der Tuphello", "GUNS" ); 
            wepNameToKey.Add("Eight The Sparrow", "GUNS2" ); 
            wepNameToKey.Add("Phieraggi", "GUNS3" ); 
            wepNameToKey.Add("Shadow Pinion", "TRAPANO" ); 
            wepNameToKey.Add("Valkyrie Turner", "TRAPANO2" ); 
            wepNameToKey.Add("Sarabande of Healing", "SARABANDE" ); 
            wepNameToKey.Add("Heart of Fire", "FIREEXPLOSION" ); 
            wepNameToKey.Add("Nduja Fritta", "NDUJA" ); 
            wepNameToKey.Add("Spinach", "POWER" ); 
            wepNameToKey.Add("Candelabrador", "AREA" ); 
            wepNameToKey.Add("Bracer", "SPEED" ); 
            wepNameToKey.Add("Empty Tome", "COOLDOWN" ); 
            wepNameToKey.Add("Spellbinder", "DURATION" ); 
            wepNameToKey.Add("Duplicator", "AMOUNT" ); 
            wepNameToKey.Add("Hollow Heart", "MAXHEALTH" ); 
            wepNameToKey.Add("Armor", "ARMOR" ); 
            wepNameToKey.Add("Wings", "MOVESPEED" ); 
            wepNameToKey.Add("Attractorb", "MAGNET" ); 
            wepNameToKey.Add("Crown", "GROWTH" ); 
            wepNameToKey.Add("Clover", "LUCK" ); 
            wepNameToKey.Add("Stone Mask", "GREED" ); 
            wepNameToKey.Add("Tirajisú", "REVIVAL" ); 
            wepNameToKey.Add("Pummarola", "REGEN" ); 
            wepNameToKey.Add("Skull O'Maniac", "CURSE" ); 
            wepNameToKey.Add("Silver Ring", "SILVER" ); 
            wepNameToKey.Add("Gold Ring", "GOLD" ); 
            wepNameToKey.Add("NO FUTURE", "ROCHER" ); 
            wepNameToKey.Add("Metaglio Left", "LEFT" ); 
            wepNameToKey.Add("Metaglio Right", "RIGHT" ); 
            wepNameToKey.Add("Infinite Corridor", "CORRIDOR" ); 
            wepNameToKey.Add("Crimson Shroud", "SHROUD" ); 
            wepNameToKey.Add("Out of Bounds", "COLDEXPLOSION" ); 
            wepNameToKey.Add("Stained Glass", "WINDOW" ); 
            wepNameToKey.Add("Torrona's Box", "PANDORA" ); 
            wepNameToKey.Add("Vento Sacro", "VENTO" ); 
            wepNameToKey.Add("Fuwalafuwaloo", "VENTO2" ); 
            wepNameToKey.Add("La Robba", "ROBBA" ); 
            wepNameToKey.Add("Game Killer", "WINDOW2" ); 
            wepNameToKey.Add("Cygnus", "SILF_COUNTER" ); 
            wepNameToKey.Add("Zhar Ptytsia", "SILF2_COUNTER" ); 
            wepNameToKey.Add("Candybox", "CANDYBOX" ); 
            wepNameToKey.Add("Red Muscle", "GUNS_COUNTER" ); 
            wepNameToKey.Add("Twice Upon a Time", "GUNS2_COUNTER" ); 
            wepNameToKey.Add("Greatest Jubilee", "JUBILEE" ); 
            wepNameToKey.Add("Victory Sword", "VICTORY" ); 
            wepNameToKey.Add("Sole Solution", "SOLES" ); 
            wepNameToKey.Add("Flock Destroyer", "GATTI_COUNTER" ); 
            wepNameToKey.Add("Nduja Fritta Counter", "NDUJA_COUNTER" ); 
            wepNameToKey.Add("Bracelet", "TRIASSO1" ); 
            wepNameToKey.Add("Bi-Bracelet", "TRIASSO2" ); 
            wepNameToKey.Add("Tri-Bracelet", "TRIASSO3" ); 
            wepNameToKey.Add("Divine Bloodline", "BLOODLINE" ); 
            wepNameToKey.Add("Super Candybox II Turbo", "CANDYBOX2" ); 
            wepNameToKey.Add("Mille Bolle Blu", "BUBBLES" ); 
            wepNameToKey.Add("Blood Astronomia", "ASTRONOMIA" ); 
            wepNameToKey.Add("Flames of Misspell", "MISSPELL" ); 
            wepNameToKey.Add("Ashes of Muspell", "MISSPELL2" ); 
            wepNameToKey.Add("Silver Wind", "SILVERWIND" ); 
            wepNameToKey.Add("Festive Winds", "SILVERWIND2" ); 
            wepNameToKey.Add("Four Seasons", "FOURSEASONS" ); 
            wepNameToKey.Add("Godai Shuffle", "FOURSEASONS2" ); 
            wepNameToKey.Add("Summon Night", "SUMMONNIGHT" ); 
            wepNameToKey.Add("Echo Night", "SUMMONNIGHT2" ); 
            wepNameToKey.Add("Mirage Robe", "MIRAGEROBE" ); 
            wepNameToKey.Add("J'Odore", "MIRAGEROBE2" ); 
            wepNameToKey.Add("Boo Roo Boolle", "BUBBLES2" ); 
            wepNameToKey.Add("Night Sword", "NIGHTSWORD" ); 
            wepNameToKey.Add("Muramasa", "NIGHTSWORD2" ); 
            wepNameToKey.Add("108 Bocce", "BOCCE" ); 
            wepNameToKey.Add("SpellString", "SPELL_STRING" ); 
            wepNameToKey.Add("SpellStream", "SPELL_STREAM" ); 
            wepNameToKey.Add("SpellStrike", "SPELL_STRIKE" ); 
            wepNameToKey.Add("SpellStrom", "SPELL_STROM" ); 
            wepNameToKey.Add("Anima of Mortaccio", "BONE2" ); 
            wepNameToKey.Add("Tetraforce", "TETRAFORCE" ); 
            wepNameToKey.Add("Shadow Servant", "SHADOWSERVANT" ); 
            wepNameToKey.Add("Ophion", "SHADOWSERVANT2" ); 
            wepNameToKey.Add("Prismatic Missile", "PRISMATICMISS" ); 
            wepNameToKey.Add("Luminaire", "PRISMATICMISS2" ); 
            wepNameToKey.Add("Flash Arrow", "FLASHARROW" ); 
            wepNameToKey.Add("Millionaire", "FLASHARROW2" ); 
            wepNameToKey.Add("Eskizzibur", "SWORD" ); 
            wepNameToKey.Add("Legionnaire", "SWORD2" ); 
            wepNameToKey.Add("Party Popper", "PARTY" ); 
            wepNameToKey.Add("Phas3r", "PHASER" ); 
            wepNameToKey.Add("Silver Sliver", "SHADOWSERVANT_COUNTER" ); 
            wepNameToKey.Add("Party Pooper", "PARTY_COUNTER" ); 
            wepNameToKey.Add("Academy Badge", "ACADEMYBADGE" ); 
            wepNameToKey.Add("Insatiable", "INSATIABLE" ); 
            wepNameToKey.Add("Yatta Daikarin", "CHERRY2" ); 
            wepNameToKey.Add("Glass Fandango", "ICELANCE" ); 
            wepNameToKey.Add("Celestial Voulge", "ICELANCE2" ); 
            wepNameToKey.Add("Acchee Pee Kia", "CART2EVO" ); 
            wepNameToKey.Add("Profusione D'Amore", "FLOWER2" ); 
            wepNameToKey.Add("Gravijoe", "LAROBBA2" ); 
            wepNameToKey.Add("Arma Dio", "ARMADIO" ); 
            wepNameToKey.Add("Photonstorm", "PHASER2" ); 
            wepNameToKey.Add("Pako Battiliar", "BATTILIA" ); 
            wepNameToKey.Add("Mazo Familiar", "BATTILIA2" ); 
            wepNameToKey.Add("Report!", "C1_REPORT1" ); 
            wepNameToKey.Add("Emergency Meeting", "C1_REPORT2" ); 
            wepNameToKey.Add("Lucky Swipe", "C1_SWIPECARD1" ); 
            wepNameToKey.Add("Crossed Wires", "C1_SWIPECARD2" ); 
            wepNameToKey.Add("Lifesign Scan", "C1_MEDICAL1" ); 
            wepNameToKey.Add("Paranormal Scan", "C1_MEDICAL2" ); 
            wepNameToKey.Add("Just Vent", "C1_VENT1" ); 
            wepNameToKey.Add("Unjust Ejection", "C1_VENT2" ); 
            wepNameToKey.Add("Clear Debris", "C1_GARBA1" ); 
            wepNameToKey.Add("Clear Asteroids", "C1_GARBA2" ); 
            wepNameToKey.Add("Sharp Tongue", "C1_TONGUE1" ); 
            wepNameToKey.Add("Impostongue", "C1_TONGUE2" ); 
            wepNameToKey.Add("Science Rocks", "C1_SAMPLES1" ); 
            wepNameToKey.Add("Rocket Science", "C1_SAMPLES2" ); 
            wepNameToKey.Add("Hats", "C1_HATCOLLECTION1" ); 
            wepNameToKey.Add("Mini Crewmate", "C1_SHRINK_CREWMA" ); 
            wepNameToKey.Add("Mini Engineer", "C1_SHRINK_ENGINE" ); 
            wepNameToKey.Add("Mini Ghost", "C1_SHRINK_GHOSTT" ); 
            wepNameToKey.Add("Mini Shapeshifter", "C1_SHRINK_SHAPES" ); 
            wepNameToKey.Add("Mini Guardian", "C1_SHRINK_GUARDI" ); 
            wepNameToKey.Add("Mini Impostor", "C1_SHRINK_IMPOST" ); 
            wepNameToKey.Add("Mini Scientist", "C1_SHRINK_SCIENT" ); 
            wepNameToKey.Add("Mini Horse", "C1_SHRINK_HORSEE" ); 
            wepNameToKey.Add("Silver Tongue", "C1_TONGUE1_COUNTER" ); 
            wepNameToKey.Add("Parm Aegis", "PARMA" ); 
            wepNameToKey.Add("Karoma's Mana", "DOMINION" ); 
            wepNameToKey.Add("Fire Nova", "NOVA_FIRE" ); 
            wepNameToKey.Add("Ice Nova", "NOVA_ICEE" ); 
            wepNameToKey.Add("Fear Nova", "NOVA_FEAR" ); 
            wepNameToKey.Add("Flik the Blue", "FLIK" ); 
            wepNameToKey.Add("Blue Impulse", "FLIK2" ); 
            wepNameToKey.Add("Take Us Away", "TRAINHAZARD" ); 
            wepNameToKey.Add("Santa Javelin", "SANTAJAVELIN" ); 
            wepNameToKey.Add("Seraphic Cry", "SANTAJAVELIN2" ); 
            wepNameToKey.Add("Levelin'Eh", "SANTAJAVELINCOUNTER" ); 
            wepNameToKey.Add("Sorbetto", "CONEOFCOLD" ); 
            wepNameToKey.Add("Acquazzone", "FLOOD" ); 
            wepNameToKey.Add("Long Gun ", "FB_FULLAUTO" ); 
            wepNameToKey.Add("Short Gun ", "FB_RAPIDFIRE" ); 
            wepNameToKey.Add("Spread Shot ", "FB_SPREAD" ); 
            wepNameToKey.Add("Sonic Bloom", "FB_SONIC" ); 
            wepNameToKey.Add("Blade Crossbow", "FB_BLADECROSSBOW" ); 
            wepNameToKey.Add("Wave Beam", "FB_WAVE" ); 
            wepNameToKey.Add("C-U-Laser", "FB_LASER" ); 
            wepNameToKey.Add("Firearm", "FB_FIREARM" ); 
            wepNameToKey.Add("Metal Claw", "FB_METALCLAW" ); 
            wepNameToKey.Add("Atmo-Torpedo", "FB_CRUSH" ); 
            wepNameToKey.Add("Homing Miss", "FB_HOMING" ); 
            wepNameToKey.Add("Prism Lass", "FB_PRISMCUTLASS" ); 
            wepNameToKey.Add("Pronto Beam", "FB_PROTONBEAM" ); 
            wepNameToKey.Add("Fire-L3GS", "FB_FIREWALL" ); 
            wepNameToKey.Add("Big Fuzzy Fist", "FB_BIGFUZZYFIST" ); 
            wepNameToKey.Add("Multistage Missiles", "FB_MULTISTAGE" ); 
            wepNameToKey.Add("Time Warp", "FB_TIMEWARP" ); 
            wepNameToKey.Add("Weapon Power-Up", "FB_WEAPONPU" ); 
            wepNameToKey.Add("Prototype B", "FB_PROTOTYPE_B" ); 
            wepNameToKey.Add("Prototype A", "FB_PROTOTYPE_A" ); 
            wepNameToKey.Add("Prototype C", "FB_PROTOTYPE_C" ); 
            wepNameToKey.Add("BFC2000-AD", "FB_CROSSBOWCRASH" ); 
            wepNameToKey.Add("Diver Mines", "FB_DIVERMINES" ); 
            wepNameToKey.Add("Prism Damsel", "FB_PRISMCUTLASS_COUNTER" ); 
            wepNameToKey.Add("Wandering the Jet Black", "D20_JETBLACK" ); 
            wepNameToKey.Add("Confodere", "TP_CONFODERE1" ); 
            wepNameToKey.Add("Vol Confodere", "TP_CONFODERE2" ); 
            wepNameToKey.Add("Melio Confodere", "TP_CONFODERE3" ); 
            wepNameToKey.Add("Alchemy Whip", "TP_ALCHEMYWHIP1" ); 
            wepNameToKey.Add("Vampire Killer", "TP_ALCHEMYWHIP2" ); 
            wepNameToKey.Add("Coat of Arms", "TP_COATOFARMS" ); 
            wepNameToKey.Add("Morning Star", "TP_MORNINGSTAR" ); 
            wepNameToKey.Add("Belnades' Spellbook", "TP_SPELLBOOK" ); 
            wepNameToKey.Add("Ebony Diabologue", "TP_DIABOLOGUE" ); 
            wepNameToKey.Add("Alucart Sworb", "TP_ALUCARDSWORD1" ); 
            wepNameToKey.Add("Alucard Swords", "TP_ALUCARDSWORD2" ); 
            wepNameToKey.Add("Iron Ball", "TP_IRONBALL1" ); 
            wepNameToKey.Add("Wrecking Ball", "TP_IRONBALL2" ); 
            wepNameToKey.Add("Alucard Spear", "TP_ALUCARDSPEAR1" ); 
            wepNameToKey.Add("Thunderbolt Spear", "TP_ALUCARDSPEAR2" ); 
            wepNameToKey.Add("Mace", "TP_MACE1" ); 
            wepNameToKey.Add("Stamazza", "TP_MACE2" ); 
            wepNameToKey.Add("Trident", "TP_CHAUVE1" ); 
            wepNameToKey.Add("Gungnir-Souris", "TP_CHAUVE2" ); 
            wepNameToKey.Add("Javelin", "TP_JAVELIN1" ); 
            wepNameToKey.Add("Long Inus", "TP_JAVELIN2" ); 
            wepNameToKey.Add("Curved Knife", "TP_BWAKA1" ); 
            wepNameToKey.Add("Bwaka Knife", "TP_BWAKA2" ); 
            wepNameToKey.Add("Shuriken", "TP_SHURIKEN1" ); 
            wepNameToKey.Add("Yagyu Shuriken", "TP_SHURIKEN2" ); 
            wepNameToKey.Add("Luminatio", "TP_LIGHT1" ); 
            wepNameToKey.Add("Vol Luminatio", "TP_LIGHT2" ); 
            wepNameToKey.Add("Umbra", "TP_DARK1" ); 
            wepNameToKey.Add("Vol Umbra", "TP_DARK2" ); 
            wepNameToKey.Add("Discus", "TP_DISCUS1" ); 
            wepNameToKey.Add("Stellar Blade", "TP_DISCUS2" ); 
            wepNameToKey.Add("Wine Glass", "TP_WINEGLASS1" ); 
            wepNameToKey.Add("Meal Ticket", "TP_WINEGLASS2" ); 
            wepNameToKey.Add("Vanitas Whip", "TP_MARTIALWHIP1" ); 
            wepNameToKey.Add("Aurablaster Tip", "TP_MARTIALWHIP2" ); 
            wepNameToKey.Add("Dextro Custos", "TP_CUSTOS1" ); 
            wepNameToKey.Add("Sinestro Custos", "TP_CUSTOS2" ); 
            wepNameToKey.Add("Centralis Custos", "TP_CUSTOS3" ); 
            wepNameToKey.Add("Trinum Custodem", "TP_CUSTOS4" ); 
            wepNameToKey.Add("Wind Whip", "TP_WINDWHIP1" ); 
            wepNameToKey.Add("Spirit Tornado Tip", "TP_WINDWHIP2" ); 
            wepNameToKey.Add("Platinum Whip", "TP_PLATINUMWHIP1" ); 
            wepNameToKey.Add("Cross Crasher Tip", "TP_PLATINUMWHIP2" ); 
            wepNameToKey.Add("Dragon Water Whip", "TP_DRAGONWATER1" ); 
            wepNameToKey.Add("Hydrostormer Tip", "TP_DRAGONWATER2" ); 
            wepNameToKey.Add("Soul Steal", "TP_SOULSTEAL_WEAPON" ); 
            wepNameToKey.Add("Hand Grenade", "TP_RPG1" ); 
            wepNameToKey.Add("The RPG", "TP_RPG2" ); 
            wepNameToKey.Add("Alucard Shield", "TP_ALUCARDSHIELD" ); 
            wepNameToKey.Add("Sonic Dash", "TP_RAPIDUS1" ); 
            wepNameToKey.Add("Rapidus Fio", "TP_RAPIDUS2" ); 
            wepNameToKey.Add("Raging Fire", "TP_FIRE1" ); 
            wepNameToKey.Add("Salamender", "TP_FIRE2" ); 
            wepNameToKey.Add("Ice Fang", "TP_ICE1" ); 
            wepNameToKey.Add("Cocytus", "TP_ICE2" ); 
            wepNameToKey.Add("Gale Force", "TP_WIND1" ); 
            wepNameToKey.Add("Pneuma Tempestas", "TP_WIND2" ); 
            wepNameToKey.Add("Rock Riot", "TP_EARTH1" ); 
            wepNameToKey.Add("Gemma Torpor", "TP_EARTH2" ); 
            wepNameToKey.Add("Fulgur", "TP_ELEC1" ); 
            wepNameToKey.Add("Tenebris Tonitrus", "TP_ELEC2" ); 
            wepNameToKey.Add("Keremet Bubbles", "TP_ACID1" ); 
            wepNameToKey.Add("Keremet Morbus", "TP_ACID2" ); 
            wepNameToKey.Add("Hex", "TP_EVIL1" ); 
            wepNameToKey.Add("Nightmare", "TP_EVIL2" ); 
            wepNameToKey.Add("Refectio", "TP_HOLY1" ); 
            wepNameToKey.Add("Sanctuary", "TP_HOLY2" ); 
            wepNameToKey.Add("Optical Shot", "TP_SPITE1" ); 
            wepNameToKey.Add("Acerbatus", "TP_SPITE2" ); 
            wepNameToKey.Add("Globus", "TP_ENERGY1" ); 
            wepNameToKey.Add("Nitesco", "TP_ENERGY2" ); 
            wepNameToKey.Add("Speculo Raging Fire", "TP_FIRE1_COUNTER" ); 
            wepNameToKey.Add("Speculo Ice Fang", "TP_ICE1_COUNTER" ); 
            wepNameToKey.Add("Speculo Rock Riot", "TP_EARTH1_COUNTER" ); 
            wepNameToKey.Add("Speculo Gale Force", "TP_WIND1_COUNTER" ); 
            wepNameToKey.Add("Speculo Fulgur", "TP_ELEC1_COUNTER" ); 
            wepNameToKey.Add("Speculo Keremet Bubbles", "TP_ACID1_COUNTER" ); 
            wepNameToKey.Add("Speculo Hex", "TP_EVIL1_COUNTER" ); 
            wepNameToKey.Add("Speculo Refectio", "TP_HOLY1_COUNTER" ); 
            wepNameToKey.Add("Speculo Globus", "TP_ENERGY1_COUNTER" ); 
            wepNameToKey.Add("Crissaegrim Tip", "TP_SONICWHIP2" ); 
            wepNameToKey.Add("Vibhuti Whip", "TP_HOLYWHIP1" ); 
            wepNameToKey.Add("Daybreaker Tip", "TP_HOLYWHIP2" ); 
            wepNameToKey.Add("Silver Revolver", "TP_GUN1" ); 
            wepNameToKey.Add("Jewel Gun", "TP_GUN2" ); 
            wepNameToKey.Add("Tyrfing", "TP_SLASH1" ); 
            wepNameToKey.Add("Rune Sword", "TP_SLASH2" ); 
            wepNameToKey.Add("Universitas", "TP_UNIVERSITAS" ); 
            wepNameToKey.Add("Dominus Anger", "TP_DOMINUS1" ); 
            wepNameToKey.Add("Dominus Hatred", "TP_DOMINUS2" ); 
            wepNameToKey.Add("Dominus Agony", "TP_DOMINUS3" ); 
            wepNameToKey.Add("Power of Sire", "TP_DOMINUS4" ); 
            wepNameToKey.Add("Guardian's Targe", "TP_SACREDBEASTS1" ); 
            wepNameToKey.Add("Sacred Beasts Tower Shield", "TP_SACREDBEASTS2" ); 
            wepNameToKey.Add("Star Flail", "TP_STARFLAIL1" ); 
            wepNameToKey.Add("Moon Rod", "TP_STARFLAIL2" ); 
            wepNameToKey.Add("Jet Black Whip", "TP_LEMURIA1" ); 
            wepNameToKey.Add("Mormegil Tip", "TP_LEMURIA2" ); 
            wepNameToKey.Add("Spectral Sword", "TP_SPECTRALSWORD" ); 
            wepNameToKey.Add("Iron Shield", "TP_SHIELD1" ); 
            wepNameToKey.Add("Dark Iron Shield", "TP_SHIELD2" ); 
            wepNameToKey.Add("Neutron Bomb", "TP_NEUTRON_PICKUP" ); 
            wepNameToKey.Add("Troll Bomb", "TP_NEUTRON_WEAPON" ); 
            wepNameToKey.Add("Soul Steal Counter", "TP_SOULSTEAL_PICKUP" ); 
            wepNameToKey.Add("Endo Gears", "TP_GEARS_WEAPON" ); 
            wepNameToKey.Add("Peri Pendulum", "TP_PENDULUM_WEAPON" ); 
            wepNameToKey.Add("Svarog Statue", "TP_SAVROG_WEAPON" ); 
            wepNameToKey.Add("Sonic Whip", "TP_SONICWHIP1" ); 
            wepNameToKey.Add("Arrow of Goth", "TP_GOTH_MISSILE" ); 
            wepNameToKey.Add("Arrow of Goth Upgrade", "TP_GOTH_MISSILE_HOLYWHIP2" ); 
            wepNameToKey.Add("Hydro Storm", "TP_HYDROSTORM" ); 
            wepNameToKey.Add("Hydro Storm Whip", "TP_HYDROSTORM_WATERDRAGONWHIP" ); 
            wepNameToKey.Add("Dark Rift", "TP_DARKRIFT" ); 
            wepNameToKey.Add("Dark Rift Whip", "TP_DARKRIFT_JETBLACKWHIP" ); 
            wepNameToKey.Add("Summon Spirit", "TP_SUMMON_SPIRIT" ); 
            wepNameToKey.Add("Sword Brothers", "TP_SWORD_BROTHERS" ); 
            wepNameToKey.Add("Valmanway", "TP_VALMANWAY" ); 
            wepNameToKey.Add("Valmanway Whip", "TP_VALMANWAY_SONICWHIP" ); 
            wepNameToKey.Add("Grand Cross", "TP_GRANDCROSS" ); 
            wepNameToKey.Add("Grand Cross Whip", "TP_GRANDCROSS_PLATINUMWHIP" ); 
            wepNameToKey.Add("Summon Spirit Tornado", "TP_SPIRITTORNADO" ); 
            wepNameToKey.Add("Spirit Tornado", "TP_SPIRITTORNADO_WINDWHIP" ); 
            wepNameToKey.Add("Myo Lift", "TP_ELEVATOR_WEAPON" ); 
            wepNameToKey.Add("Epi Head", "TP_HEADS_WEAPON" ); 
            wepNameToKey.Add("Clock Tower", "TP_CLOCKTOWER_WEAPON" ); 
            wepNameToKey.Add("Aura Blast", "TP_AURABLAST_WEAPON" ); 
            wepNameToKey.Add("Burning Alcarde", "TP_BLUEFIRE_WEAPON" ); 
            wepNameToKey.Add("Aura Blast Whip", "TP_AURABLAST_MARTIALWHIP" ); 
            wepNameToKey.Add("Ukoback", "TP_ACC_FAMILIAR_UKOBACK" ); 
            wepNameToKey.Add("Bitterfly", "TP_ACC_FAMILIAR_BITTERFLY" ); 
            wepNameToKey.Add("Imp", "TP_ACC_FAMILIAR_IMP" ); 
            wepNameToKey.Add("Alleged Ghost", "TP_ACC_FAMILIAR_ALLEGEDGHOST" ); 
            wepNameToKey.Add("Wood Rod", "TP_ACC_FAMILIAR_WIZARD" ); 
            wepNameToKey.Add("Faerie", "TP_ACC_FAMILIAR_FAIRY" ); 
            wepNameToKey.Add("Pumpkin", "TP_ACC_FAMILIAR_PUMPKIN" ); 
            wepNameToKey.Add("Familiar Forge", "TP_FAMILIARFORGE" ); 
            wepNameToKey.Add("Sacred Cardinal", "TP_ACC_FAMILIAR_CARDINAL" ); 
            wepNameToKey.Add("Sacred Dragon", "TP_ACC_FAMILIAR_DRAGON" ); 
            wepNameToKey.Add("Sacred Tiger", "TP_ACC_FAMILIAR_TIGER" ); 
            wepNameToKey.Add("Sacred Turtle", "TP_ACC_FAMILIAR_TURTLE" ); 
            wepNameToKey.Add("Icebrand", "TP_ICEBRAND" ); 
            wepNameToKey.Add("Death Hand", "TP_DEATHHAND" ); 
            wepNameToKey.Add("Hand of Vlad", "TP_DRACULAHAND" );
            wepNameToKey.Add("Power Of Lire", "TP_POWEROFLIRE" );
        }
    }

    public class EmptyJson
    {
        [JsonProperty("version")]
        public string Version { get; } = "0.3";

        [JsonProperty("characters")]
        public List<CharacterObject> Characters { get; set; } = new List<CharacterObject>();

        [JsonProperty("spriteData")]
        public List<SpriteObject> Sprites { get; set; } = new List<SpriteObject>();
    }
    public class StatModifier
    {
        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("area")]
        public float Area { get; set; }

        [JsonProperty("armor")]
        public float Armor { get; set; }

        [JsonProperty("banish")]
        public float Banish { get; set; }

        [JsonProperty("charm")]
        public int Charm { get; set; }

        [JsonProperty("cooldown")]
        public float Cooldown { get; set; }

        [JsonProperty("curse")]
        public float Curse { get; set; }

        [JsonProperty("defang")]
        public float Defang { get; set; }

        [JsonProperty("duration")]
        public float Duration { get; set; }

        [JsonProperty("fever")]
        public float Fever { get; set; }

        [JsonProperty("greed")]
        public float Greed { get; set; }

        [JsonProperty("growth")]
        public float Growth { get; set; }

        [JsonProperty("invulTimeBonus")]
        public float Invul { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("luck")]
        public float Luck { get; set; }

        [JsonProperty("magnet")]
        public float Magnet { get; set; }

        [JsonProperty("maxHp")]
        public float MaxHp { get; set; }

        [JsonProperty("moveSpeed")]
        public float MoveSpeed { get; set; }

        [JsonProperty("power")]
        public float Power { get; set; }

        [JsonProperty("regen")]
        public float Regen { get; set; }

        [JsonProperty("rerolls")]
        public float Rerolls { get; set; }

        [JsonProperty("revivals")]
        public float Revivals { get; set; }

        [JsonProperty("shields")]
        public float Shields { get; set; }

        /*[JsonProperty("sineArea")]
        public string SineArea { get; set; }

        [JsonProperty("sineCooldown")]
        public string SineCoolDown { get; set; }

        [JsonProperty("sineDuration")]
        public string SineDuration { get; set; }

        [JsonProperty("sineMight")]
        public string SineMight { get; set; }

        [JsonProperty("sineSpeed")]
        public string SineSpeed { get; set; }*/

        [JsonProperty("shroud")]
        public float Shroud { get; set; }

        [JsonProperty("skips")]
        public float Skips { get; set; }

        [JsonProperty("speed")]
        public float Speed { get; set; }

        public StatModifier Cleaner()
        {
            StatModifier cleanedModifier = new StatModifier();
            
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                var value = prop.GetValue(this, null);
                if (value != null && (value.ToString() != "0" || value.ToString() != "0.0")) 
                    cleanedModifier.GetType().GetProperty(prop.Name).SetValue(cleanedModifier, value);
            }
            return cleanedModifier;
        }
    }
    public class CharacterObject
    {
        [JsonProperty("alwaysHidden")]
        public bool AlwaysHidden { get; set; }

        [JsonProperty("charName")]
        public string CharName { get; set; }

        [JsonProperty("currentSkin")]
        public string CurrentSkin { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("exLevels")]
        public int ExLevels { get; set; }

        [JsonProperty("exWeapons")]
        public List<string> ExWeapons { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("hiddenWeapons")]
        public List<string> HiddenWeapons { get; set; }

        [JsonProperty("isBought")]
        public bool IsBought { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; } = 1;

        [JsonProperty("onEveryLevelUp")]
        public StatModifier OnEveryLevelUp { get; set; }

        [JsonProperty("portraitName")]
        public string PortraitName {  get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("showcase")]
        public List<string> Showcase { get; set; }

        [JsonProperty("skins")]
        public List<SkinObject> Skins { get; set; } = new List<SkinObject>();

        [JsonProperty("startingWeapon")]
        public string StartingWeapon {  get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("walkingFrames")]
        public int WalkingFrames { get; set; }

        [JsonProperty("statModifiers")]
        public List<StatModifier> StatModifiers { get; set; } = new List<StatModifier>();

        public bool ShouldSerializeExWeapons()
        {
            return ExWeapons.Count > 0;
        }
        public bool ShouldSerializeHiddenWeapons()
        {
            return HiddenWeapons.Count > 0;
        }
        public bool ShouldSerializeShowcase()
        {
            return Showcase.Count > 0;
        }
        public CharacterObject Cleaner()
        {
            CharacterObject cleanedObject = new CharacterObject();

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                var value = prop.GetValue(this, null);
                if (value != null && (value.ToString() != "0" || value.ToString() != "0.0" || value.ToString() != ""))
                   cleanedObject.GetType().GetProperty(prop.Name).SetValue(cleanedObject, value);
            }
            return cleanedObject;
        }
    }
    public class SkinObject
    {
        [JsonProperty("alwaysAnimated")]
        public bool AlwaysAnimated { get; set; }

        [JsonProperty("alwaysHidden")]
        public bool AlwaysHidden { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("area")]
        public float Area { get; set; }

        [JsonProperty("armor")]
        public float Armor { get; set; }

        [JsonProperty("banish")]
        public float Banish { get; set; }

        [JsonProperty("charm")]
        public float Charm { get; set; }

        [JsonProperty("cooldown")]
        public float Cooldown { get; set; }

        [JsonProperty("curse")]
        public float Curse { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("duration")]
        public float Duration { get; set; }

        [JsonProperty("exAccessories")]
        public List<string> ExAccessories { get; set; }

        [JsonProperty("exWeapons")]
        public List<string> ExWeapons { get; set; }

        [JsonProperty("greed")]
        public float Greed { get; set; }

        [JsonProperty("growth")]
        public float Growth { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("hiddenWeapons")]
        public List<string> HiddenWeapons { get; set; }

        [JsonProperty("luck")]
        public float Luck { get; set; }

        [JsonProperty("magnet")]
        public float Magnet { get; set; }

        [JsonProperty("maxHp")]
        public float MaxHp { get; set; }

        [JsonProperty("moveSpeed")]
        public float MoveSpeed { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("onEveryLevelUp")]
        public StatModifier OnEveryLevelUp { get; set; }

        [JsonProperty("power")]
        public float Power { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }

        [JsonProperty("regen")]
        public float Regen { get; set; }

        [JsonProperty("rerolls")]
        public float Rerolls { get; set; }

        [JsonProperty("revivals")]
        public float Revivals { get; set; }

        [JsonProperty("secret")]
        public bool Secret { get; set; }

        [JsonProperty("shields")]
        public float Shields { get; set; }

        [JsonProperty("skinType")]
        public string SkinType { get; set; }

        [JsonProperty("skips")]
        public float Skips { get; set; }

        [JsonProperty("speed")]
        public float Speed { get; set; }

        [JsonProperty("spriteName")]
        public string SpriteName { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        [JsonProperty("textureName")]
        public string TextureName { get; set; }

        [JsonProperty("unlocked")]
        public bool Unlocked { get; set; }

        [JsonProperty("walkingFrames")]
        public int WalkingFrames { get; set; } = 4;

        public bool ShouldSerializeExAccessories()
        {
            return ExAccessories.Count > 0;
        }
        public bool ShouldSerializeExWeapons()
        {
            return ExWeapons.Count > 0;
        }
        public bool ShouldSerializeHiddenWeapons()
        {
            return HiddenWeapons.Count > 0;
        }

        public SkinObject Cleaner()
        {
            SkinObject cleanedObject = new SkinObject();

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                var value = prop.GetValue(this, null);
                if (value != null && (value.ToString() != "0" || value.ToString() != "0.0" || value.ToString() != ""))
                    cleanedObject.GetType().GetProperty(prop.Name).SetValue(cleanedObject, value);
            }
            return cleanedObject;
        }
    }
    public class SpriteObject
    {
        [JsonProperty("rect")]
        public SpriteRect Rect {  get; set; }

        [JsonProperty("pivot")]
        public SpritePivot Pivot { get; set; }

        [JsonProperty("spriteName")]
        public string SpriteName { get; set; }

        [JsonProperty("textureName")]
        public string TextureName { get; set; }
    }
    public class SpriteRect
    {
        [JsonProperty("x")]
        [DefaultValue(-1)]
        public int X { get; set; }

        [JsonProperty("y")]
        [DefaultValue(-1)]
        public int Y { get; set; }
        [JsonProperty("width")]
        [DefaultValue(-1)]
        public int Width { get; set; }

        [JsonProperty("height")]
        [DefaultValue(-1)]
        public int Height { get; set; }
    }
    public class SpritePivot
    {
        [JsonProperty("x")]
        [DefaultValue(-1)]
        public int X { get; set; }

        [JsonProperty("y")]
        [DefaultValue(-1)]
        public int Y { get; set; }
    }

}
