using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace STOCPack
{
    public class Shaldi : NPC
    {
        public Sprite shaldiSprite;

        public float WanderSpeed = 2f;

        //private bool caught;

        public override void Initialize()
        {
            base.Initialize();

            spriteRenderer[0].sprite = shaldiSprite;

            behaviorStateMachine.ChangeState(new Shaldi_Wander(this));

            base.Navigator.SetSpeed(this.WanderSpeed);

            base.Navigator.maxSpeed = this.WanderSpeed;
        }

        public void StartPersuingPlayer(PlayerManager player)
        {
            behaviorStateMachine.ChangeNavigationState(new NavigationState_TargetPlayer(this, 63, player.transform.position));
        }
        public void PersuePlayer(PlayerManager player)
        {
            behaviorStateMachine.CurrentNavigationState.UpdatePosition(player.transform.position);
        }
    }

    public class Shaldi_StateBase : NpcState
    {
        protected Shaldi PinkGuy;

        public Shaldi_StateBase(Shaldi shald) : base(shald)
        {
            PinkGuy = shald;
        }
    }

    public class Shaldi_Wander : Shaldi_StateBase
    {
        private bool caught;

        public Shaldi_Wander(Shaldi shald) : base(shald)
        {

        }

        public override void Enter()
        {
            base.Enter();
            if (!npc.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            }
        }

        public override void OnStateTriggerEnter(Collider other, bool validCollision)
        {
            base.OnStateTriggerEnter(other, validCollision);
            PlayerManager component = other.GetComponent<PlayerManager>();
            bool flag = validCollision && component != null && !this.caught;
            if (flag)
            {
                Baldi baldi = this.npc.ec.GetBaldi();
                bool flag2 = baldi == null;
                if (flag2)
                {
                    this.npc.ec.SpawnNPC(Resources.FindObjectsOfTypeAll<NPC>().First((NPC x) => x.name == "Baldi"), default(IntVector2));
                    baldi = this.npc.ec.GetBaldi();
                }
                this.npc.Entity.SetHeight(4f);
                this.npc.gameObject.transform.LookAt(component.transform);
                baldi.transform.position = this.npc.gameObject.transform.position;
                baldi.Entity.SetHeight(-5f);
                Singleton<CoreGameManager>.Instance.EndGame(component.transform, baldi);
                this.caught = true;
            }
        }

        public override void DestinationEmpty()
        {
            base.DestinationEmpty();
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }

        public override void PlayerSighted(PlayerManager player)
        {
            base.PlayerSighted(player);
            if (!player.Tagged)
            {
                PinkGuy.StartPersuingPlayer(player);
            }
        }

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            if (!player.Tagged)
            {
                PinkGuy.PersuePlayer(player);
            }
        }
    }
}
