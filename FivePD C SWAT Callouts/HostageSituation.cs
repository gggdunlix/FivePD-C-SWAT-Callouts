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
    [CalloutProperties(name: "Hostage situation", author: "GGGDunlix", version: "1.0")]
    public class HostageSituation : Callout
    {
        //Declaring the ped variables
        private Ped hostage, suspect1, suspect2, suspect3;


        
        /* Callout Credits:+
         * Code written by GGGDunlix
         * GET THE CALLOUT IDEA CREATORS
         */
//List of possible locations
        private List<Vector3> Locations = new List<Vector3>()
        {
            //NEED TO GET STORY MODE INTERIOR LOCATIONS

        };


        public HostageSituation()
        {
            //Callout location
            Vector3 location = Locations.OrderBy(x => World.GetDistance(x, Game.PlayerPed.Position)).Skip(1).First();

            InitInfo(location);
            //Callout Properties
            ShortName = "Hostage Situation";
            CalloutDescription = "There is a hostage held at gunpoint at the location. Respond Code 3.";
            ResponseCode = 3;
            StartDistance = 40f;
            Radius = 5f;
        }
        //Make the suspects shoot hostage if one of them dies, if hostage is dead then they shoot at the player
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

                
                List<WeaponHash> weapons = new List<WeaponHash>() {
                    WeaponHash.CarbineRifle,
                    WeaponHash.AssaultRifleMk2,
                    WeaponHash.MicroSMG,
                    WeaponHash.PistolMk2,
                    WeaponHash.PumpShotgunMk2,
                    WeaponHash.SpecialCarbineMk2

                };
                suspect1 = await SpawnPed(terrorists.SelectRandom(), Vector3Extension.Around(Location, 3f));
                suspect2 = await SpawnPed(terrorists.SelectRandom(), Vector3Extension.Around(Location, 3f));
                suspect3 = await SpawnPed(terrorists.SelectRandom(), Vector3Extension.Around(Location, 3f));


                suspect1.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                suspect2.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                suspect3.Weapons.Give(weapons.SelectRandom(), 250, true, true);
                
                suspect1.AlwaysKeepTask = true;
                suspect1.BlockPermanentEvents = true;
                
                suspect2.AlwaysKeepTask = true;
                suspect2.BlockPermanentEvents = true;
                
                suspect3.AlwaysKeepTask = true;
                suspect3.BlockPermanentEvents = true;

                hostage = await SpawnPed(RandomUtils.GetRandomPed(), Location);
                hostage.Task.Cower(-1);

                suspect1.Task.AimAt(hostage, -1);
                suspect2.Task.AimAt(hostage, -1);
                suspect3.Task.AimAt(hostage, -1);


            }
            catch
            {
                Debug.WriteLine("There was an error with the Hostage Situation Callout. It has been terminated.");
                EndCallout();
            }

//                  All Ped Scanarios need to check if the suspect has a weapon before the task is executed, or else holding E will not stop them!
//              -----------------------------------------------------------------------------------------------------------------------------------------------


        }
        public async Task StartSituation() {
            if (Game.PlayerPed.IsInRangeOf(Location, 10f)) {
                // 33% chance suspects shoot the hostage on arrival, 33%chance hostage gets up and runs away,
                // 33% chance suspects don't ever shoot hostage and it stays, just attack player
                int rnd = new RandomUtils.Random(1, 3);
                if (rnd = 1) {
                    Tick += outcome1;
                } else if (rnd = 2) {
                    Tick += outcome2;
                } else {
                    Tick += outcome3;
                }
            }
        }
        public async Task outcome1() {
            if (hostage.IsAlive()) {
                suspect1.Task.FightAgainst(hostage);
                suspect2.Task.FightAgainst(hostage);
                suspect3.Task.FightAgainst(hostage);
            } else {
                suspect1.Task.FightAgainst(Game.PlayerPed);
                suspect2.Task.FightAgainst(Game.PlayerPed);
                suspect3.Task.FightAgainst(Game.PlayerPed);
            }

        }
        public async Task outcome2() {
            hostage.Task.FleeFrom(suspect1);
            suspect1.Task.FightAgainst(Game.PlayerPed);
            suspect2.Task.FightAgainst(Game.PlayerPed);
            suspect3.Task.FightAgainst(Game.PlayerPed);
        }
        public async Task outcome3() {
            
        }
        public override void OnCancelBefore()
        {
            base.OnCancelBefore();

            try
            {
                if (suspect1.IsAlive && !suspect1.IsCuffed) { suspect1.Delete(); }
                if (suspect2.IsAlive && !suspect2.IsCuffed) { suspect2.Delete(); }
                if (suspect3.IsAlive && !suspect3.IsCuffed) { suspect3.Delete(); }
            }
            catch
            { }
        }
    }
}
