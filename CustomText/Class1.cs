using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Networking;

namespace CustomText
{
    public class NameOnShip : VTOLMOD
    {
        public override void ModLoaded()
        {
            this.createSettings();

            VTOLAPI.MissionReloaded = (UnityAction)Delegate.Combine(VTOLAPI.MissionReloaded, new UnityAction(this.MissionReloaded));
            VTOLAPI.SceneLoaded = (UnityAction<VTOLScenes>)Delegate.Combine(VTOLAPI.SceneLoaded, new UnityAction<VTOLScenes>(this.SceneLoaded));
            base.ModLoaded();
        }
        private void SceneLoaded(VTOLScenes scene)
        {
            switch (scene)
            {
                case VTOLScenes.ReadyRoom:
                    break;
                case VTOLScenes.Akutan:
                case VTOLScenes.CustomMapBase:
                    Log("Map Loaded");
                    base.StartCoroutine("SetUpScene");
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }

        private void MissionReloaded()
        {
            base.StartCoroutine("SetUpScene");

        }

        private IEnumerator SetUpScene()
        {
            Debug.Log("Waiting for scene load");
            while (VTMapManager.fetch == null || !VTMapManager.fetch.scenarioReady || FlightSceneManager.instance.switchingScene)
            {
                yield return null;
            }
            Debug.Log("Scene loaded");


            GameObject go = VTOLAPI.GetPlayersVehicleGameObject();
            GameObject body = GameObject.Find("body").gameObject;


            bool flag = go != null && body != null && VTOLAPI.GetPlayersVehicleEnum() == VTOLVehicles.FA26B;
            //TODO add a check to see if user is in fa-26b
            if (flag)
            {
                //steals label 
                this.labelObject = GameObject.Find("HelmPanelLabel").gameObject;
                this.text = this.labelObject.GetComponent<VTText>();


                bool flag2 = labelObject != null && this.text != null;
                if (flag2)
                {
                    
                    //change the label color and name
                    this.text.text = this.customName != null ? customName : PilotSaveManager.current.pilotName; //if user didnt left custom name setting blank it will default to their pilot's name
                    this.text.color = Color.black;
                    this.text.ApplyText();

                    /*
                    this.name = new GameObject();
                    this.name.name = "NameTag";
                    this.name.transform.parent = body.transform;
                    */

                    //pastes the new object and moves it 
                    this.name = Instantiate(this.labelObject);
                    this.name.name = "Nametag";
                    this.name.transform.parent = body.transform; //parents it to body of plane
                    this.name.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
                    
                    //from perspective of looking from the back
                    //forward, left/right, up/down
                    this.name.transform.localPosition = new Vector3(0.008865f, 0.016f, 0.0045f);

                    //(yaw, pitch, roll)
                    this.name.transform.localRotation = Quaternion.Euler(176f, 49f, 90);

                    

                    Debug.Log("Found label tings");
                    Debug.Log("Text is currently: " + this.text.text);

                }
                else
                {
                    Debug.Log("No label tings found");
                }
            }
            else
            {
                Debug.Log("Game objects not found or player is not in FA-26B");
            }


            yield break;
        }


       

        //call this in modloaded
        private void createSettings()
        {
            nameCallBack += setCustomName;
            
            Settings modSettings = new Settings(this);
            modSettings.CreateStringSetting("Custom Name", nameCallBack, null);
            VTOLAPI.CreateSettingsMenu(modSettings);

            
        }


        private void setCustomName(string value)
        {
            this.customName = value;
        }

        //stores custom name of person if they change settings
        private string customName;

        private VTText text;


        private GameObject labelObject;

        private GameObject name;

        private UnityAction<string> nameCallBack;
       
    }
}
