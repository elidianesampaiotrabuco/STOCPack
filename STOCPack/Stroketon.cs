using MTM101BaldAPI.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace STOCPack
{
    public class Stroketon : NPC
    {
        public Sprite stroketonSprite;
        public SoundObject stroketonDrJr;
        public float WanderSpeed = 50f;

        public override void Initialize()
        {
            base.Initialize();

            spriteRenderer[0].sprite = stroketonSprite;

            AudioManager audioManager = GetComponent<AudioManager>();
            PropagatedAudioManager drjrPlayer = base.gameObject.AddComponent<PropagatedAudioManager>();
            drjrPlayer.audioDevice = base.gameObject.AddComponent<AudioSource>();
            drjrPlayer.ReflectionSetVariable("soundOnStart", new SoundObject[] { stroketonDrJr });
            drjrPlayer.ReflectionSetVariable("loopOnStart", true);

            behaviorStateMachine.ChangeState(new Stroketon_Wander(this));

            base.Navigator.SetSpeed(this.WanderSpeed);

            base.Navigator.maxSpeed = this.WanderSpeed;
        }
    }

    public class Stroketon_StateBase : NpcState
    {
        protected Stroketon plankton;

        public Stroketon_StateBase(Stroketon st) : base(st)
        {
            plankton = st;
        }
    }

    public class Stroketon_Wander : Stroketon_StateBase
    {
        public Stroketon_Wander(Stroketon st) : base(st)
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

        public override void DestinationEmpty()
        {
            base.DestinationEmpty();
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }
    }

}
