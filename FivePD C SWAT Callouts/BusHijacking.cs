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

        //List of possible locations
        /*
        private List<Vector3> Locations = new List<Vector3>()
        {
            new Vector3(2336.882f, 3136.938f, 48.1965f),
            new Vector3(2707.504f, 4144.324f, 43.8383f),
            new Vector3(1901.271f, 4912.473f, 48.78623f),
            new Vector3(-316.1659f, 6313.192f, 32.29678f),
            new Vector3(-1575.015f, 5162.655f, 19.59452f),
            new Vector3(403.4432f, 2636.324f, 44.49727f),
            new Vector3(414.0077f, -1166.904f, 29.29198f),
            new Vector3(106.2034f, -1813.366f, 26.52958f),
            new Vector3(908.4533f, -1655.34f, 30.18371f),
            new Vector3(1071.322f, -711.8113f, 58.47411f),
            new Vector3(-1024.314f, 368.9541f, 71.36354f),
            new Vector3(-841.6989f, -1050.529f, 11.29686f)
        };
        InitInfo(Locations.SelectRandom());
        */
        public BusHijacking()
        {
            //Callout location
            InitInfo(World.GetNextPositionOnStreet(Vector3Extension.Around(Game.PlayerPed.Position, 120f), true));

            //Callout Properties
            ShortName = "Bus Hijacking";
            CalloutDescription = "A bus has been hijacked by an unknown individual, and multiple innocent passengers are on board. Respond Code 3.";
            ResponseCode = 3;
            StartDistance = 60f;
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
                    PedHash.Epsilon01AMY,
                    PedHash.EdToh,
                    PedHash.Groom,
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
                bus = await SpawnVehicle(VehicleHash.Bus, Location);
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

                PedData driverData = await driver.GetData();
                //does driver have gun?
                if (RandomUtils.GetRandomNumber(0, 1) == 0)
                {
                    //Driver does not have gun.
                    driver.Weapons.Give(WeaponHash.Machete, 1, true, true);
                    driver.Task.FleeFrom(Game.PlayerPed, -1);
                    driver.DrivingStyle = DrivingStyle.AvoidTrafficExtremely;
                    
                     //Items
                    Item knife = new Item();
                    knife.Name = "Bloody Knife";
                    knife.IsIllegal = true;
                    
                    data.Items.Add(knife);

                }
                {
                    driver.Weapons.Give(WeaponHash.APPistol, 200, true, true);
                    driver.Task.FleeFrom(Game.PlayerPed, -1);
                    driver.DrivingStyle = DrivingStyle.Rushed;
                    //Items
                    Item apPistolItem = new Item();
                    knife.Name = "Automatic Pistol";
                    knife.IsIllegal = true;
                    
                    data.Items.Add(apPistolItem);

                    driver.Task.VehicleShootAtPed(Game.PlayerPed);
                }
                driver.SetData(data);

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
