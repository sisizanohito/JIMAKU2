using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SeikaCenter;


namespace Voice
{
    [DataContract]
    class SeikaData
    {
        [DataMember]
        public Dictionary<string, SeikaStruct> Data;

        public SeikaData()
        {
            Data = new Dictionary<string, SeikaStruct>();
            Task<List<SeikaACTOR>> actorList = SeikaConnect.GetActor(@"http://localhost:7180");
            foreach(SeikaACTOR actor in actorList.Result)
            {
                int id = actor.ID;
                string name = actor.Name;
                //Console.WriteLine($"-------------------------------------------------------");
                //Console.WriteLine($"{id}:{name}");
                Data.Add(name, new SeikaStruct(name, id));
                
            }
        }
        //登録名からSeikaStructを検索
        public SeikaStruct GetActor(string name)
        {
            SeikaStruct seikaStruct = Data[name];
            return seikaStruct;
        }
    }

    [DataContract]
    class SeikaStruct
    {
        [DataMember]
        public string name;
        [DataMember]
        public int cid;

        [DataContract]
        public struct Pram {
            [DataMember]
            public decimal value;
            [DataMember]
            public decimal max;
            [DataMember]
            public decimal min;
            [DataMember]
            public decimal step;

            public Pram(decimal _value, decimal _max, decimal _min, decimal _step)
            {
                value = _value;
                max = _max;
                min = _min;
                step = _step;
            }
        }
        [DataMember]
        public Dictionary<string, Pram> parameter;

        public SeikaStruct(string _name,int _cid)
        {
            this.name = _name;
            this.cid = _cid;
            parameter = new Dictionary<string, Pram>();

            //var avatorParam = SeikaConnect.Instance().scc.GetAvatorParams_current2(cid);
            Task<SeikaACTORpram> avatorParam = SeikaConnect.Getpram(@"http://localhost:7180", cid);
            SeikaACTORpram prams = avatorParam.Result;
            foreach (KeyValuePair<string, Dictionary<string, decimal>> kvp in prams.Effects)
            {
                decimal value = kvp.Value["value"];
                decimal max = kvp.Value["max"];
                decimal min = kvp.Value["min"];
                decimal step = kvp.Value["step"];
                parameter.Add("effect"+"_"+ kvp.Key, new Pram(value, max, min, step));
            }

            foreach (KeyValuePair<string, Dictionary<string, decimal>> kvp in prams.Emotions)
            {
                decimal value = kvp.Value["value"];
                decimal max = kvp.Value["max"];
                decimal min = kvp.Value["min"];
                decimal step = kvp.Value["step"];
                parameter.Add("emotion" + "_" + kvp.Key, new Pram(value, max, min, step));
            }

        }
    }

    [DataContract]
    class SeikaACTOR
    {
        [DataMember(Name = "cid")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    class SeikaACTORpram
    {

        [DataMember(Name = "effect")]
        public Dictionary<string, Dictionary<string, decimal>> Effects { get; set; }

        [DataMember(Name = "emotion")]
        public Dictionary<string, Dictionary<string, decimal>> Emotions { get; set; }
    }
}
