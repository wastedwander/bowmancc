﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx.Helpers;
using System.IO;
using Styx;
using System.Windows.Media;
using Styx.Common;

namespace TheBeastMaster
{
    public class BeastMasterSettings : Settings
    {
        public static readonly BeastMasterSettings Instance = new BeastMasterSettings();

        public BeastMasterSettings()
            : base(Path.Combine(Utilities.AssemblyDirectory, string.Format(@"CustomClasses/Config/TheBeastMaster-Settings-{0}.xml", StyxWoW.Me.Name)))
        {
        }

        [Setting, DefaultValue(true)]
        public bool Party { get; set; }

        [Setting, DefaultValue(false)]
        public bool DSLFR { get; set; }

        [Setting, DefaultValue(false)]
        public bool DSNOR { get; set; }

        [Setting, DefaultValue(false)]
        public bool DSHC { get; set; }

        [Setting, DefaultValue(true)]
        public bool TL1_NO { get; set; }

        [Setting, DefaultValue(true)]
        public bool TL2_NO { get; set; }

        [Setting, DefaultValue(true)]
        public bool TL3_NO { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL1_SS { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL1_WS { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL1_BS { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL2_FV { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL2_DB { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL2_TOTH { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL3_AMOC { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL3_BSTRK { get; set; }

        [Setting, DefaultValue(false)]
        public bool TL3_LR { get; set; }

        [Setting, DefaultValue(false)]
        public bool CP { get; set; }

        [Setting, DefaultValue(false)]
        public bool CW { get; set; }

        [Setting, DefaultValue(false)]
        public bool SMend { get; set; }

        [Setting, DefaultValue(false)]
        public bool RP { get; set; }

        [Setting, DefaultValue(false)]
        public bool MP { get; set; }

        [Setting, DefaultValue(true)]
        public bool MS { get; set; }

        [Setting, DefaultValue(true)]
        public bool TL { get; set; }
		
        [Setting, DefaultValue(true)]
        public bool AOEDB { get; set; }

        [Setting, DefaultValue(false)]
        public bool AOELR { get; set; }

        [Setting, DefaultValue(false)]
        public bool T1 { get; set; }

        [Setting, DefaultValue(false)]
        public bool T2 { get; set; }

        [Setting, DefaultValue(false)]
        public bool RF { get; set; }

        [Setting, DefaultValue(false)]
        public bool LB { get; set; }

        [Setting, DefaultValue(false)]
        public bool GE { get; set; }

        [Setting, DefaultValue(false)]
        public bool RS { get; set; }

        [Setting, DefaultValue(4)]
        public int Mobs { get; set; }

        [Setting, DefaultValue(1)]
        public int PET { get; set; }

        [Setting, DefaultValue(5)]
        public int FFS { get; set; }

        [Setting, DefaultValue(60)]
        public int FocusShots { get; set; }

        [Setting, DefaultValue(50)]
        public int MendHealth { get; set; }

        [Setting, DefaultValue(true)]
        public bool AspectSwitching { get; set; }
		
		[Setting, DefaultValue(true)]
        public bool DB { get; set; }
		
		[Setting, DefaultValue(false)]
        public bool GLV { get; set; }
		
		[Setting, DefaultValue(true)]
        public bool SLS { get; set; }
		
		[Setting, DefaultValue(true)]
        public bool BDS { get; set; }
		
		[Setting, DefaultValue(true)]
        public bool HM { get; set; }
		
		[Setting, DefaultValue(true)]
        public bool CONC { get; set; }
		
		[Setting, DefaultValue(true)]
        public bool LXR { get; set; }

        [Setting, DefaultValue(true)]
        public bool FF { get; set; }
		
		[Setting, DefaultValue(false)]
        public bool DETR { get; set; }
		
		[Setting, DefaultValue(true)]
        public bool BWR { get; set; }

        [Setting, DefaultValue(false)]
        public bool BRA { get; set; }

        [Setting, DefaultValue(true)]
        public bool MDPet { get; set; }

        [Setting, DefaultValue("Never")]
        public string FDCBox { get; set; }

        [Setting, DefaultValue("Always")]
        public string SerpentBox { get; set; }

        [Setting, DefaultValue("1 + 2")]
        public string ScatterBox { get; set; }

        [Setting, DefaultValue("1 + 2")]
        public string IntimidateBox { get; set; }
		
    }
}