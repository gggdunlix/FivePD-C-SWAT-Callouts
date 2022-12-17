using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using FivePD.API;
using FivePD.API.Utils;

#pragma warning disable 1998
namespace FivePDCSWATCallouts
{
    [CalloutProperties(name: "USLA Shooting", author: "GGGDunlix", version: "1.0")]
    public class USLAShooting : Callout
    {
        //Declaring the ped variables
        private Ped terror1_1, terror1_2, terror1_3, terror1_4, terror2_1, terror2_2, terror2_3, terror2_4;
        private Vehicle van1, van2;
        private bool phase1, phase2a;
        private bool phase2b, phase2spawned;


        //List of possible locations
        /* Callout Credits:+
         * Code written by GGGDunlix
         * Ideas and inspiration by Platinum Dev and Commodore
         */

        private List<Vector3> Locations = new List<Vector3>()
        {
            //NEED TO GET USLA COLLEGE LOCATIONS
            new Vector3(-1736.077f, 161.913f, 64.37097f),
            new Vector3(-1692.991f, 195.0446f, 63.8455f),
            new Vector3(-1646.493f, 145.7752f, 62.07328f),
            new Vector3(-1614.085f, 183.8651f, 59.85993f),
            new Vector3(-1622.292f, 221.009f, 60.28655f),
            new Vector3(-1736.575f, 242.3569f, 65.11923f),
            new Vector3(-1664.675f, 264.3767f, 62.39096f),
            new Vector3(-1537.373f, 220.9161f, 60.09211f)


        };


        public USLAShooting()
        {
            //Callout location
            InitInfo(Locations.SelectRandom());

            //Callout Properties
            ShortName = "USLA Shooting";
            CalloutDescription = "There is a terrorist group shooting at the San Andreas State University. Neutralize the threat, Respond Code 3.";
            ResponseCode = 3;
            StartDistance = 40f;
            Radius = 5f;
        }

        public override async Task OnAccept()
        {
            try
            {
                InitBlip();
                UpdateData();
            }
            catch
            {
                EndCallout();
            }
        }

        public async override void OnStart(Ped player)
        {


            base.OnStart(player);
            try
            {
                List<PedHash> terrorists = new List<PedHash>()
                {
                    PedHash.EdToh,
                    PedHash.Armoured01,
                    PedHash.Armoured02SMM,
                    PedHash.GunVend01,
                    PedHash.Bouncer01SMM

                };

                List<VehicleHash> vans = new List<VehicleHash>() {
                    VehicleHash.Burrito,
                    VehicleHash.Burrito2,
                    VehicleHash.GBurrito2,
                    VehicleHash.Youga,
                    VehicleHash.Youga2
                };
                List<WeaponHash> weapons = new List<WeaponHash>() {
                    WeaponHash.BZGas,
                    WeaponHash.CarbineRifle,
                    WeaponHash.AssaultRifleMk2,
                    WeaponHash.MicroSMG,
                    WeaponHash.Molotov,
                    WeaponHash.PistolMk2,
                    WeaponHash.PumpShotgunMk2,
                    WeaponHash.SpecialCarbineMk2

                };
                terror1_1 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror1_2 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror1_3 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror1_4 = await SpawnPed(terrorists.SelectRandom(), Location);


                van1 = await SpawnVehicle(vans.SelectRandom(), Location + 3);

                terror1_1.SetIntoVehicle(van1, VehicleSeat.Driver);
                terror1_2.SetIntoVehicle(van1, VehicleSeat.Any);
                terror1_3.SetIntoVehicle(van1, VehicleSeat.Any);
                terror1_4.SetIntoVehicle(van1, VehicleSeat.Any);

                terror1_1.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror1_2.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror1_3.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror1_4.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror1_1.AlwaysKeepTask = true;
                terror1_1.BlockPermanentEvents = true;
                terror1_2.AlwaysKeepTask = true;
                terror1_2.BlockPermanentEvents = true;
                terror1_3.AlwaysKeepTask = true;
                terror1_3.BlockPermanentEvents = true;
                terror1_4.AlwaysKeepTask = true;
                terror1_4.BlockPermanentEvents = true;

                phase1 = true;
                phase2a = false;
                phase2b = false;
                Debug.WriteLine("Started ticking!");
                terror1_1.Task.LeaveVehicle();
                terror1_2.Task.LeaveVehicle();
                terror1_3.Task.LeaveVehicle();
                terror1_4.Task.LeaveVehicle();

                Tick += Phase1Shootout;




            }
            catch
            {
                Debug.WriteLine("There was an error with the USLA Shooting Callout. It has been terminated.");
                EndCallout();
            }




        }
        public async Task Phase1Shootout()
        {
            await BaseScript.Delay(2000);
            List<Ped> terroristPeds = new List<Ped>
            {
                terror1_1, terror1_2, terror1_3, terror1_4, terror2_1, terror2_2, terror2_3, terror2_4
            };
            if (phase1)
            {
                await BaseScript.Delay(1000);
                await AttackNearestPed(terror1_1, 100f, terroristPeds);
                await AttackNearestPed(terror1_2, 100f, terroristPeds);
                await AttackNearestPed(terror1_3, 100f, terroristPeds);
                await AttackNearestPed(terror1_4, 100f, terroristPeds);
            }
            else
            {
                Tick += Phase2Shootout;
            }
            if ((terror1_1.IsDead || terror1_1.IsCuffed) && (terror1_2.IsDead || terror1_2.IsCuffed) && (terror1_2.IsDead || terror1_2.IsCuffed) && (terror1_3.IsDead || terror1_3.IsCuffed) && (terror1_4.IsDead || terror1_4.IsCuffed))
            {
                phase2a = true;
                phase1 = false;
            }


        }
        public async Task Phase2Shootout()
        {
            List<Ped> terroristPeds = new List<Ped>
            {
                terror1_1, terror1_2, terror1_3, terror1_4, terror2_1, terror2_2, terror2_3, terror2_4
            };
            if (phase2a && !phase2spawned) {
                List<PedHash> terrorists = new List<PedHash>()
                {
                    PedHash.EdToh,
                    PedHash.Armoured01,
                    PedHash.Armoured02SMM,
                    PedHash.GunVend01,
                    PedHash.Bouncer01SMM

                };

                List<VehicleHash> vans = new List<VehicleHash>() {
                    VehicleHash.Burrito,
                    VehicleHash.Burrito2,
                    VehicleHash.GBurrito2,
                    VehicleHash.Youga,
                    VehicleHash.Youga2
                };
                List<WeaponHash> weapons = new List<WeaponHash>() {
                    WeaponHash.BZGas,
                    WeaponHash.CarbineRifle,
                    WeaponHash.AssaultRifleMk2,
                    WeaponHash.MicroSMG,
                    WeaponHash.Molotov,
                    WeaponHash.PistolMk2,
                    WeaponHash.PumpShotgunMk2,
                    WeaponHash.SpecialCarbineMk2

                };
                terror2_1 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror2_2 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror2_3 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror2_4 = await SpawnPed(terrorists.SelectRandom(), Location);


                van2 = await SpawnVehicle(vans.SelectRandom(), World.GetNextPositionOnStreet(Location + 5));

                terror2_1.SetIntoVehicle(van2, VehicleSeat.Driver);
                terror2_2.SetIntoVehicle(van2, VehicleSeat.Any);
                terror2_3.SetIntoVehicle(van2, VehicleSeat.Any);
                terror2_4.SetIntoVehicle(van2, VehicleSeat.Any);

                terror2_1.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror2_2.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror2_3.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror2_4.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                terror2_1.AlwaysKeepTask = true;
                terror2_1.BlockPermanentEvents = true;
                terror2_2.AlwaysKeepTask = true;
                terror2_2.BlockPermanentEvents = true;
                terror2_3.AlwaysKeepTask = true;
                terror2_3.BlockPermanentEvents = true;
                terror2_4.AlwaysKeepTask = true;
                terror2_4.BlockPermanentEvents = true;

                phase2spawned = true;
                
                terror2_1.Task.DriveTo(van2, van1.Position, 15f, 30, 786468);
            }
            if (phase2spawned) {
                if (van2.IsInRangeOf(van1.Position, 25f) && van1.Speed < 10) {
                    terror2_1.Task.LeaveVehicle();
                    terror2_2.Task.LeaveVehicle();
                    terror2_3.Task.LeaveVehicle();
                    terror2_4.Task.LeaveVehicle();
                    BaseScript.Delay(1000);
                    if (!terror2_1.IsInVehicle()) {
                        await AttackNearestPed(terror2_2, 100f, terroristPeds);
                        await AttackNearestPed(terror2_2, 100f, terroristPeds);
                        await AttackNearestPed(terror2_2, 100f, terroristPeds);
                        await AttackNearestPed(terror2_2, 100f, terroristPeds);
                        phase2b = true;
                    }
                }
            }
            if (phase2b)
            {
                await BaseScript.Delay(1000);
                await AttackNearestPed(terror2_1, 100f, terroristPeds);
                await AttackNearestPed(terror2_2, 100f, terroristPeds);
                await AttackNearestPed(terror2_3, 100f, terroristPeds);
                await AttackNearestPed(terror2_4, 100f, terroristPeds);
            }
            
        }
        public async Task AttackNearestPed(Ped attacker, float radius, List<Ped> ignoreThem)
        {

            List<Ped> allPeds = World.GetAllPeds().ToList();

            List<Ped> attackablePeds = new List<Ped>();
            foreach (Ped p in allPeds)
            {
                if (!ignoreThem.Contains(p) && attacker.IsInRangeOf(p.Position, radius) && p.Equals(attacker))
                {
                    attackablePeds.Add(p);
                }
            }
            attackablePeds.OrderBy(x => World.GetDistance(x.Position, attacker.Position));
            await BaseScript.Delay(2500);
            attacker.Task.FightAgainst(attackablePeds.First());

        }
        public override void OnCancelBefore()
        {
            base.OnCancelBefore();

            try
            {
                if (terror1_1.IsAlive && !terror1_1.IsCuffed) { terror1_1.Delete(); }
            }
            catch
            { }
        }
    }
}
