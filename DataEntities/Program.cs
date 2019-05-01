using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataEntities.Entities;

namespace DataEntities {
    class Program {
        static void Main(string[] args) {
            using (var entities = new PluginContext()) {
                var entity = entities.Users
                    //.Include(e => e.Stats) e.g.
                    .FirstOrDefault(e => e.Username == "citi");
            }
        }
    }
}
