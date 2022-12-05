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
        private Ped terror1_1, terror1_2, terror1_3, terror1_4, terror2_1, terror2_2, terror2_3, terror2_4,;
        private Vehicle van1, van2;

        //List of possible locations
        
        private List<Vector3> Locations = new List<Vector3>()
        {
            //NEED TO GET USLA COLLEGE LOCATIONS
            new Vector3(2336.882f, 3136.938f, 48.1965f),
            new Vector3(2707.504f, 4144.324f, 43.8383f)
            
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
                    PedHash.EdToh
                };
                
                List<VehicleHash> vans = new List<VehicleHash>() {
                    VehicleHash.gburrito,
                    VehicleHash.
                }
                List<WeaponHash> weapons = new List<Weapons>() {
                    WeaponHash.TearGas,
                    WeaponHash.CarbineRifle,
                    WeaponHash.AssualtRifleMk2,
                    WeaponHash.MicroSMG,
                    
                }
                terror1_1 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror1_2 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror1_3 = await SpawnPed(terrorists.SelectRandom(), Location);
                terror1_4 = await SpawnPed(terrorists.SelectRandom(), Location);
               
               
                van1 = await SpawnVehicle(vans.SelectRandom(), Location + 3);
                
                terror1_1.Weapons.Give(Weap)
                terror1_1.AlwaysKeepTask = true;
                terror1_1.BlockPermanentEvents = true;
                terror1_2.AlwaysKeepTask = true;
                terror1_2.BlockPermanentEvents = true;
                terror1_3.AlwaysKeepTask = true;
                terror1_3.BlockPermanentEvents = true;
                terror1_4.AlwaysKeepTask = true;
                terror1_4.BlockPermanentEvents = true;
                

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
                    apPistolItem.Name = "Automatic Pistol";
                    apPistolItem.IsIllegal = true;
                    
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
