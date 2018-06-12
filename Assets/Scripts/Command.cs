using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Command : MonoBehaviour {

    public enum CommandType
    {
        Offensive,
        Defensive,
        Reactive,
        Supportive,
    }

    public enum TargetType
    {
        Front,
        Around, //Cross at unit's cell
        Self, //at unit's cell
        Line, //Beam Attacks
        Triangle, //Breath attacks
        AtCell, //At a determined cell in front of unit, the distance is customizable
        CrossAtCell, //Cross at a determined cell in front of unit, the distance is customizable
        //Diagonal,
        All
    }

    public enum Buff
    {
        Guard,                                                                  //Guarding State
        Protect,
        Barrier,
        Regen,
        Haste
    }

    public enum Debuff
    {
        Weakness,
        Poison,
        Burn,
        Freeze,
        Petrify,
        Stun
    }

    [Header("General Attributes")]
    /// <summary>
    /// The name of the Command.
    /// </summary>
    public string cmdName;

    /// <summary>
    /// Description of what the command do.
    /// Ex.: 
    /// Attack - Attack an unit in front of you with your main weapon.
    /// Damage: 100%
    /// AP Cost: 2
    /// </summary>
    [TextArea]
    public string cmdDesc;

    /// <summary>
    /// Determines what type of skill this command will evoke.
    /// Offensive: Attack, Fire, Venom Sting, etc. (Almost everything that causes direct damage, debuff or damage over time)
    /// Defensive: Guard, Raise Shield, Smoke Bomb, Reflex, etc. (Actions that minimized the damage taken or has a chance to avoid the damage completely)
    /// Reactive: Counter, Counter Magic, Auto Haste, etc. (Actions that are triggered when the unit receives a specified type of attack)
    /// 
    /// You can only have one Reactive or Defensive command per Program.
    /// Executing an Defensive/Reactive command will end your turn.
    /// 
    /// Supportive: Cure, Regen, Place Trap, Protection, Barrier, Haste, etc. (Almost every Actions that supports you or another unit of the same team)
    /// </summary>
    public CommandType cmdType;

    /// <summary>
    /// Icon of the Command to be presenetd in the Command Set
    /// </summary>
    public Sprite cmdIcon;

    /// <summary>
    /// Determines if this command is on the Command Set (false) or in the Program (true).
    /// This interfere with what action it will execute when clicked.
    /// </summary>
    public bool isOnProgram;


    //Depending of the Type, some of the following attributes will be read
    //Cost in AP
    [Header("AP Cost")]
    public int cmdCost;

    [Header("Power Attributes")]
    //Power can be used for Physical Attacks, Magical Attacks, Healings, Defensive Actions
    public int powerSum;
    public float powerMultiplier;                                               //Values like 0.75 (for 75%), 1 for 100%, 1.2 for 120%, 2 for 200% attack, etc.
    public bool isPhysical;                                                     //true = Physical / false = Magical
    public bool ignoreDefense;                                                  //true for ignoring Defense/Resistance
    public float criticalRate;                                                  //If critical it will cause (Calculated Damage) * 1.5.

    [Header("Hit Attributes")]
    public bool canMiss;                                                        //true = canMiss / false = cantMiss
    public int hitRateSum;
    public float hitRateMultiplier;

    [Header("Target Attributes")]
    public TargetType cmdTarget;
    [Range(2, 6)]
    public int distance;                                                        //If AtCell or CrossAtCell, the target cell is calculated in the following way: unit's cell + distance;

    [Header("Buff Attributes")]
    public bool canCauseBuff;
    public Buff cmdBuff;
    public float buffSuccessRate;

    [Header("Debuff Attributes")]
    public bool canCauseDebuff;
    public Debuff cmdDebuff;
    public float debuffSuccessRate;


    //Calculating Methods

    public float GetHitChance(Unit attacker, Unit defender)
    {
        //Example 100 acc x 100 eva -> (100 * 1) - 40 = 60% to Hit. 
        //Summarizing, if you have the same value of acc and eva, the chance ot hit is 60%
        float hitCalc = (attacker.accuracy * hitRateMultiplier) + hitRateSum;
        hitCalc -= (defender.evasion * 0.4f);
        hitCalc = Mathf.Clamp(hitCalc, 5, 95);                                  //Minimun chance of Hit: 5% / Maximum chance of hit: 95%.
        return hitCalc;
    }

    public bool HitSuccess(Unit attacker, Unit defender)
    {
        if(!canMiss)
            return true;

        float hitCalc = GetHitChance(attacker, defender);

        float rnd = Random.Range(0, 100);
        if (rnd <= hitCalc)
            return true;
        
        return false;
    }

    public int DamageCaused(Unit attacker, Unit defender)
    {
        float damage = 0;

        if (isPhysical)
            damage = ((attacker.attack) * powerMultiplier) + powerSum;
        else
            damage = ((attacker.magicpow) * powerMultiplier) + powerSum;
        
        if(!ignoreDefense)
        {
            if (isPhysical)
            {
                    
                damage = damage - (defender.defense * 0.75f);

                if (defender.buffs.Contains(Buff.Guard))
                    damage *= 0.75f;
                
                if (defender.buffs.Contains(Buff.Protect))
                    damage *= 0.5f;
            }
            else
                damage = damage - (defender.resistance * 0.75f);
        }

        float rnd = Random.Range(0, 100);
        float newCriticalRate = (attacker.luck > 10)? criticalRate + (attacker.luck / 10) : criticalRate;
        newCriticalRate -= (defender.luck / 10) * 0.5f;
        newCriticalRate = Mathf.Clamp(newCriticalRate, 1, 50);                  //Minimun chance of Critical: 1% / Maximum Chance of Critical: 50%. 

        if(rnd <= newCriticalRate)
        {
            damage *= 1.5f;
        }

        return (int)damage;
    }

    public int HealingDone(Unit caster)
    {
        float healing = 0;

        if(isPhysical)                                                          //Yes, there will be the most common Magical Healings but also some type of Physical Healing
            healing = ((caster.attack) * powerMultiplier) + powerSum;
        else
            healing = ((caster.magicpow) * powerMultiplier) + powerSum;

        float rnd = Random.Range(0, 100);
        float newCriticalRate = (caster.luck > 10) ? criticalRate + (caster.luck / 10) : criticalRate;
        newCriticalRate = Mathf.Clamp(newCriticalRate, 1, 50);                  //Minimun chance of Critical: 1% / Maximum Chance of Critical: 50%. 

        if (rnd < newCriticalRate)
        {
            healing *= 1.5f;
        }

        return (int)healing;
    }


}
