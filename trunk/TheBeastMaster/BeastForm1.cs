﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.CommonBot;
using Styx.Combat;
using Styx.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System.Windows.Media;
using Styx.Common;

namespace TheBeastMaster
{
    public partial class BeastForm1 : Form
    {
        public BeastForm1()
        {
            InitializeComponent();
        }

        private void FillFDCBox()
        {
            FDBox.Items.Clear();
            FDBox.Items.Add("Never");
            FDBox.Items.Add("1. High Threat");
            FDBox.Items.Add("2. On Aggro");
            FDBox.Items.Add("3. Low Health");
            FDBox.Items.Add("1 + 3");
            FDBox.Items.Add("2 + 3");

            SerBox.Items.Clear();
            SerBox.Items.Add("Never");
            SerBox.Items.Add("Always");
            SerBox.Items.Add("Sometimes");

            ScatBox.Items.Clear();
            ScatBox.Items.Add("Never");
            ScatBox.Items.Add("1. Interrupt");
            ScatBox.Items.Add("2. Defense");
            ScatBox.Items.Add("1 + 2");

            IntiBox.Items.Clear();
            IntiBox.Items.Add("Never");
            IntiBox.Items.Add("1. Interrupt");
            IntiBox.Items.Add("2. Defense");
            IntiBox.Items.Add("1 + 2");
        }

        private void IntiBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)IntiBox.SelectedItem)
            {
                case "Never":
                    BeastMasterSettings.Instance.IntimidateBox = (string)IntiBox.SelectedItem;
                    break;
                case "1. Interrupt":
                    BeastMasterSettings.Instance.IntimidateBox = (string)IntiBox.SelectedItem;
                    break;
                case "2. Defense":
                    BeastMasterSettings.Instance.IntimidateBox = (string)IntiBox.SelectedItem;
                    break;
                case "1 + 2":
                    BeastMasterSettings.Instance.IntimidateBox = (string)IntiBox.SelectedItem;
                    break;
            }
        }

        private void FDBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)FDBox.SelectedItem)
            {
                case "Never":
                    BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
                    break;
                case "1. High Threat":
                    BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
                    break;
                case "2. On Aggro":
                    BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
                    break;
                case "3. Low Health":
                    BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
                    break;
                case "1 + 3":
                    BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
                    break;
                case "2 + 3":
                    BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
                    break;
            }
        }

        private void SerBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)SerBox.SelectedItem)
            {
                case "Never":
                    BeastMasterSettings.Instance.SerpentBox = (string)SerBox.SelectedItem;
                    break;
                case "Always":
                    BeastMasterSettings.Instance.SerpentBox = (string)SerBox.SelectedItem;
                    break;
                case "Sometimes":
                    BeastMasterSettings.Instance.SerpentBox = (string)SerBox.SelectedItem;
                    break;
            }
        }
        private void ScatBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)ScatBox.SelectedItem)
            {
                case "Never":
                    BeastMasterSettings.Instance.ScatterBox = (string)ScatBox.SelectedItem;
                    break;
                case "1. Interrupt":
                    BeastMasterSettings.Instance.ScatterBox = (string)ScatBox.SelectedItem;
                    break;
                case "2. Defense":
                    BeastMasterSettings.Instance.ScatterBox = (string)ScatBox.SelectedItem;
                    break;
                case "1 + 2":
                    BeastMasterSettings.Instance.ScatterBox = (string)ScatBox.SelectedItem;
                    break;
            }
        }
        private void BeastForm1_Load(object sender, EventArgs e)
        {
            FillFDCBox();
            FDBox.SelectedItem = (string)BeastMasterSettings.Instance.FDCBox;
            IntiBox.SelectedItem = (string)BeastMasterSettings.Instance.IntimidateBox;
            SerBox.SelectedItem = (string)BeastMasterSettings.Instance.SerpentBox;
            ScatBox.SelectedItem = (string)BeastMasterSettings.Instance.ScatterBox;

            BeastMasterSettings.Instance.Load();

            Mobs.Value = new decimal(BeastMasterSettings.Instance.Mobs);
            PET.Value = new decimal(BeastMasterSettings.Instance.PET);
            FFS.Value = new decimal(BeastMasterSettings.Instance.FFS);
            MendHealth.Value = new decimal(BeastMasterSettings.Instance.MendHealth);
            FocusShots.Value = new decimal(BeastMasterSettings.Instance.FocusShots);
            Party.Checked = BeastMasterSettings.Instance.Party;
            DSHC.Checked = BeastMasterSettings.Instance.DSHC;
            DSLFR.Checked = BeastMasterSettings.Instance.DSLFR;
            DSNOR.Checked = BeastMasterSettings.Instance.DSNOR;
            TL1_None.Checked = BeastMasterSettings.Instance.TL1_NO;
            TL2_None.Checked = BeastMasterSettings.Instance.TL2_NO;
            TL3_None.Checked = BeastMasterSettings.Instance.TL3_NO;
            TL1_Silence.Checked = BeastMasterSettings.Instance.TL1_SS;
            TL1_Wyvern.Checked = BeastMasterSettings.Instance.TL1_WS;
            TL1_Binding.Checked = BeastMasterSettings.Instance.TL1_BS;
            TL2_Fervor.Checked = BeastMasterSettings.Instance.TL2_FV;
            TL2_Dire.Checked = BeastMasterSettings.Instance.TL2_DB;
            TL2_Thrill.Checked = BeastMasterSettings.Instance.TL2_TOTH;
            TL3_Crows.Checked = BeastMasterSettings.Instance.TL3_AMOC;
            TL3_Blink.Checked = BeastMasterSettings.Instance.TL3_BSTRK;
            TL3_Lynx.Checked = BeastMasterSettings.Instance.TL3_LR;
            CallPet.Checked = BeastMasterSettings.Instance.CP;
            FocusFire.Checked = BeastMasterSettings.Instance.FF;
            CallWild.Checked = BeastMasterSettings.Instance.CW;
            RevivePet.Checked = BeastMasterSettings.Instance.RP;
            MendPet.Checked = BeastMasterSettings.Instance.MP;
            MultiShot.Checked = BeastMasterSettings.Instance.MS;
            Launcher.Checked = BeastMasterSettings.Instance.TL;
			AoELynx.Checked = BeastMasterSettings.Instance.AOELR;
            AoEDire.Checked = BeastMasterSettings.Instance.AOEDB;
            Trinket1.Checked = BeastMasterSettings.Instance.T1;
            Trinket2.Checked = BeastMasterSettings.Instance.T2;
            Rapid.Checked = BeastMasterSettings.Instance.RF;
            LifeBlood.Checked = BeastMasterSettings.Instance.LB;
            Gloves.Checked = BeastMasterSettings.Instance.GE;
            Racial.Checked = BeastMasterSettings.Instance.RS;
            AspectSwitching.Checked = BeastMasterSettings.Instance.AspectSwitching;
			DBeast.Checked = BeastMasterSettings.Instance.DB;
			Glaives.Checked = BeastMasterSettings.Instance.GLV;
			Silence.Checked = BeastMasterSettings.Instance.SLS;
			Binding.Checked = BeastMasterSettings.Instance.BDS;
			HMark.Checked = BeastMasterSettings.Instance.HM;
			Concussive.Checked = BeastMasterSettings.Instance.CONC;
			LynxRush.Checked = BeastMasterSettings.Instance.LXR;
			Deterrence.Checked = BeastMasterSettings.Instance.DETR;
			BWrath.Checked = BeastMasterSettings.Instance.BWR;
            SpiritMend.Checked = BeastMasterSettings.Instance.SMend;
            Burrow.Checked = BeastMasterSettings.Instance.BRA;
            Misdirect.Checked = BeastMasterSettings.Instance.MDPet;      
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/honorbuddy-forum/classes/all-one/42174-lazyraider-all-one-pve-ccs.html");
            linkLabel1.LinkVisited = true;
        }
        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.thebuddyforum.com/newreply.php?t=51333&noquote=1");
            linkLabel5.LinkVisited = true;
        }
        private void Mobs_ValueChanged(object sender, EventArgs e)
        {
            BeastMasterSettings.Instance.Mobs = (int)Mobs.Value;
        }
        private void PET_ValueChanged(object sender, EventArgs e)
        {
            BeastMasterSettings.Instance.PET = (int)PET.Value;
        }
        private void FFS_ValueChanged(object sender, EventArgs e)
        {
            BeastMasterSettings.Instance.FFS = (int)FFS.Value;
        }
        private void FocusShots_ValueChanged(object sender, EventArgs e)
        {
            BeastMasterSettings.Instance.FocusShots = (int)FocusShots.Value;
        }
        private void MendHealth_ValueChanged(object sender, EventArgs e)
        {
            BeastMasterSettings.Instance.MendHealth = (int)MendHealth.Value;
        }
        private void Party_CheckedChanged(object sender, EventArgs e)
        {
            if (Party.Checked == true)
            {
                BeastMasterSettings.Instance.Party = true;
            }
            else
            {
                BeastMasterSettings.Instance.Party = false;
            }
        }
        private void DSHC_CheckedChanged(object sender, EventArgs e)
        {
            if (DSHC.Checked == true)
            {
                BeastMasterSettings.Instance.DSHC = true;
            }
            else
            {
                BeastMasterSettings.Instance.DSHC = false;
            }
        }
        private void DSLFR_CheckedChanged(object sender, EventArgs e)
        {
            if (DSLFR.Checked == true)
            {
                BeastMasterSettings.Instance.DSLFR = true;
            }
            else
            {
                BeastMasterSettings.Instance.DSLFR = false;
            }
        }
        private void DSNOR_CheckedChanged(object sender, EventArgs e)
        {
            if (DSNOR.Checked == true)
            {
                BeastMasterSettings.Instance.DSNOR = true;
            }
            else
            {
                BeastMasterSettings.Instance.DSNOR = false;
            }
        }
        private void TL1_None_CheckedChanged(object sender, EventArgs e)
        {
            if (TL1_None.Checked == true)
            {
                BeastMasterSettings.Instance.TL1_NO = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL1_NO = false;
            }
        }
        private void TL2_None_CheckedChanged(object sender, EventArgs e)
        {
            if (TL2_None.Checked == true)
            {
                BeastMasterSettings.Instance.TL2_NO = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL2_NO = false;
            }
        }
        private void TL3_None_CheckedChanged(object sender, EventArgs e)
        {
            if (TL3_None.Checked == true)
            {
                BeastMasterSettings.Instance.TL3_NO = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL3_NO = false;
            }
        }
        private void TL1_Silence_CheckedChanged(object sender, EventArgs e)
        {
            if (TL1_Silence.Checked == true)
            {
                BeastMasterSettings.Instance.TL1_SS = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL1_SS = false;
            }
        }
        private void TL1_Wyvern_CheckedChanged(object sender, EventArgs e)
        {
            if (TL1_Wyvern.Checked == true)
            {
                BeastMasterSettings.Instance.TL1_WS = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL1_WS = false;
            }
        }
        private void TL1_Binding_CheckedChanged(object sender, EventArgs e)
        {
            if (TL1_Binding.Checked == true)
            {
                BeastMasterSettings.Instance.TL1_BS = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL1_BS = false;
            }
        }
        private void TL2_Fervor_CheckedChanged(object sender, EventArgs e)
        {
            if (TL2_Fervor.Checked == true)
            {
                BeastMasterSettings.Instance.TL2_FV = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL2_FV = false;
            }
        }
        private void TL2_Dire_CheckedChanged(object sender, EventArgs e)
        {
            if (TL2_Dire.Checked == true)
            {
                BeastMasterSettings.Instance.TL2_DB = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL2_DB = false;
            }
        }
        private void TL2_Thrill_CheckedChanged(object sender, EventArgs e)
        {
            if (TL2_Thrill.Checked == true)
            {
                BeastMasterSettings.Instance.TL2_TOTH = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL2_TOTH = false;
            }
        }
        private void TL3_Crows_CheckedChanged(object sender, EventArgs e)
        {
            if (TL3_Crows.Checked == true)
            {
                BeastMasterSettings.Instance.TL3_AMOC = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL3_AMOC = false;
            }
        }
        private void TL3_Blink_CheckedChanged(object sender, EventArgs e)
        {
            if (TL3_Blink.Checked == true)
            {
                BeastMasterSettings.Instance.TL3_BSTRK = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL3_BSTRK = false;
            }
        }
        private void TL3_Lynx_CheckedChanged(object sender, EventArgs e)
        {
            if (TL3_Lynx.Checked == true)
            {
                BeastMasterSettings.Instance.TL3_LR = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL3_LR = false;
            }
        }
        private void CallPet_CheckedChanged(object sender, EventArgs e)
        {
            if (CallPet.Checked == true)
            {
                BeastMasterSettings.Instance.CP = true;
            }
            else
            {
                BeastMasterSettings.Instance.CP = false;
            }
        }
        private void SpiritMend_CheckedChanged(object sender, EventArgs e)
        {
            if (SpiritMend.Checked == true)
            {
                BeastMasterSettings.Instance.SMend = true;
            }
            else
            {
                BeastMasterSettings.Instance.SMend = false;
            }
        }
        private void Burrow_CheckedChanged(object sender, EventArgs e)
        {
            if (Burrow.Checked == true)
            {
                BeastMasterSettings.Instance.BRA = true;
            }
            else
            {
                BeastMasterSettings.Instance.BRA = false;
            }
        }
        private void FocusFire_CheckedChanged(object sender, EventArgs e)
        {
            if (FocusFire.Checked == true)
            {
                BeastMasterSettings.Instance.FF = true;
            }
            else
            {
                BeastMasterSettings.Instance.FF = false;
            }
        }
        private void CallWild_CheckedChanged(object sender, EventArgs e)
        {
            if (CallWild.Checked == true)
            {
                BeastMasterSettings.Instance.CW = true;
            }
            else
            {
                BeastMasterSettings.Instance.CW = false;
            }
        }
        private void RevivePet_CheckedChanged(object sender, EventArgs e)
        {
            if (RevivePet.Checked == true)
            {
                BeastMasterSettings.Instance.RP = true;
            }
            else
            {
                BeastMasterSettings.Instance.RP = false;
            }
        }
        private void MendPet_CheckedChanged(object sender, EventArgs e)
        {
            if (MendPet.Checked == true)
            {
                BeastMasterSettings.Instance.MP = true;
            }
            else
            {
                BeastMasterSettings.Instance.MP = false;
            }
        }
        private void Launcher_CheckedChanged(object sender, EventArgs e)
        {
            if (Launcher.Checked == true)
            {
                BeastMasterSettings.Instance.TL = true;
            }
            else
            {
                BeastMasterSettings.Instance.TL = false;
            }
        }
		private void AoEDire_CheckedChanged(object sender, EventArgs e)
        {
            if (AoEDire.Checked == true)
            {
                BeastMasterSettings.Instance.AOEDB = true;
            }
            else
            {
                BeastMasterSettings.Instance.AOEDB = false;
            }
        }
        private void AoELynx_CheckedChanged(object sender, EventArgs e)
        {
            if (AoELynx.Checked == true)
            {
                BeastMasterSettings.Instance.AOELR = true;
            }
            else
            {
                BeastMasterSettings.Instance.AOELR = false;
            }
        }
        private void Trinket1_CheckedChanged(object sender, EventArgs e)
        {
            if (Trinket1.Checked == true)
            {
                BeastMasterSettings.Instance.T1 = true;
            }
            else
            {
                BeastMasterSettings.Instance.T1 = false;
            }
        }
        private void Trinket2_CheckedChanged(object sender, EventArgs e)
        {
            if (Trinket2.Checked == true)
            {
                BeastMasterSettings.Instance.T2 = true;
            }
            else
            {
                BeastMasterSettings.Instance.T2 = false;
            }
        }
        private void Rapid_CheckedChanged(object sender, EventArgs e)
        {
            if (Rapid.Checked == true)
            {
                BeastMasterSettings.Instance.RF = true;
            }
            else
            {
                BeastMasterSettings.Instance.RF = false;
            }
        }
        private void Gloves_CheckedChanged(object sender, EventArgs e)
        {
            if (Gloves.Checked == true)
            {
                BeastMasterSettings.Instance.GE = true;
            }
            else
            {
                BeastMasterSettings.Instance.GE = false;
            }
        }
        private void LifeBlood_CheckedChanged(object sender, EventArgs e)
        {
            if (LifeBlood.Checked == true)
            {
                BeastMasterSettings.Instance.LB = true;
            }
            else
            {
                BeastMasterSettings.Instance.LB = false;
            }
        }
        private void Racial_CheckedChanged(object sender, EventArgs e)
        {
            if (Racial.Checked == true)
            {
                BeastMasterSettings.Instance.RS = true;
            }
            else
            {
                BeastMasterSettings.Instance.RS = false;
            }
        }
        private void MultiShot_CheckedChanged(object sender, EventArgs e)
        {
            if (MultiShot.Checked == true)
            {
                BeastMasterSettings.Instance.MS = true;
            }
            else
            {
                BeastMasterSettings.Instance.MS = false;
            }
        }

        private void AspectSwitching_CheckedChanged(object sender, EventArgs e)
        {
            if (AspectSwitching.Checked == true)
            {
                BeastMasterSettings.Instance.AspectSwitching = true;
            }
            else
            {
                BeastMasterSettings.Instance.AspectSwitching = false;
            }
        }
		
		private void DBeast_CheckedChanged(object sender, EventArgs e)
        {
            if (DBeast.Checked == true)
            {
                BeastMasterSettings.Instance.DB = true;
            }
            else
            {
                BeastMasterSettings.Instance.DB = false;
            }
        }
		private void Glaives_CheckedChanged(object sender, EventArgs e)
        {
            if (Glaives.Checked == true)
            {
                BeastMasterSettings.Instance.GLV = true;
            }
            else
            {
                BeastMasterSettings.Instance.GLV = false;
            }
        }
		private void Silence_CheckedChanged(object sender, EventArgs e)
        {
            if (Silence.Checked == true)
            {
                BeastMasterSettings.Instance.SLS = true;
            }
            else
            {
                BeastMasterSettings.Instance.SLS = false;
            }
        }
		private void Binding_CheckedChanged(object sender, EventArgs e)
        {
            if (Binding.Checked == true)
            {
                BeastMasterSettings.Instance.BDS = true;
            }
            else
            {
                BeastMasterSettings.Instance.BDS = false;
            }
        }
		private void HMark_CheckedChanged(object sender, EventArgs e)
        {
            if (HMark.Checked == true)
            {
                BeastMasterSettings.Instance.HM = true;
            }
            else
            {
                BeastMasterSettings.Instance.HM = false;
            }
        }
		private void Concussive_CheckedChanged(object sender, EventArgs e)
        {
            if (Concussive.Checked == true)
            {
                BeastMasterSettings.Instance.CONC = true;
            }
            else
            {
                BeastMasterSettings.Instance.CONC = false;
            }
        }
		private void Deterrence_CheckedChanged(object sender, EventArgs e)
        {
            if (Deterrence.Checked == true)
            {
                BeastMasterSettings.Instance.DETR = true;
            }
            else
            {
                BeastMasterSettings.Instance.DETR = false;
            }
        }
        private void Misdirect_CheckedChanged(object sender, EventArgs e)
        {
            if (Misdirect.Checked == true)
            {
                BeastMasterSettings.Instance.MDPet = true;
            }
            else
            {
                BeastMasterSettings.Instance.MDPet = false;
            }
        }
		private void LynxRush_CheckedChanged(object sender, EventArgs e)
        {
            if (LynxRush.Checked == true)
            {
                BeastMasterSettings.Instance.LXR = true;
            }
            else
            {
                BeastMasterSettings.Instance.LXR = false;
            }
        }
		private void BWrath_CheckedChanged(object sender, EventArgs e)
        {
            if (BWrath.Checked == true)
            {
                BeastMasterSettings.Instance.BWR = true;
            }
            else
            {
                BeastMasterSettings.Instance.BWR = false;
            }
        }
        private void SaveButton2_Click_1(object sender, EventArgs e)
        {
            BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
            BeastMasterSettings.Instance.SerpentBox = (string)SerBox.SelectedItem;
            BeastMasterSettings.Instance.ScatterBox = (string)ScatBox.SelectedItem;
            BeastMasterSettings.Instance.IntimidateBox = (string)IntiBox.SelectedItem;
            BeastMasterSettings.Instance.Save();
            Logging.Write("Configuration Saved");
            Close();
        }
        private void SaveButton_Click_1(object sender, EventArgs e)
        {
            BeastMasterSettings.Instance.FDCBox = (string)FDBox.SelectedItem;
            BeastMasterSettings.Instance.SerpentBox = (string)SerBox.SelectedItem;
            BeastMasterSettings.Instance.ScatterBox = (string)ScatBox.SelectedItem;
            BeastMasterSettings.Instance.IntimidateBox = (string)IntiBox.SelectedItem;
            BeastMasterSettings.Instance.Save();
            Logging.Write("Configuration Saved");
            Close();
        }
    }
}
