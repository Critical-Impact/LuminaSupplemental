using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace LuminaSupplemental.SpaghettiGenerator
{
    public class DatabaseBuilder
    {
        SQLite.SQLiteConnection _libra;

        #region Builder state - to organize
        static Dictionary<long, JArray> _bossCurrency = new Dictionary<long, JArray>();

        public Dictionary<uint, HashSet<uint>> ItemDropsByMobId = new Dictionary<uint, HashSet<uint>>();
        public Dictionary<uint, HashSet<uint>> PlaceNamesByMobId = new Dictionary<uint, HashSet<uint>>();
        public List<ENpcPlaces.ENpcData> ENpcPlaces = new List<ENpcPlaces.ENpcData>();
        #endregion

        public SQLite.SQLiteConnection Libra => _libra;

        public DatabaseBuilder(SQLite.SQLiteConnection libra)
        {
            _libra = libra;
            _instance = this;
        }

        static DatabaseBuilder _instance;
        public static DatabaseBuilder Instance => _instance;

        public void Build()
        {
            var modules = new Queue<Module>(new Module[]
            {
                new Mobs(),
                new ENpcPlaces()
            });

            var total = modules.Count;
            while (modules.Count > 0)
            {
                var module = modules.Dequeue();
                PrintLine($"* {module.Name}... {total - modules.Count}/{total}");
                module.Start();
            }
        }

        public dynamic CreateItem(object id)
        {
            dynamic item = new JObject();
            item.id = id;
            return item;
        }


        public static void PrintLine(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}
