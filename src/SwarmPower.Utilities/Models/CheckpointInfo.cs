using System;
using System.Collections.Generic;
using System.Text;

namespace SwarmPower.Utilities.Models
{
    public class CheckpointInfo
    {
        public string Hash { get; set; }
        public string StakeModifierV2 { get; set; }
        public CheckpointInfo(string hash, string stakeModifierV2 = null)
        {
            this.Hash = hash;
            this.StakeModifierV2 = stakeModifierV2;
        }
    }
}
