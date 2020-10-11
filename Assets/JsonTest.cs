using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;

public class JsonTest : MonoBehaviour
{
    public GameObject Muchachoman;


    private IEnumerator Start()
    {
        while (!GameObject.Find("Muchachoman(Clone)"))
        {
            yield return null;
        }

        Muchachoman = GameObject.Find("Muchachoman(Clone)");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            PlayerBattleUnitHolder data = Muchachoman.GetComponent<PlayerBattleUnitHolder>();

            byte[] bites = SerializationUtility.SerializeValue(data, DataFormat.Binary);
            File.WriteAllBytes(Application.persistentDataPath + "/PAWSave.dat", bites);
            //SaveUtils.SaveToJson<byte[]>(Application.persistentDataPath + "/PAWSave.json", bites);

            //var jsonResolver = new IgnorableSerializerContractResolver();            

            //var jsonSettings = new JsonSerializerSettings() {  NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = jsonResolver };

            //SaveUtils.SaveToJson<BaseBattleUnitHolder>(Application.persistentDataPath + "/PAWSave.json", Muchachoman.GetComponent<BaseBattleUnitHolder>(), jsonSettings);
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + "/PAWSave.json");
            PlayerBattleUnitHolder unit = SerializationUtility.DeserializeValue<PlayerBattleUnitHolder>(bytes, DataFormat.Binary);

            Debug.Log(unit.UnitSkills[1].isUnlocked);
        }
    }
}
