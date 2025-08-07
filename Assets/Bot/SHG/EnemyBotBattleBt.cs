using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace SHG
{
    public class EnemyBotBattleBt : EnemyBotBt
    {
        IBot bot;
        LocalPlayerController player;
        public EnemyBotBattleBt(
            IBot bot,
            LocalPlayerController player,
            IList<BtNode> children = null): base (bot, children)
        {
            this.bot = bot;
            this.player = player;
        }

        public override NodeState Evaluate()
        {
            if (this.player.Hp.Value.current <= 0) {
                return (this.ReturnState(NodeState.Success));
            }
            if (this.bot.IsStopped) {
                float dist = Vector3.Distance(
                    this.bot.Transform.position,
                    this.player.transform.position
                );
                if (dist < IBot.ATTACK_RANGE) {
                    this.bot.Attack(this.player);
                }
            }
            this.bot.NavMeshAgent.SetDestination(this.player.transform.position);
            this.bot.NavMeshAgent.isStopped = false;
            return (this.ReturnState(NodeState.Running));
        }
    }
}