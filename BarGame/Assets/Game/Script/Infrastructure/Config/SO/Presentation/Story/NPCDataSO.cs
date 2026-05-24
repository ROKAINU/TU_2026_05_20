using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Domain;
using Game.Application.Contracts;

namespace Game.Infrastructure
{
    // ===== NPCデータ定義 =====
    [CreateAssetMenu(menuName = "NPC/NPCData")]
    public class NPCDataSO : ScriptableObject
    {
        public string npcId;
        public string displayName;
        public Sprite portrait;
        
        // Ink会話スクリプト
        public TextAsset conversationInkJson;

        // NPCが要求する注文
        public CustomerOrder customerOrder;
        
        // 追加のNPC固有データ
    }

    // ===== NPC管理 =====
    [CreateAssetMenu(menuName = "NPC/NPCDatabase")]
    public class NPCDatabaseSO : ScriptableObject
    {
        public List<NPCDataSO> npcs;
        
        public NPCDataSO GetNPC(string npcId)
        {
            return npcs.FirstOrDefault(n => n.npcId == npcId);
        }
    }
}