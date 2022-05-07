using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetting : MonoBehaviour
{
    [Header("Is Tutorial On?")]
    public bool BallPlay_isTutorial = false;
    [Space]
    [Header("Ball Setting")]
    public float Ball_DynamicFriction = 0.6f;
    public float Ball_StaticFriction = 0.6f;
    public float Ball_Bounciness = 0.7f;
    public float BallLiveLimitTime = 15f;
    public float BallRespawnTime = 3.0f;
    [Space]
    [Header("SpaceShip")]
    public float Item_SpaceShip_Catch_Time = 3.0f;
    public float Item_SpaceShip_Catch_Delay = 3.0f;
    public float Item_SpaceShip_MoveDistance = 5f;
    [Space]
    [Header("Portal")]
    public float Item_Portal_Reset_Time = 5.0f;
    [Space]
    [Header("Floor Cannon")]
    public bool Item_Reflection_InfinityStay = false;
    public float Item_Reflection_AddforceY = 300.0f;
    public float Item_Reflection_NewCreateDelayTime = 5.0f;
    [Space]
    [Header("Side Cannon")]
    public float Item_Spring_Addforce = 300.0f;
    public float Item_Spring_NewCreateDelayTime = 2.0f;
    [Space]
    [Header("Shard")]
    public float Item_Shard_Reset_time = 2.0f;
    [Space]
    [Header("Neon")]
    public float Item_Neon_DelayTime = 20.0f;
    [Space]
    [Header("Goal")]
    public float Item_Goal_MoveTime = 5.0f;
    public float Item_Goal_DelayTime = 20.0f;
}
