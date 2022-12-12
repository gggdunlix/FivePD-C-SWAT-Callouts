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
    [CalloutProperties(name: "Bus Hijacking", author: "GGGDunlix", version: "1.0")]
    public class BusHijacking : Callout
    {
        //Declaring the ped variables
        private Ped driver, passenger1, passenger2, passenger3, passenger4;
        private Vehicle bus;


        /*Callout Credits:
        Code Written by GGGDunlix
        Coding assistance given by HuskyNinja99
        
        */
        public BusHijacking()
        {
            //Callout location
            InitInfo(World.GetNextPositionOnStreet(Vector3Extension.Around(Game.PlayerPed.Position, 300f), true));

            //Callout Properties
            ShortName = "Bus Hijacking";
            CalloutDescription = "A bus has been hijacked by an unknown individual, and multiple innocent passengers are on board. Respond Code 3.";
            ResponseCode = 3;
            StartDistance = 150f;
            Radius = 2f;
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
                List<PedHash> drivers = new List<PedHash>()
                {
                    PedHash.Chef,
                    PedHash.Cletus,
                    PedHash.Clown01SMY,
                    PedHash.CocaineMale01,
                    PedHash.Dealer01SMY,
                    PedHash.EdToh,
                    PedHash.Indian01AMY
                };
                List<PedHash> peds = new List<PedHash>()
                {
                    PedHash.Abigail,
                    PedHash.Agent14,
                    PedHash.Ashley,
                    PedHash.Barry,
                    PedHash.Benny,
                    PedHash.Beverly,
                    PedHash.Bride,
                    PedHash.DaveNorton,
                    PedHash.Milton,
                    PedHash.TennisCoach
                };
                driver = await SpawnPed(drivers.SelectRandom(), Location);
                passenger1 = await SpawnPed(peds.SelectRandom(), Location);
                passenger2 = await SpawnPed(peds.SelectRandom(), Location);
                passenger3 = await SpawnPed(peds.SelectRandom(), Location);
                passenger4 = await SpawnPed(peds.SelectRandom(), Location);
                

                
                Vector3 lookingAt = World.GetNextPositionOnStreet(new Vector3(Location.X + 2, Location.Y + 2, Location.Z));

                float busHeading = GetHeadingFromVector_2d((Location.X - lookingAt.X), (Location.Y - lookingAt.Y));
                bus = await SpawnVehicle(VehicleHash.Bus, Location, busHeading);
                await BaseScript.Delay(1000);
                driver.SetIntoVehicle(bus, VehicleSeat.Driver);
                passenger1.SetIntoVehicle(bus, VehicleSeat.Any);
                passenger2.SetIntoVehicle(bus, VehicleSeat.Any);
                passenger3.SetIntoVehicle(bus, VehicleSeat.Any);
                passenger4.SetIntoVehicle(bus, VehicleSeat.Any);

                driver.AlwaysKeepTask = true;
                driver.BlockPermanentEvents = true;
                passenger1.AlwaysKeepTask = true;
                passenger1.BlockPermanentEvents = true;
                passenger2.AlwaysKeepTask = true;
                passenger2.BlockPermanentEvents = true;
                passenger3.AlwaysKeepTask = true;
                passenger3.BlockPermanentEvents = true;
                passenger4.AlwaysKeepTask = true;
                passenger4.BlockPermanentEvents = true;


                passenger1.Task.Cower(-1);
                passenger2.Task.Cower(-1);
                passenger3.Task.Cower(-1);
                passenger4.Task.Cower(-1);


                PedData driverData = await driver.GetData();
                //does driver have gun?
                
                    //Driver does not have gun.
                    driver.Weapons.Give(WeaponHash.Machete, 1, true, true);
                    driver.Task.FleeFrom(Game.PlayerPed, -1);
                    driver.DrivingStyle = DrivingStyle.AvoidTrafficExtremely;
                    
                     //Items
                    Item knife = new Item();
                    knife.Name = "Bloody Knife";
                    knife.IsIllegal = true;
                    
                    driverData.Items.Add(knife);
                Utilities.ExcludeVehicleFromTrafficStop(bus.NetworkId, true);
                driver.SetData(driverData);
                FivePD.API.Pursuit.RegisterPursuit(driver);
                bus.AttachBlip();
                
            }
            catch
            {
                EndCallout();
            }



        }
        
        public override void OnCancelBefore()
        {
            base.OnCancelBefore();
            
            try
            {
                if (driver.IsAlive && !driver.IsCuffed) { driver.Delete(); }
            }
            catch
            { }
        }
    }
}
