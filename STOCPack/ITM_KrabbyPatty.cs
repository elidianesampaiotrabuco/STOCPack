using System;
using System.Collections.Generic;
using System.Text;

namespace STOCPack
{
    public class ITM_KrabbyPatty : Item
    {
        public SoundObject KrabbyPattyCrunch;

        public override bool Use(PlayerManager pm)
        {
            pm.plm.stamina = pm.plm.staminaMax * 2f;
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(KrabbyPattyCrunch);
            return true;
        }
    }
}
