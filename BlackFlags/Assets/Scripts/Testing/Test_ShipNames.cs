using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

using Generation.Generators;
using Generation.Ships;
using GameMechanics.Data;
using GameMechanics.Ships;
public class Test_ShipNames : MonoBehaviour
{
    [SerializeField] private ShipGenerator generator;
    //void Start()
    //{
        
    //    WorldGenerator.Initialize();

    //    generator = WorldGenerator.GetShipSpawnData(4);
    //    for (int i = 0; i < 1; i++)
    //    {
    //        Ship newShip = generator.GenerateShipData(ShipType_ROLE.Patrol, EntityType_KINGDOM.KINGDOM_Britain);


            
    //        //print(newShip.GetSubClassName());
    //        //print(newShip.GetModelName());

    //        //print("Velocidad a barlovento: " + newShip.GetMaxSmoothSpeed());
    //        //print("Velocidad de ceñida: " + newShip.GetMaxUpwindSpeed());
    //        //print(Ship.GetCompleteName(newShip));


    //        // var s = Ship.GetSpeed(newShip);

    //        //for (int j = 0; j < s.Length; j++)
    //        //{
    //        //    print(s[i]);
    //        //}

    //    }


    //    //for (int i = 0; i < 200; i++)
    //    {
    //        Ship newShip = generator.GenerateShipData(ShipType_ROLE.Merchant, EntityType_KINGDOM.KINGDOM_Britain);
    //        print(newShip.GetModelName());
    //    }

    //    //Stopwatch clock = Stopwatch.StartNew();
    //    //for (int i = 0; i < 80000; i++)
    //    //{
    //    //    WorldGenerator.GiveShipName(EntityType_KINGDOM.KINGDOM_France, ShipType_ROLE.Pirate,GenerationMode.Sequence);
    //    //}
    //    //clock.Stop();
    //    //UnityEngine.Debug.Log(clock.Elapsed);
    //}

    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Space))
    //    //{

    //    //    print(WorldGenerator.GetCharacterName(EntityType_KINGDOM.KINGDOM_Britain, true));


    //    //    //Ship newShip = generator.GenerateShipData(ShipType_ROLE.Patrol, EntityType_KINGDOM.KINGDOM_Britain);
    //    //    //print(newShip.GetSubClassName());
    //    //    //print(newShip.GetModelName());

    //    //    //print(newShip.GetMaxSmoothSpeed());
    //    //    //print(Ship.GetCompleteName(newShip));

    //    //    //print(WorldGenerator.GiveShipName(EntityType_KINGDOM.KINGDOM_France, ShipType_ROLE.Pirate));
    //    //    //print(WorldGenerator.GetCharanterName(EntityType_KINGDOM.KINGDOM_Portugal, true));
    //    //}
    //}
}
